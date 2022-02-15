// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using ClosedXML.Excel;
using MappingEdu.Core.Services.Auditing;
using MappingEdu.Core.Services.Import;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    public class OpenXmlToSerializedEnumerationItemMapperTests
    {
        [TestFixture]
        public class When_mapping_enumeration_item : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[]
                    {
                        new SerializedDomain {Name = "Domain1", Enumerations = new[] {new SerializedEnumeration {Name = "Enumeration1"}}},
                        new SerializedDomain {Name = "Domain2", Enumerations = new[] {new SerializedEnumeration {Name = "Enumeration1"}}}
                    }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationItems");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("D1").Value = "Code Value";
                worksheet.Cell("F1").Value = "Short Description";
                worksheet.Cell("E1").Value = "Description";

                worksheet.Cell("A2").Value = "Domain2";
                worksheet.Cell("C2").Value = "Enumeration1";
                worksheet.Cell("D2").Value = "CodeValue1";
                worksheet.Cell("F2").Value = "apple";
                worksheet.Cell("E2").Value = "big apple";
                worksheet.Cell("A3").Value = "Domain1";
                worksheet.Cell("C3").Value = "Enumeration1";
                worksheet.Cell("D3").Value = "CodeValue1";
                worksheet.Cell("F3").Value = "pear";

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_serialized_enumeration_values_models()
            {
                var domain1 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain1");
                var entity1 = domain1.Enumerations[0];
                var element = entity1.EnumerationValues[0];
                element.CodeValue.ShouldEqual("CodeValue1");
                element.ShortDescription.ShouldEqual("pear");

                var domain2 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain2");
                var entity2 = domain2.Enumerations[0];
                element = entity2.EnumerationValues[0];
                element.CodeValue.ShouldEqual("CodeValue1");
                element.ShortDescription.ShouldEqual("apple");
                element.Description.ShouldEqual("big apple");
            }

            [Test]
            public void Should_not_log_any_errors()
            {
                _auditor.GetAll().Count().ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_enumeration_value_sheet : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private TestSpreadsheetDocumentWrapper _testSpreadsheetDocument;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _testSpreadsheetDocument = new TestSpreadsheetDocumentWrapper();

                var openXmlSpreadsheetReader = GenerateStub<IOpenXmlSpreadsheetReader>();
                openXmlSpreadsheetReader.Stub(x => x.HasWorksheetPart(_testSpreadsheetDocument, "EnumerationItems")).Return(false);

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(new XLWorkbook(), _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing sheet 'EnumerationItems'");
            }
        }

        [TestFixture]
        public class When_missing_domain_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationItems");
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("D1").Value = "Code Value";

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Element Group' on sheet 'EnumerationItems'");
            }
        }

        [TestFixture]
        public class When_missing_enumeration_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationItems");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Code Value";

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Enumeration' on sheet 'EnumerationItems'");
            }
        }

        [TestFixture]
        public class When_missing_code_value_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationItems");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Code Value' on sheet 'EnumerationItems'");
            }
        }

        [TestFixture]
        public class When_duplicate_code_value_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[] { new SerializedDomain { Name = "My Domain name", Enumerations = new[] { new SerializedEnumeration { Name = "My Enumeration name" } } } }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationItems");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("D1").Value = "Code Value";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "My Enumeration name";
                worksheet.Cell("D2").Value = "CodeValue 1";
                worksheet.Cell("A3").Value = "My Domain name";
                worksheet.Cell("C3").Value = "My Enumeration name";
                worksheet.Cell("D3").Value = "CodeValue 1";
                worksheet.Cell("A4").Value = "My Domain name";
                worksheet.Cell("C4").Value = "My Enumeration name";
                worksheet.Cell("D4").Value = "CodeValue 2";

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_models_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Enumerations.Single().EnumerationValues.Single().CodeValue.ShouldEqual("CodeValue 2");
            }

            [Test]
            public void Should_log_error_for_each_duplication()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate code value defined in row 2 on sheet 'EnumerationItems'").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate code value defined in row 3 on sheet 'EnumerationItems'").ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_missing_domain_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[] { new SerializedDomain { Name = "My Domain name", Enumerations = new[] { new SerializedEnumeration { Name = "My Enumeration name" } } } }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationItems");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("D1").Value = "Code Value";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "My Enumeration name";
                worksheet.Cell("D2").Value = "Element2";
                worksheet.Cell("A3").Value = "Missing Domain";
                worksheet.Cell("C3").Value = "Enumeration1";
                worksheet.Cell("D3").Value = "Element1";
                worksheet.Cell("A4").Value = "Missing Domain";
                worksheet.Cell("C4").Value = "Enumeration1";
                worksheet.Cell("D4").Value = "Element2";

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_model_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Enumerations.Single().EnumerationValues.Single().CodeValue.ShouldEqual("Element2");
            }

            [Test]
            public void Should_log_error_for_each_missing_domain()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 3 on sheet 'EnumerationItems' due to undefined element group").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 4 on sheet 'EnumerationItems' due to undefined element group").ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_missing_enumeration_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[] { new SerializedDomain { Name = "My Domain name", Enumerations = new[] { new SerializedEnumeration { Name = "My Enumeration name" } } } }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationItems");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("D1").Value = "Code Value";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "My Enumeration name";
                worksheet.Cell("D2").Value = "Element2";
                worksheet.Cell("A3").Value = "My Domain name";
                worksheet.Cell("C3").Value = "Missing Enumeration";
                worksheet.Cell("D3").Value = "Element1";
                worksheet.Cell("A4").Value = "My Domain name";
                worksheet.Cell("C4").Value = "Missing Enumeration";
                worksheet.Cell("D4").Value = "Element2";

                var openXmlToSerializedEnumerationValueMapper = new OpenXmlToSerializedEnumerationValueMapper(_auditor);
                openXmlToSerializedEnumerationValueMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_model_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Enumerations.Single().EnumerationValues.Single().CodeValue.ShouldEqual("Element2");
            }

            [Test]
            public void Should_log_error_for_each_missing_domain()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 3 on sheet 'EnumerationItems' due to undefined enumeration").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 4 on sheet 'EnumerationItems' due to undefined enumeration").ShouldNotBeNull();
            }
        }
    }
}