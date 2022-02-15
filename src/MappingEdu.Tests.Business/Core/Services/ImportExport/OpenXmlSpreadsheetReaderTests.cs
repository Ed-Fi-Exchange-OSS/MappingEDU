// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using MappingEdu.Core.Services.Import;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    [TestFixture]
    public class OpenXmlSpreadsheetReaderTests
    {
        private SpreadsheetDocumentWrapper _spreadsheet;
        private readonly OpenXmlSpreadsheetReader _openXmlSpreadsheetReader = new OpenXmlSpreadsheetReader();

        [OneTimeSetUp]
        public void Setup()
        {
            var ms = new MemoryStream();
            using (var resourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), "OpenXmlSpreadsheetReaderTests.xlsx"))
            {
                Assert.IsNotNull(resourceStream, "Unable to find OpenXmlSpreadsheetReaderTests.xlsx");
                resourceStream.CopyTo(ms);
                resourceStream.Close();
            }

            var spreadsheet = SpreadsheetDocument.Open(ms, false);
            _spreadsheet = new SpreadsheetDocumentWrapper
            {
                SpreadsheetDocument = spreadsheet
            };
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            if (_spreadsheet != null && _spreadsheet.SpreadsheetDocument != null)
            {
                _spreadsheet.SpreadsheetDocument.Dispose();
                _spreadsheet.SpreadsheetDocument = null;
                _spreadsheet = null;
            }
        }

        [Test]
        public void GetAllHeaderCells()
        {
            _openXmlSpreadsheetReader.GetHeaderCellValues(_spreadsheet, "HeaderValueTests").Count().ShouldBeGreaterThan(0);
        }

        [Test]
        public void GetBooleanCellValueWhenFalse()
        {
            _openXmlSpreadsheetReader.GetCellValue(_spreadsheet, "CellValueTests", "A8").ShouldEqual("False");
        }

        [Test]
        public void GetBooleanCellValueWhenTrue()
        {
            _openXmlSpreadsheetReader.GetCellValue(_spreadsheet, "CellValueTests", "A6").ShouldEqual("True");
        }

        [Test]
        public void GetColumnForHeader()
        {
            _openXmlSpreadsheetReader.GetColumnForHeader(_spreadsheet, "HeaderValueTests", "Find This Header").ShouldEqual("C");
        }

        [Test]
        public void GetColumnForMissingHeader()
        {
            _openXmlSpreadsheetReader.GetColumnForHeader(_spreadsheet, "HeaderValueTests", "Not a header").ShouldBeNull();
        }

        [Test]
        public void GetMissingCellValue()
        {
            _openXmlSpreadsheetReader.GetCellValue(_spreadsheet, "CellValueTests", "ZZ1000").ShouldBeNull();
        }

        [Test]
        public void GetNumericCellValue()
        {
            _openXmlSpreadsheetReader.GetCellValue(_spreadsheet, "CellValueTests", "A5").ShouldEqual("12345");
        }

        [Test]
        public void GetStringCellValue()
        {
            _openXmlSpreadsheetReader.GetCellValue(_spreadsheet, "CellValueTests", "A7").ShouldEqual("apple");
        }

        [Test]
        public void HasWorksheetPart()
        {
            _openXmlSpreadsheetReader.HasWorksheetPart(_spreadsheet, "TestSheetName").ShouldBeTrue();
        }

        [Test]
        public void MissingWorksheetPart()
        {
            _openXmlSpreadsheetReader.HasWorksheetPart(_spreadsheet, "MissingSheet").ShouldBeFalse();
        }

        [Test]
        public void When_row_is_blank()
        {
            var rows = _openXmlSpreadsheetReader.GetRows(_spreadsheet, "CellValueTests");
            var row = rows.Single(x => x.RowIndex == 14);
            _openXmlSpreadsheetReader.IsBlankRow(_spreadsheet, row).ShouldBeTrue();
        }

        [Test]
        public void When_row_is_not_blank()
        {
            var rows = _openXmlSpreadsheetReader.GetRows(_spreadsheet, "CellValueTests");
            var row = rows.Single(x => x.RowIndex == 4);
            _openXmlSpreadsheetReader.IsBlankRow(_spreadsheet, row).ShouldBeFalse();
        }
    }
}
