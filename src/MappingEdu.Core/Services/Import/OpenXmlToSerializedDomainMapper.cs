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
    public interface IOpenXmlToSerializedDomainMapper
    {
        void Map(XLWorkbook workbook, SerializedMappedSystem mappedSystem, ImportOptions options);
    }

    public class OpenXmlToSerializedDomainMapper : IOpenXmlToSerializedDomainMapper
    {
        private const string _sheetName = "ElementGroupDefinitions";
        private const string _nameHeader = "Element Group";
        private const string _definitionHeader = "Definition";
        private const string _isExtendedHeader = "Is Extended";
        private readonly IAuditor _auditor;
        private readonly IOpenXmlToSerializedEntityMapper _serializedEntityMapper;
        private readonly IOpenXmlToSerializedEnumerationMapper _serializedEnumerationMapper;

        public OpenXmlToSerializedDomainMapper(IOpenXmlToSerializedEntityMapper serializedEntityMapper, IOpenXmlToSerializedEnumerationMapper serializedEnumerationMapper, IAuditor auditor)
        {
            _serializedEntityMapper = serializedEntityMapper;
            _serializedEnumerationMapper = serializedEnumerationMapper;
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
            if (nameHeaderColumn == null)
            {
                _auditor.Error("Missing column '{0}' on sheet '{1}'", _nameHeader, _sheetName);
                return;
            }

            var definitionHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _definitionHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();
            if (definitionHeaderColumn == null)
            {
                _auditor.Warn("Missing column '{0}' on sheet '{1}'", _definitionHeader, _sheetName);
            }

            var isExtendedHeaderColumn = headerCells.Where(x => x.Value.ToString().ToLower() == _isExtendedHeader.ToLower()).Select(x => x.Address.ColumnLetter).FirstOrDefault();

            var columns = worksheet.Row(1).Cells().Select(x => x.Value.ToString()).ToArray();

            if (columns.Any())
            {
                var columnsNotUsed = columns.Where(y => !new[] {_nameHeader.ToLower(), _definitionHeader.ToLower(), _isExtendedHeader.ToLower() }.Contains(y.ToLower())).Select(z => z);

                foreach (var column in columnsNotUsed)
                {
                    _auditor.Warn("Column '{0}' on sheet '{1}' is not recognized and is being skipped.", column, _sheetName);
                }
            }

            var domains = new List<Tuple<int, SerializedDomain>>();
            foreach (var row in worksheet.Rows())
            {
                var rowNumber = row.RowNumber();

                if (rowNumber == 1)
                    continue;

                var domain = new SerializedDomain
                {
                    Name = row.Cell(nameHeaderColumn).Value.ToString()
                };

                if (definitionHeaderColumn != null)
                    domain.Definition = row.Cell(definitionHeaderColumn).Value.ToString();

                if (isExtendedHeaderColumn != null)
                    domain.IsExtended = row.Cell(isExtendedHeaderColumn).Value.ToString();

                if (string.IsNullOrWhiteSpace(domain.Name))
                {
                    if (!row.IsEmpty()) _auditor.Info("Skipping row {0} on sheet {1} due to missing element group name", rowNumber, _sheetName);
                    continue;
                }

                domains.Add(new Tuple<int, SerializedDomain>(rowNumber, domain));
            }

            var duplicateRows = domains
                .GroupBy(x => x.Item2.Name, x => x.Item1, (key, values) => new {Key = key, RowNumbers = values.ToList()})
                .Where(y => y.RowNumbers.Count > 1)
                .SelectMany(z => z.RowNumbers)
                .AsQueryable();

            foreach (var duplicateRow in duplicateRows)
                _auditor.Warn("Duplicate element group defined in row {0} on sheet '{1}'", duplicateRow, _sheetName);

            mappedSystem.Domains = domains.Where(y => !duplicateRows.Contains(y.Item1)).Select(x => x.Item2).ToArray();

            _serializedEntityMapper.Map(workbook, mappedSystem, options);
            _serializedEnumerationMapper.Map(workbook, mappedSystem, options);
        }
    }
}