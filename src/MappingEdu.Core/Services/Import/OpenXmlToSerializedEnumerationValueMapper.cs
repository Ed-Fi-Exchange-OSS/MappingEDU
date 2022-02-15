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
    public interface IOpenXmlToSerializedEnumerationValueMapper
    {
        void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options);
    }

    public class OpenXmlToSerializedEnumerationValueMapper : IOpenXmlToSerializedEnumerationValueMapper
    {
        private const string _sheetName = "EnumerationItems";
        private const string _nameHeader = "Code Value";
        private const string _enumerationHeader = "Enumeration";
        private const string _domainHeader = "Element Group";
        private const string _shortDescriptionHeader = "Short Description";
        private const string _descriptionHeader = "Description";
        private readonly IAuditor _auditor;

        public OpenXmlToSerializedEnumerationValueMapper(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options)
        {
            IXLWorksheet worksheet;
            if (!workbook.TryGetWorksheet(_sheetName, out worksheet))
            {
                _auditor.Error("Missing sheet '{0}'", _sheetName);
                return;
            }

            var headerCells = worksheet.Row(1).Cells();

            var nameHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _nameHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            var entityHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _enumerationHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            var domainHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _domainHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (nameHeaderColumn == null || entityHeaderColumn == null || domainHeaderColumn == null)
            {
                if (nameHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _nameHeader, _sheetName);
                if (entityHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _enumerationHeader, _sheetName);
                if (domainHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _domainHeader, _sheetName);
                return;
            }

            var shortDescriptionHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _shortDescriptionHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (shortDescriptionHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _shortDescriptionHeader, _sheetName);

            var descriptionHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _descriptionHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (descriptionHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _descriptionHeader, _sheetName);

            var columns = worksheet.Row(1).Cells().Select(x => x.Value.ToString()).ToArray();
            if (columns.Any())
            {
                var columnsNotUsed = columns.Where(y => !new[] {_domainHeader.ToLower(), _nameHeader.ToLower(), _enumerationHeader.ToLower(), _descriptionHeader.ToLower(), _shortDescriptionHeader.ToLower() }.Contains(y.ToLower())).Select(z => z);

                foreach (var column in columnsNotUsed)
                {
                    _auditor.Warn("Column '{0}' on sheet '{1}' is not recognized and is being skipped.", column, _sheetName);
                }
            }

            var domains = new Dictionary<string, Dictionary<string, List<Tuple<int, SerializedEnumerationValue>>>>();
            foreach (var row in worksheet.Rows())
            {
                var rowNumber = row.RowNumber();
                if (rowNumber == 1)
                    continue;

                var domainName = row.Cell(domainHeaderColumn).Value.ToString();
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    if (!row.IsEmpty())
                        _auditor.Info("Skipping row {0} on sheet '{1}' due to missing element group name", rowNumber, _sheetName);
                    continue;
                }

                var entityName = row.Cell(entityHeaderColumn).Value.ToString();
                if (string.IsNullOrWhiteSpace(entityName))
                {
                    _auditor.Info("Skipping row {0} on sheet '{1}' due to missing entity name", rowNumber, _sheetName);
                    continue;
                }

                var element = new SerializedEnumerationValue
                {
                    CodeValue = row.Cell(nameHeaderColumn).Value.ToString()
                };

                if (shortDescriptionHeaderColumn != null)
                    element.ShortDescription = row.Cell(shortDescriptionHeaderColumn).Value.ToString();
                if (descriptionHeaderColumn != null)
                    element.Description = row.Cell(descriptionHeaderColumn).Value.ToString();

                if (string.IsNullOrWhiteSpace(element.CodeValue))
                {
                    _auditor.Info("Skipping row {0} on sheet '{1}' due to missing code value", rowNumber, _sheetName);
                    continue;
                }

                if (!domains.ContainsKey(domainName))
                    domains.Add(domainName, new Dictionary<string, List<Tuple<int, SerializedEnumerationValue>>>());
                
                var domainCollection = domains[domainName];
                if (!domainCollection.ContainsKey(entityName))
                    domainCollection.Add(entityName, new List<Tuple<int, SerializedEnumerationValue>>());

                domainCollection[entityName].Add(new Tuple<int, SerializedEnumerationValue>(rowNumber, element));
            }

            foreach (var domain in domains)
            {
                var serializedDomain = mappedSystem.Domains.FirstOrDefault(x => x.Name == domain.Key);
                if (serializedDomain == null)
                {
                    foreach (var row in domain.Value.SelectMany(x => x.Value))
                        _auditor.Warn("Skipping row {0} on sheet '{1}' due to undefined element group", row.Item1, _sheetName);
                    continue;
                }

                foreach (var enumeration in domain.Value)
                {
                    var serializedEnumeration = serializedDomain.Enumerations.FirstOrDefault(x => x.Name == enumeration.Key);
                    if (serializedEnumeration == null)
                    {
                        foreach (var row in enumeration.Value)
                            _auditor.Warn("Skipping row {0} on sheet '{1}' due to undefined enumeration", row.Item1, _sheetName);
                        continue;
                    }

                    var duplicateRows = enumeration.Value.GroupBy(x => x.Item2.CodeValue, x => x.Item1, (key, values) => new {Key = key, RowNumbers = values.ToList()})
                        .Where(y => y.RowNumbers.Count > 1)
                        .SelectMany(z => z.RowNumbers);
                    foreach (var duplicateRow in duplicateRows)
                        _auditor.Warn("Duplicate code value defined in row {0} on sheet '{1}'", duplicateRow, _sheetName);

                    serializedEnumeration.EnumerationValues = enumeration.Value.Where(y => !duplicateRows.Contains(y.Item1)).Select(x => x.Item2).ToArray();
                }
            }
        }
    }
}