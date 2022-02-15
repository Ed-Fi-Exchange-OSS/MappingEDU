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
    public interface IOpenXmlToSerializedElementMapper
    {
        void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options);
    }

    public class OpenXmlToSerializedElementMapper : IOpenXmlToSerializedElementMapper
    {
        private const string _dataTypeHeader = "Data Type";
        private const string _definitionHeader = "Definition";
        private const string _domainHeader = "Element Group";
        private const string _entityHeader = "Entity";
        private const string _fieldLengthHeader = "Field Length";
        private const string _isExtendedHeader = "Is Extended";
        private const string _nameHeader = "Element";
        private const string _sheetName = "ElementDefinitions";
        private const string _technicalNameHeader = "Technical Name";
        private const string _urlHeader = "Url";
        private readonly IAuditor _auditor;

        public OpenXmlToSerializedElementMapper(IAuditor auditor)
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
            // Headers

            var nameHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _nameHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            var entityHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _entityHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            var domainHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _domainHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (nameHeaderColumn == null || entityHeaderColumn == null || domainHeaderColumn == null)
            {
                if (nameHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _nameHeader, _sheetName);
                if (entityHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _entityHeader, _sheetName);
                if (domainHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _domainHeader, _sheetName);
                return;
            }

            var definitionHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _definitionHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (definitionHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _definitionHeader, _sheetName);
            var dataTypeHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _dataTypeHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (dataTypeHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _dataTypeHeader, _sheetName);
            var fieldLengthHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _fieldLengthHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (fieldLengthHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _fieldLengthHeader, _sheetName);
            var urlHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _urlHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (urlHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _urlHeader, _sheetName);
            var technicalNameHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _technicalNameHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (technicalNameHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _technicalNameHeader, _sheetName);

            var isExtendedHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _isExtendedHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();

            // unused columns

            var columns = worksheet.Row(1).Cells().Select(x => x.Value.ToString()).ToArray();
            if (columns.Any())
            {
                var columnsNotUsed = columns.Where(y => !new[]
                {
                    _domainHeader.ToLower(), _nameHeader.ToLower(), _definitionHeader.ToLower(), _isExtendedHeader.ToLower(),
                    _entityHeader.ToLower(), _dataTypeHeader.ToLower(), _fieldLengthHeader.ToLower(), _urlHeader.ToLower(), _technicalNameHeader.ToLower()
                }.Contains(y.ToLower())).Select(z => z); // check for extended too

                columnsNotUsed = columnsNotUsed.Where(m => !m.ToLower().StartsWith("ext_"));

                foreach (var column in columnsNotUsed)
                {
                    _auditor.Warn("Column '{0}' on sheet '{1}' is not recognized and is being skipped.", column, _sheetName);
                }
            }

            // domains

            var domains = new Dictionary<string, Dictionary<string, List<Tuple<int, SerializedElement>>>>();

            // extended fields

            var extendedColumns = headerCells.Where(x => x.Value.ToString().ToLower().StartsWith("ext_")).ToArray();
            var customDetails = new List<SerializedCustomDetailMetadata>();
            foreach (var extendedColumn in extendedColumns)
            {
                bool isBoolean = true;

                // determine if all values are boolean type

                foreach(var cell in worksheet.Column(extendedColumn.Address.ColumnNumber).Cells())
                {
                    if (cell.Address.RowNumber == 1)
                        continue;

                    var columnValue = cell.Value.ToString();
                    bool isBooleanVal;
                    if (!bool.TryParse(columnValue, out isBooleanVal) && !(columnValue == "0" || columnValue == "1"))
                    {
                        isBoolean = false;
                        break;
                    }
                };

                var customDetailName = extendedColumn.Value.ToString().Substring(4).Replace('_', ' ');
                if (customDetails.All(m => m.Name != customDetailName))
                {
                    var customDetail = new SerializedCustomDetailMetadata
                    {
                        IsBoolean = isBoolean,
                        Name = customDetailName
                    };
                    customDetails.Add(customDetail);
                }
            }
            mappedSystem.CustomDetails = customDetails.ToArray();

            // rows
            foreach (var row in worksheet.Rows())
            {
                var rowNumber = row.RowNumber();
                if (rowNumber == 1)
                    continue;

                // domain name
                var domainName = row.Cell(domainHeaderColumn).Value.ToString();
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    if (!row.IsEmpty())
                        _auditor.Info("Skipping row {0} on sheet '{1}' due to missing element group name", rowNumber, _sheetName);
                    continue;
                }

                // entity name

                var entityName = row.Cell(entityHeaderColumn).Value.ToString();
                if (string.IsNullOrWhiteSpace(entityName))
                {
                    _auditor.Info("Skipping row {0} on sheet '{1}' due to missing entity name", rowNumber, _sheetName);
                    continue;
                }

                // element name

                var element = new SerializedElement
                {
                    Name = row.Cell(nameHeaderColumn).Value.ToString()
                };

                // definition

                if (definitionHeaderColumn != null)
                    element.Definition = row.Cell(definitionHeaderColumn).Value.ToString();

                // data type

                if (dataTypeHeaderColumn != null)
                    element.DataType = row.Cell(dataTypeHeaderColumn).Value.ToString();

                // url

                if (urlHeaderColumn != null)
                    element.ItemUrl = row.Cell(urlHeaderColumn).Value.ToString();

                // technical name

                if (technicalNameHeaderColumn != null)
                    element.TechnicalName = row.Cell(technicalNameHeaderColumn).Value.ToString();

                // is extended
                if (isExtendedHeaderColumn != null)
                    element.IsExtended = row.Cell(isExtendedHeaderColumn).Value.ToString();

                // field name

                if (fieldLengthHeaderColumn != null)
                {
                    var fieldLengthStr = row.Cell(fieldLengthHeaderColumn).Value.ToString();
                    if (!string.IsNullOrWhiteSpace(fieldLengthStr))
                    {
                        int fieldLength;
                        if (int.TryParse(fieldLengthStr, out fieldLength))
                            element.FieldLength = fieldLength;
                        else
                            _auditor.Warn("Field length value on row {0} is not an integer", rowNumber);
                    }
                }

                // extended fields (custom details)

                var serializedCustomDetails = new List<SerializedElementCustomDetail>();
                foreach (var extendedColumn in extendedColumns)
                {
                    var customDetailName = extendedColumn.Value.ToString().Substring(4).Replace('_', ' '); ;
                    var columnValue = row.Cell(extendedColumn.Address.ColumnNumber).Value.ToString();
                    if (serializedCustomDetails.All(m => m.Name != customDetailName)) // add if does not exist
                    {
                        var customDetail = new SerializedElementCustomDetail
                        {
                            Name = customDetailName,
                            Value = columnValue
                        };
                        serializedCustomDetails.Add(customDetail);
                    }
                }
                element.CustomDetails = serializedCustomDetails.ToArray();

                // checks

                if (string.IsNullOrWhiteSpace(element.Name))
                {
                    _auditor.Info("Skipping row {0} on sheet '{1}' due to missing element name", rowNumber, _sheetName);
                    continue;
                }

                if (!domains.ContainsKey(domainName))
                    domains.Add(domainName, new Dictionary<string, List<Tuple<int, SerializedElement>>>());

                var domainCollection = domains[domainName];
                if (!domainCollection.ContainsKey(entityName))
                    domainCollection.Add(entityName, new List<Tuple<int, SerializedElement>>());

                domainCollection[entityName].Add(new Tuple<int, SerializedElement>(rowNumber, element));
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

                foreach (var entity in domain.Value)
                {
                    SerializedEntity serializedEntity = null;
                    var pathNames = entity.Key.Split('.').ToArray();

                    var e = serializedDomain.Entities.FirstOrDefault(x => x.Name == pathNames.First());
                    if (e != null)
                        serializedEntity = FindSubEntity(e, pathNames, 1);

                    //var serializedEntity = serializedDomain.Entities.FirstOrDefault(x => x.Name == name);
                    if (serializedEntity == null)
                    {
                        foreach (var row in entity.Value)
                            _auditor.Warn("Skipping row {0} on sheet '{1}' due to undefined entity", row.Item1, _sheetName);
                        continue;
                    }

                    var duplicateRows = entity.Value
                        .GroupBy(x => x.Item2.Name, x => x.Item1, (key, values) => new {Key = key, RowNumbers = values.ToList()})
                        .Where(y => y.RowNumbers.Count > 1)
                        .SelectMany(z => z.RowNumbers);

                    foreach (var duplicateRow in duplicateRows)
                        _auditor.Warn("Duplicate element defined in row {0} on sheet '{1}'", duplicateRow, _sheetName);

                    //// fix names

                    //var fixedElements = new List<SerializedElement>();
                    //foreach (var element in entity.Value.Where(y => !duplicateRows.Contains(y.Item1)).Select(x => x.Item2))
                    //{
                    //    element.Name = name;
                    //    fixedElements.Add(element);
                    //}
                    serializedEntity.Elements = entity.Value.Where(y => !duplicateRows.Contains(y.Item1)).Select(x => x.Item2).ToArray();
                }
            }
        }

        public SerializedEntity FindSubEntity(SerializedEntity node, string[] path, int entityIndex)
        {
            if (path.Length <= entityIndex)
                return node;

            if (node.SubEntities == null)
                return null;

            var child = node.SubEntities.FirstOrDefault(x => x.Name == path[entityIndex]);
            if(child != null)
                return FindSubEntity(child, path, entityIndex + 1);

            return null;
        }
    }
}