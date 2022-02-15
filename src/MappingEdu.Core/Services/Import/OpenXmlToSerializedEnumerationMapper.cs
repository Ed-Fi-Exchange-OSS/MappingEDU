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
    public interface IOpenXmlToSerializedEnumerationMapper
    {
        void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options);
    }

    public class OpenXmlToSerializedEnumerationMapper : IOpenXmlToSerializedEnumerationMapper
    {
        private const string _sheetName = "EnumerationDefinitions";
        private const string _nameHeader = "Enumeration";
        private const string _domainHeader = "Element Group";
        private const string _definitionHeader = "Definition";
        private const string _isExtendedHeader = "Is Extended";
        private readonly IAuditor _auditor;
        private readonly IOpenXmlToSerializedEnumerationValueMapper _serializedEnumerationValueMapper;

        public OpenXmlToSerializedEnumerationMapper(IOpenXmlToSerializedEnumerationValueMapper serializedEnumerationValueMapper, IAuditor auditor)
        {
            _serializedEnumerationValueMapper = serializedEnumerationValueMapper;
            _auditor = auditor;
        }

        public void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options)
        {
            IXLWorksheet worksheet;
            if (!workbook.TryGetWorksheet(_sheetName, out worksheet))
            {
                _auditor.Info("Not importing enumerations because sheet '{0}' could not be found", _sheetName);
                return;
            }

            var headerCells = worksheet.Row(1).Cells();

            var nameHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _nameHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            var domainHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _domainHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (nameHeaderColumn == null || domainHeaderColumn == null)
            {
                if (nameHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _nameHeader, _sheetName);
                if (domainHeaderColumn == null)
                    _auditor.Error("Missing column '{0}' on sheet '{1}'", _domainHeader, _sheetName);
                return;
            }
            var definitionHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _definitionHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (definitionHeaderColumn == null)
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _definitionHeader, _sheetName);

            var isExtendedHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _isExtendedHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();

            var columns = worksheet.Row(1).Cells().Select(x => x.Value.ToString().ToLower()).ToArray();

            if (columns.Any())
            {
                var columnsNotUsed = columns.Where(y => !new[] {_domainHeader.ToLower(), _nameHeader.ToLower(), _definitionHeader.ToLower(), _isExtendedHeader.ToLower() }.Contains(y)).Select(z => z);

                columnsNotUsed = columnsNotUsed.Where(m => !m.ToLower().StartsWith("ext_"));

                foreach (var column in columnsNotUsed)
                {
                    _auditor.Warn("Column '{0}' on sheet '{1}' is not recognized and is being skipped.", column, _sheetName);
                }
            }

            var domains = new Dictionary<string, List<Tuple<int, SerializedEnumeration>>>();

            // extended fields

            var extendedColumns = headerCells.Where(x => x.Value.ToString().ToLower().StartsWith("ext_")).ToArray();
            var customDetails = new List<SerializedCustomDetailMetadata>();
            foreach (var extendedColumn in extendedColumns)
            {
                bool isBoolean = true;

                // determine if all values are boolean type

                foreach (var cell in worksheet.Column(extendedColumn.Address.ColumnNumber).Cells())
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

            var addCustomDetails = new List<SerializedCustomDetailMetadata>();
            foreach (var detail in customDetails.ToArray())
            {
                if (!(mappedSystem.CustomDetails?.Select(x => x.Name.ToLower()).Equals(detail.Name.ToLower()) ?? false))
                    addCustomDetails.Add(detail);
            }
            if(mappedSystem.CustomDetails != null)
                mappedSystem.CustomDetails.Union(addCustomDetails.ToArray());

            foreach (var row in worksheet.Rows())
            {
                var rowNumber = row.RowNumber();
                if (rowNumber == 1)
                    continue;

                var domainName = row.Cell(domainHeaderColumn).Value.ToString();
                if (string.IsNullOrWhiteSpace(domainName))
                {
                    if (!row.IsEmpty())
                        _auditor.Info("Skipping row {0} on sheet '{1}' due to empty element group name", rowNumber, _sheetName);
                    continue;
                }

                var enumeration = new SerializedEnumeration
                {
                    Name = row.Cell(nameHeaderColumn).Value.ToString()
                };

                if (definitionHeaderColumn != null)
                    enumeration.Definition = row.Cell(definitionHeaderColumn).Value.ToString();

                if (isExtendedHeaderColumn != null)
                    enumeration.IsExtended = row.Cell(isExtendedHeaderColumn).Value.ToString();

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
                enumeration.CustomDetails = serializedCustomDetails.ToArray();

                if (string.IsNullOrWhiteSpace(enumeration.Name))
                {
                    _auditor.Info("Skipping row {0} on sheet '{1}' due to empty enumeration name", rowNumber, _sheetName);
                    continue;
                }

                if (!domains.ContainsKey(domainName))
                    domains.Add(domainName, new List<Tuple<int, SerializedEnumeration>>());

                domains[domainName].Add(new Tuple<int, SerializedEnumeration>(rowNumber, enumeration));
            }

            foreach (var domain in domains)
            {
                var serializedDomain = mappedSystem.Domains.FirstOrDefault(x => x.Name == domain.Key);
                if (serializedDomain == null)
                {
                    foreach (var row in domain.Value)
                        _auditor.Warn("Skipping row {0} on sheet '{1}' due to undefined element group", row.Item1, _sheetName);
                    continue;
                }

                var duplicateRows = domain.Value.GroupBy(x => x.Item2.Name, x => x.Item1, (key, values) => new {Key = key, RowNumbers = values.ToList()})
                    .Where(y => y.RowNumbers.Count > 1)
                    .SelectMany(z => z.RowNumbers);
                foreach (var duplicateRow in duplicateRows)
                    _auditor.Warn("Duplicate enumeration defined in row {0} on sheet '{1}'", duplicateRow, _sheetName);

                serializedDomain.Enumerations = domain.Value.Where(y => !duplicateRows.Contains(y.Item1)).Select(x => x.Item2).ToArray();
            }

            _serializedEnumerationValueMapper.Map(workbook, mappedSystem, options);
        }
    }
}
