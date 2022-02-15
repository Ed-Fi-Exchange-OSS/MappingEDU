// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using MappingEdu.Core.Services.Auditing;

namespace MappingEdu.Core.Services.Import
{
    public interface IOpenXmlToSerializedEntityMapper
    {
        void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options);
    }

    public class OpenXmlToSerializedEntityMapper : IOpenXmlToSerializedEntityMapper
    {
        private const string _sheetName = "EntityDefinitions";
        private const string _nameHeader = "Entity";
        private const string _domainHeader = "Element Group";
        private const string _definitionHeader = "Definition";
        private const string _isExtendedHeader = "Is Extended";
        private readonly IAuditor _auditor;
        private readonly IOpenXmlToSerializedElementMapper _serializedElementMapper;

        public OpenXmlToSerializedEntityMapper(IOpenXmlToSerializedElementMapper serializedElementMapper, IAuditor auditor)
        {
            _serializedElementMapper = serializedElementMapper;
            _auditor = auditor;
        }

        public void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options)
        {
            IXLWorksheet worksheet;
            if (!workbook.TryGetWorksheet(_sheetName, out worksheet))
            {
                _auditor.Info("Not importing entities because sheet '{0}' could not be found", _sheetName);
                return;
            }

            var nameHeaderColumn = worksheet.Row(1).Cells().Where(x => x.Value.ToString().ToLower() == _nameHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            var domainHeaderColumn = worksheet.Row(1).Cells().Where(x => x.Value.ToString().ToLower() == _domainHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (nameHeaderColumn == null || domainHeaderColumn == null)
            {
                if (nameHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _nameHeader, _sheetName);
                if (domainHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _domainHeader, _sheetName);
                return;
            }
            var definitionHeaderColumn = worksheet.Row(1).Cells().Where(x => x.Value.ToString().ToLower() == _definitionHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (definitionHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _definitionHeader, _sheetName);

            var isExtendedHeaderColumn = worksheet.Row(1).Cells().Where(x => x.Value.ToString().ToLower() == _isExtendedHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();

            // columns

            var columns = worksheet.Row(1).Cells().Select(x => x.Value.ToString()).ToArray();
            if (columns.Any())
            {
                var columnsNotUsed = columns.Where(y => !new[] {_domainHeader.ToLower(), _nameHeader.ToLower(), _definitionHeader.ToLower(), _isExtendedHeader.ToLower() }.Contains(y.ToLower())).Select(z => z);

                foreach (var column in columnsNotUsed)
                {
                    _auditor.Warn("Column '{0}' on sheet '{1}' is not recognized and is being skipped.", column, _sheetName);
                }
            }

            // add entities to domain list

            var domains = new Dictionary<string, List<Tuple<int, SerializedEntity>>>();
            foreach (var row in worksheet.Rows())
            {
                var rowNumber = row.RowNumber();
                if (rowNumber == 1)
                    continue;

                var domainName = row.Cell(domainHeaderColumn).Value.ToString();
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    if (!row.IsEmpty()) _auditor.Info("Skipping row {0} on sheet '{1}' due to empty element group name", rowNumber, _sheetName);
                    continue;
                }

                var entity = new SerializedEntity
                {
                    Name = row.Cell(nameHeaderColumn).Value.ToString()
                };

                if (definitionHeaderColumn != null)
                    entity.Definition = row.Cell(definitionHeaderColumn).Value.ToString();

                if (isExtendedHeaderColumn != null)
                    entity.IsExtended = row.Cell(isExtendedHeaderColumn).Value.ToString();

                if (string.IsNullOrWhiteSpace(entity.Name))
                {
                    _auditor.Info("Skipping row {0} on sheet '{1}' due to empty entity name", rowNumber, _sheetName);
                    continue;
                }

                if (!domains.ContainsKey(domainName))
                    domains.Add(domainName, new List<Tuple<int, SerializedEntity>>());

                domains[domainName].Add(new Tuple<int, SerializedEntity>(rowNumber, entity));
            }

            foreach (var domain in domains)
            {
                // search for undefined domains

                var serializedDomain = mappedSystem.Domains.FirstOrDefault(x => x.Name == domain.Key);
                if (serializedDomain == null)
                {
                    foreach (var row in domain.Value)
                        _auditor.Warn("Skipping row {0} on sheet '{1}' due to undefined element group", row.Item1, _sheetName);
                    continue;
                }
                serializedDomain.Entities = new SerializedEntity[] {};

                // search for duplicate rows

                var duplicateRows = domain.Value.GroupBy(x => x.Item2.Name, x => x.Item1, (key, values) => new {Key = key, RowNumbers = values.ToList()}).Where(y => y.RowNumbers.Count > 1).SelectMany(z => z.RowNumbers).AsQueryable();

                foreach (var duplicateRow in duplicateRows)
                    _auditor.Warn("Duplicate entity defined in row {0} on sheet '{1}'", duplicateRow, _sheetName);

                // search domains for parent child entities

                var entityGroups = new List<SerializedEntityGroup>();
                var rawEntities = domain.Value.Where(y => !duplicateRows.Contains(y.Item1)).Select(x => x.Item2).OrderBy(o => o.Name).ToArray();

                // get unique entity names from delimited string

                foreach (var rawEntity in rawEntities)
                {
                    var splitName = rawEntity.Name.Split('.');
                    var name = splitName.Last();

                    var parent = entityGroups.FirstOrDefault(x => x.Name == splitName[0]);
                    if (parent == null && splitName.Length == 1)
                    {
                        var entityGroup = new SerializedEntityGroup
                        {
                            Name = name,
                            OriginalName = rawEntity.Name,
                            Definition = rawEntity.Definition,
                            Elements = rawEntity.Elements,
                            IsExtended = rawEntity.IsExtended,
                            Children = new List<SerializedEntityGroup>()
                        };
                        entityGroups.Add(entityGroup);
                    }
                    else if (parent != null && splitName.Length > 1)
                    {
                        for (var i = 1; i < splitName.Length - 1; i++)
                        {
                            parent = parent.Children.FirstOrDefault(x => x.Name == splitName[i]);
                            if (parent == null) break;
                        }

                        if (parent == null)
                        {
                            _auditor.Warn("Undefined parent '{0}' on sheet '{1}'", rawEntity.Name, _sheetName);
                        }
                        else if (parent.Children.Any(x => x.Name == name))
                        {
                            _auditor.Warn("Duplicate entity defined '{0}' on sheet '{1}'", rawEntity.Name, _sheetName);
                        }
                        else
                        {
                            var entityGroup = new SerializedEntityGroup
                            {
                                Name = name,
                                OriginalName = rawEntity.Name,
                                Definition = rawEntity.Definition,
                                Elements = rawEntity.Elements,
                                IsExtended = rawEntity.IsExtended,
                                Children = new List<SerializedEntityGroup>()
                            };
                            parent.Children.Add(entityGroup);
                        }
                    }
                    else
                    {
                        _auditor.Warn("Undefined parent '{0}' on sheet '{1}'", rawEntity.Name, _sheetName);
                    }
                }
                serializedDomain.Entities = entityGroups.Select(serializedEntityGroup => serializedEntityGroup.ToEntity()).ToArray();
            }

            _serializedElementMapper.Map(workbook, mappedSystem, options);
        }

        private IList<SerializedEntityGroup> BuildTree(IEnumerable<SerializedEntityGroup> source)
        {
            var groups = source.GroupBy(i => i.Parent);

            var roots = groups.FirstOrDefault(g => g.Key == null).ToList();

            if (roots.Count > 0)
            {
                var dict = groups.Where(g => g.Key != null).ToDictionary(g => g.Key, g => g.ToList());
                for (var i = 0; i < roots.Count; i++)
                    AddChildren(roots[i], dict);
            }

            return roots;
        }

        private void AddChildren(SerializedEntityGroup parent, IDictionary<string, List<SerializedEntityGroup>> source)
        {
            if (source.ContainsKey(parent.Name))
            {
                parent.Children = source[parent.Name];
                foreach (var t in parent.Children)
                    AddChildren(t, source);
            }
            else
            {
                parent.Children = new List<SerializedEntityGroup>();
            }
        }

        private class SerializedEntityGroup
        {
            public List<SerializedEntityGroup> Children { get; set; }

            public string Definition { get; set; }

            public SerializedElement[] Elements { get; set; }

            public string Name { get; set; }

            public string OriginalName { get; set; }

            public string Parent { get; set; }

            public string IsExtended { get; set; }

            public SerializedEntity ToEntity()
            {
                var entity = new SerializedEntity
                {
                    Name = Name,
                    Definition = Definition,
                    Elements = Elements,
                    IsExtended = IsExtended,
                    SubEntities = Children.Select(child => child.ToEntity()).ToArray()
                };

                return entity;
            }
        }
    }
}