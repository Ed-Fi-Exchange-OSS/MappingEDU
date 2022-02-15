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
using Should;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    public class OpenXmlToSerializedElementMapperTests
    {
        [TestFixture]
        public class When_mapping_elements : TestBase
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
                        new SerializedDomain {Name = "Domain1", Entities = new[] {new SerializedEntity {Name = "Entity1"}}},
                        new SerializedDomain {Name = "Domain2", Entities = new[] {new SerializedEntity {Name = "Entity1"}}}
                    }
                };

                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("D1").Value = "Element";
                worksheet.Cell("E1").Value = "Data Type";
                worksheet.Cell("F1").Value = "Field Length";
                worksheet.Cell("G1").Value = "Url";
                worksheet.Cell("H1").Value = "Technical Name";

                worksheet.Cell("A2").Value = "Domain2";
                worksheet.Cell("C2").Value = "Entity1";
                worksheet.Cell("D2").Value = "Element1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("E2").Value = "my datatype";
                worksheet.Cell("F2").Value = "100";
                worksheet.Cell("G2").Value = "my url";
                worksheet.Cell("H2").Value = "my technical name";
                worksheet.Cell("A3").Value = "Domain1";
                worksheet.Cell("C3").Value = "Entity1";
                worksheet.Cell("D3").Value = "Element1";
                worksheet.Cell("B3").Value = "pear";

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_serialized_entity_models()
            {
                var domain1 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain1");
                var entity1 = domain1.Entities[0];
                var element = entity1.Elements[0];
                element.Name.ShouldEqual("Element1");
                element.Definition.ShouldEqual("pear");

                var domain2 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain2");
                var entity2 = domain2.Entities[0];
                element = entity2.Elements[0];
                element.Name.ShouldEqual("Element1");
                element.Definition.ShouldEqual("apple");
                element.DataType.ShouldEqual("my datatype");
                element.FieldLength.ShouldEqual(100);
                element.ItemUrl.ShouldEqual("my url");
                element.TechnicalName.ShouldEqual("my technical name");
            }

            [Test]
            public void Should_not_log_any_errors()
            {
                _auditor.GetAll().Count().ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_entity_sheet : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private ImportOptions _options;

            [OneTimeSetUp]
            public void Setup()
            {
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem();

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(new XLWorkbook(), _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing sheet 'ElementDefinitions'");
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

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("D1").Value = "Element";

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Element Group' on sheet 'ElementDefinitions'");
            }
        }

        [TestFixture]
        public class When_missing_entity_column : TestBase
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

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("D1").Value = "Element";

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Entity' on sheet 'ElementDefinitions'");
            }
        }

        [TestFixture]
        public class When_missing_element_column : TestBase
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

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Entity";

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Element' on sheet 'ElementDefinitions'");
            }
        }

        [TestFixture]
        public class When_duplicate_element_found_in_data : TestBase
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
                    Domains = new[] { new SerializedDomain { Name = "My Domain name", Entities = new[] { new SerializedEntity { Name = "My Entity name" } } } }
                };

                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("D1").Value = "Element";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "My Entity name";
                worksheet.Cell("D2").Value = "Element1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "My Domain name";
                worksheet.Cell("C3").Value = "My Entity name";
                worksheet.Cell("D3").Value = "Element1";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "My Domain name";
                worksheet.Cell("C4").Value = "My Entity name";
                worksheet.Cell("D4").Value = "Element2";
                worksheet.Cell("B4").Value = "pear";
                
                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_models_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Entities.Single().Elements.Single().Name.ShouldEqual("Element2");
            }

            [Test]
            public void Should_log_error_for_each_duplication()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate element defined in row 2 on sheet 'ElementDefinitions'").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate element defined in row 3 on sheet 'ElementDefinitions'").ShouldNotBeNull();
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
                    Domains = new[] { new SerializedDomain { Name = "My Domain name", Entities = new[] { new SerializedEntity { Name = "My Entity name" } } } }
                };

                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("D1").Value = "Element";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "My Entity name";
                worksheet.Cell("D2").Value = "Element2";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "Missing Domain";
                worksheet.Cell("C3").Value = "Entity1";
                worksheet.Cell("D3").Value = "Element1";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "Missing Domain";
                worksheet.Cell("C4").Value = "Entity1";
                worksheet.Cell("D4").Value = "Element2";
                worksheet.Cell("B4").Value = "orange";

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_model_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Entities.Single().Elements.Single().Name.ShouldEqual("Element2");
            }

            [Test]
            public void Should_log_error_for_each_missing_domain()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 3 on sheet 'ElementDefinitions' due to undefined element group").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 4 on sheet 'ElementDefinitions' due to undefined element group").ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_missing_entity_found_in_data : TestBase
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
                    Domains = new[] { new SerializedDomain { Name = "My Domain name", Entities = new[] { new SerializedEntity { Name = "My Entity name" } } } }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("D1").Value = "Element";
                worksheet.Cell("E1").Value = "Data Type";
                worksheet.Cell("F1").Value = "Field Length";
                worksheet.Cell("G1").Value = "Url";
                worksheet.Cell("H1").Value = "Technical Name";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "My Entity name";
                worksheet.Cell("D2").Value = "Element2";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "My Domain name";
                worksheet.Cell("C3").Value = "Missing Entity";
                worksheet.Cell("D3").Value = "Element1";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "My Domain name";
                worksheet.Cell("C4").Value = "Missing Entity";
                worksheet.Cell("D4").Value = "Element2";
                worksheet.Cell("B4").Value = "orange";

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_model_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Entities.Single().Elements.Single().Name.ShouldEqual("Element2");
            }

            [Test]
            public void Should_log_error_for_each_missing_domain()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 3 on sheet 'ElementDefinitions' due to undefined entity").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 4 on sheet 'ElementDefinitions' due to undefined entity").ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_field_length_is_not_an_integer : TestBase
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
                    Domains = new[] { new SerializedDomain { Name = "Domain2", Entities = new[] { new SerializedEntity { Name = "Entity1" } } } }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("ELementDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("D1").Value = "Element";
                worksheet.Cell("F1").Value = "Field Length";

                worksheet.Cell("A2").Value = "Domain2";
                worksheet.Cell("C2").Value = "Entity1";
                worksheet.Cell("D2").Value = "Element1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("F2").Value = "not an integer";

                var openXmlToSerializedElementMapper = new OpenXmlToSerializedElementMapper(_auditor);
                openXmlToSerializedElementMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_serialized_entity_models()
            {
                var domain2 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain2");
                var entity2 = domain2.Entities[0];
                var element = entity2.Elements[0];
                element.Name.ShouldEqual("Element1");
                element.Definition.ShouldEqual("apple");
                element.FieldLength.ShouldBeNull();
            }

            [Test]
            public void Should_log_warning_for_data_type_missing()
            {
                _auditor.GetAll().Count(z => z.Message.Equals("Missing column 'Data Type' on sheet 'ElementDefinitions'")).ShouldEqual(1);
            }

            [Test]
            public void Should_log_warning_for_field_length()
            {
                _auditor.GetAll().Count(z => z.Message.Equals("Field length value on row 2 is not an integer")).ShouldEqual(1);
            }

            [Test]
            public void Should_log_warning_for_technical_name_missing()
            {
                _auditor.GetAll().Count(z => z.Message.Equals("Missing column 'Technical Name' on sheet 'ElementDefinitions'")).ShouldEqual(1);
            }

            [Test]
            public void Should_log_warning_for_url_missing()
            {
                _auditor.GetAll().Count(z => z.Message.Equals("Missing column 'Url' on sheet 'ElementDefinitions'")).ShouldEqual(1);
            }

            [Test]
            public void Should_log_warnings()
            {
                _auditor.GetAll().Count().ShouldEqual(4);
            }
        }
    }
}