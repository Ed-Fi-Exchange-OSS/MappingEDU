// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace MappingEdu.Core.Services.Import
{
    public interface IOpenXmlSpreadsheetReader
    {
        bool HasWorksheetPart(ISpreadsheetDocumentWrapper document, string sheetName);

        IEnumerable<Row> GetRows(ISpreadsheetDocumentWrapper document, string sheetName);

        IEnumerable<string> GetHeaderCellValues(ISpreadsheetDocumentWrapper document, string sheetName);

        bool IsBlankRow(ISpreadsheetDocumentWrapper document, Row row);

        string GetCellValue(ISpreadsheetDocumentWrapper document, string sheetName, string cellAddress);

        string GetColumnForHeader(ISpreadsheetDocumentWrapper document, string sheetName, string header);
    }

    public class OpenXmlSpreadsheetReader : IOpenXmlSpreadsheetReader
    {
        private readonly Regex _columnNameRegex = new Regex(@"[A-Za-z]");

        private readonly Regex _rowNameRegex = new Regex(@"\d+");

        public bool HasWorksheetPart(ISpreadsheetDocumentWrapper document, string sheetName)
        {
            return document.SpreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().Any(s => s.Name == sheetName);
        }

        public IEnumerable<Row> GetRows(ISpreadsheetDocumentWrapper document, string sheetName)
        {
            var sheet = GetWorksheetPart(document, sheetName);
            return sheet.Worksheet.Descendants<Row>();
        }

        public IEnumerable<string> GetHeaderCellValues(ISpreadsheetDocumentWrapper document, string sheetName)
        {
            var worksheetPart = GetWorksheetPart(document, sheetName);
            return worksheetPart.Worksheet.Descendants<Cell>().Where(x => GetRowNumber(x.CellReference.Value) == 1).Select(x => GetCellValue(document, x));
        }

        public bool IsBlankRow(ISpreadsheetDocumentWrapper document, Row row)
        {
            var cells = row.Descendants<Cell>();
            foreach (var cell in cells)
            {
                if (!string.IsNullOrWhiteSpace(GetCellValue(document, cell)))
                    return false;
            }

            return true;
        }

        public string GetCellValue(ISpreadsheetDocumentWrapper document, string sheetName, string cellAddress)
        {
            var worksheetPart = GetWorksheetPart(document, sheetName);
            var cell = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(x => x.CellReference == cellAddress);
            return GetCellValue(document, cell);
        }

        public string GetColumnForHeader(ISpreadsheetDocumentWrapper document, string sheetName, string header)
        {
            var worksheetPart = GetWorksheetPart(document, sheetName);
            var cells = worksheetPart.Worksheet.Descendants<Cell>().Where(x => GetRowNumber(x.CellReference.Value) == 1);
            var matchedHeaderCell = cells.FirstOrDefault(x => GetCellValue(document, x) == header);
            if (matchedHeaderCell == null)
                return null;
            return GetColumnName(matchedHeaderCell.CellReference.Value);
        }

        private WorksheetPart GetWorksheetPart(ISpreadsheetDocumentWrapper document, string sheetName)
        {
            var sheet = document.SpreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>().First(s => sheetName.Equals(s.Name));
            return (WorksheetPart) document.SpreadsheetDocument.WorkbookPart.GetPartById(sheet.Id);
        }

        private string GetCellValue(ISpreadsheetDocumentWrapper document, Cell cell)
        {
            if (cell == null)
                return null;

            if (cell.DataType == null || cell.InnerText == "")
                return cell.InnerText;

            if (cell.DataType.Value == CellValues.Boolean)
                return cell.InnerText == "0" ? "False" : "True";

            if (cell.DataType.Value != CellValues.SharedString)
                return cell.InnerText;

            var stringTable = document.SpreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            return stringTable.SharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
        }

        private int GetRowNumber(string cellName)
        {
            var match = _rowNameRegex.Match(cellName);
            return int.Parse(match.Value);
        }

        private string GetColumnName(string cellName)
        {
            var match = _columnNameRegex.Match(cellName);
            return match.Value;
        }
    }
}