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
    public class OpenXmlToSerializedDomainMapperTests
    {
        [TestFixture]
        public class When_mapping_domains : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEntityMapper _serializedEntityMapper;
            private IOpenXmlToSerializedEnumerationMapper _serializedEnumerationMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEntityMapper = GenerateStub<IOpenXmlToSerializedEntityMapper>();
                _serializedEnumerationMapper = GenerateStub<IOpenXmlToSerializedEnumerationMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ElementGroupDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("B2").Value = "apple";

                var openXmlToSerializedDomainMapper = new OpenXmlToSerializedDomainMapper(_serializedEntityMapper, _serializedEnumerationMapper, _auditor);
                openXmlToSerializedDomainMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_serialized_domain_models()
            {
                _serializedMappedSystem.Domains.Single().Name.ShouldEqual("My Domain name");
                _serializedMappedSystem.Domains.Single().Definition.ShouldEqual("apple");
            }

            [Test]
            public void Should_create_serialized_entities()
            {
                _serializedEntityMapper.AssertWasCalled(x => x.Map(_workbook, _serializedMappedSystem, _options));
            }

            [Test]
            public void Should_create_serialized_enumerations()
            {
                _serializedEnumerationMapper.AssertWasCalled(x => x.Map(_workbook, _serializedMappedSystem, _options));
            }

            [Test]
            public void Should_not_log_any_errors()
            {
                _auditor.GetAll().Count().ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_domain_sheet : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEntityMapper _serializedEntityMapper;
            private ImportOptions _options;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEntityMapper = GenerateStub<IOpenXmlToSerializedEntityMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();

                var openXmlToSerializedDomainMapper = new OpenXmlToSerializedDomainMapper(_serializedEntityMapper, null, _auditor);
                openXmlToSerializedDomainMapper.Map(new XLWorkbook(), _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing sheet 'ElementGroupDefinitions'");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_missing_domain_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEntityMapper _serializedEntityMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEntityMapper = GenerateStub<IOpenXmlToSerializedEntityMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ElementGroupDefinitions");
                worksheet.Cell("B1").Value = "Definition";

                var openXmlToSerializedDomainMapper = new OpenXmlToSerializedDomainMapper(_serializedEntityMapper, null, _auditor);
                openXmlToSerializedDomainMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Element Group' on sheet 'ElementGroupDefinitions'");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_missing_definition_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEntityMapper _serializedEntityMapper;
            private IOpenXmlToSerializedEnumerationMapper _serializedEnumerationMapper;

            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEntityMapper = GenerateStub<IOpenXmlToSerializedEntityMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _serializedEnumerationMapper = GenerateStub<IOpenXmlToSerializedEnumerationMapper>();

                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ElementGroupDefinitions");
                worksheet.Cell("A1").Value = "Element Group";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("B2").Value = "apple";

                var openXmlToSerializedDomainMapper = new OpenXmlToSerializedDomainMapper(_serializedEntityMapper, _serializedEnumerationMapper, _auditor);
                openXmlToSerializedDomainMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_warning()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Warning);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Definition' on sheet 'ElementGroupDefinitions'");
            }
        }

        [TestFixture]
        public class When_columns_exist_that_are_not_used : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEntityMapper _serializedEntityMapper;
            private IOpenXmlToSerializedEnumerationMapper _serializedEnumerationMapper;

            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEntityMapper = GenerateStub<IOpenXmlToSerializedEntityMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _workbook = new XLWorkbook();
                _serializedEnumerationMapper = GenerateStub<IOpenXmlToSerializedEnumerationMapper>();

                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ElementGroupDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Ferrari";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("B2").Value = "apple";

                var openXmlToSerializedDomainMapper = new OpenXmlToSerializedDomainMapper(_serializedEntityMapper, _serializedEnumerationMapper, _auditor);
                openXmlToSerializedDomainMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_warning()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Warning);
                _auditor.GetAll().Single().Message.ShouldEqual("Column 'Ferrari' on sheet 'ElementGroupDefinitions' is not recognized and is being skipped.");
            }
        }

        [TestFixture]
        public class When_duplicate_domain_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEntityMapper _serializedEntityMapper;
            private IOpenXmlToSerializedEnumerationMapper _serializedEnumerationMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEntityMapper = GenerateStub<IOpenXmlToSerializedEntityMapper>();
                _serializedEnumerationMapper = GenerateStub<IOpenXmlToSerializedEnumerationMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ElementGroupDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Ferrari";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "My Domain name";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "Different Name";
                worksheet.Cell("B4").Value = "pear";

                var openXmlToSerializedDomainMapper = new OpenXmlToSerializedDomainMapper(_serializedEntityMapper, _serializedEnumerationMapper, _auditor);
                openXmlToSerializedDomainMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_models_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Name.ShouldEqual("Different Name");
                _serializedMappedSystem.Domains.Single().Definition.ShouldEqual("pear");
            }

            [Test]
            public void Should_log_error_for_each_duplication()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate element group defined in row 2 on sheet 'ElementGroupDefinitions'").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate element group defined in row 3 on sheet 'ElementGroupDefinitions'").ShouldNotBeNull();
            }
        }
    }
}