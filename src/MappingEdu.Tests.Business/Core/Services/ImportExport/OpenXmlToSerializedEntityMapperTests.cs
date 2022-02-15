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
    public class OpenXmlToSerializedEntityMapperTests
    {
        [TestFixture]
        public class When_mapping_entities : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedElementMapper _serializedElementMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedElementMapper = GenerateStub<IOpenXmlToSerializedElementMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[]
                    {
                        new SerializedDomain {Name = "Domain1"},
                        new SerializedDomain {Name = "Domain2"}
                    }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EntityDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("B1").Value = "Definition";

                worksheet.Cell("A2").Value = "Domain2";
                worksheet.Cell("C2").Value = "Entity1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "Domain1";
                worksheet.Cell("C3").Value = "Different Entity";
                worksheet.Cell("B3").Value = "pear";

                var openXmlToSerializelEntityMapper = new OpenXmlToSerializedEntityMapper(_serializedElementMapper, _auditor);
                openXmlToSerializelEntityMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_serialized_elements()
            {
                _serializedElementMapper.AssertWasCalled(x => x.Map(_workbook, _serializedMappedSystem, _options));
            }

            [Test]
            public void Should_create_serialized_entity_models()
            {
                var domain1 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain1");
                var entity = domain1.Entities[0];
                entity.Name.ShouldEqual("Different Entity");
                entity.Definition.ShouldEqual("pear");

                var domain2 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain2");
                entity = domain2.Entities[0];
                entity.Name.ShouldEqual("Entity1");
                entity.Definition.ShouldEqual("apple");
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
            private IOpenXmlToSerializedElementMapper _serializedElementMapper;
            private ImportOptions _options;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedElementMapper = GenerateStub<IOpenXmlToSerializedElementMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[]
                    {
                        new SerializedDomain {Name = "Domain1"},
                        new SerializedDomain {Name = "Domain2"}
                    }
                };

                var openXmlToSerializelEntityMapper = new OpenXmlToSerializedEntityMapper(_serializedElementMapper, _auditor);
                openXmlToSerializelEntityMapper.Map(new XLWorkbook(), _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Info);
                _auditor.GetAll().Single().Message.ShouldEqual("Not importing entities because sheet 'EntityDefinitions' could not be found");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.Count(x => x.Entities != null).ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_domain_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedElementMapper _serializedElementMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedElementMapper = GenerateStub<IOpenXmlToSerializedElementMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[]
                    {
                        new SerializedDomain {Name = "Domain1"},
                        new SerializedDomain {Name = "Domain2"}
                    }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EntityDefinitions");
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("B1").Value = "Definition";

                var openXmlToSerializelEntityMapper = new OpenXmlToSerializedEntityMapper(_serializedElementMapper, _auditor);
                openXmlToSerializelEntityMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Element Group' on sheet 'EntityDefinitions'");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.Count(x => x.Entities != null).ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_entity_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedElementMapper _serializedElementMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedElementMapper = GenerateStub<IOpenXmlToSerializedElementMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[]
                    {
                        new SerializedDomain {Name = "Domain1"},
                        new SerializedDomain {Name = "Domain2"}
                    }
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EntityDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";

                var openXmlToSerializelEntityMapper = new OpenXmlToSerializedEntityMapper(_serializedElementMapper, _auditor);
                openXmlToSerializelEntityMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Entity' on sheet 'EntityDefinitions'");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.Count(x => x.Entities != null).ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_definition_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedElementMapper _serializedElementMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedElementMapper = GenerateStub<IOpenXmlToSerializedElementMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[]
                    {
                        new SerializedDomain {Name = "Domain1"},
                        new SerializedDomain {Name = "Domain2"}
                    }
                };

                _workbook = new XLWorkbook();
                var worksheet = _workbook.Worksheets.Add("EntityDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Entity";

                worksheet.Cell("A2").Value = "Domain2";
                worksheet.Cell("C2").Value = "Entity1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "Domain1";
                worksheet.Cell("C3").Value = "Different Entity";
                worksheet.Cell("B3").Value = "pear";

                var openXmlToSerializelEntityMapper = new OpenXmlToSerializedEntityMapper(_serializedElementMapper, _auditor);
                openXmlToSerializelEntityMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_warning()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Warning);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Definition' on sheet 'EntityDefinitions'");
            }
        }

        [TestFixture]
        public class When_duplicate_entity_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedElementMapper _serializedElementMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedElementMapper = GenerateStub<IOpenXmlToSerializedElementMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[] { new SerializedDomain { Name = "My Domain name" } }
                };

                _workbook = new XLWorkbook();
                var worksheet = _workbook.Worksheets.Add("EntityDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("B1").Value = "Definition";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "Entity1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "My Domain name";
                worksheet.Cell("C3").Value = "Entity1";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "My Domain name";
                worksheet.Cell("C4").Value = "Entity2";
                worksheet.Cell("B4").Value = "pear";

                var openXmlToSerializelEntityMapper = new OpenXmlToSerializedEntityMapper(_serializedElementMapper, _auditor);
                openXmlToSerializelEntityMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_models_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Entities.Single().Name.ShouldEqual("Entity2");
            }

            [Test]
            public void Should_log_error_for_each_duplication()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate entity defined in row 2 on sheet 'EntityDefinitions'").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate entity defined in row 3 on sheet 'EntityDefinitions'").ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_missing_domain_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedElementMapper _serializedElementMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedElementMapper = GenerateStub<IOpenXmlToSerializedElementMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[] { new SerializedDomain { Name = "My Domain name" } }
                };
                _workbook = new XLWorkbook();
                var worksheet = _workbook.Worksheets.Add("EntityDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Entity";
                worksheet.Cell("B1").Value = "Definition";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "Entity2";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "Missing Domain";
                worksheet.Cell("C3").Value = "Entity1";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "Missing Domain";
                worksheet.Cell("C4").Value = "Entity1";
                worksheet.Cell("B4").Value = "orange";

                var openXmlToSerializelEntityMapper = new OpenXmlToSerializedEntityMapper(_serializedElementMapper, _auditor);
                openXmlToSerializelEntityMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_model_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Entities.Single().Name.ShouldEqual("Entity2");
            }

            [Test]
            public void Should_log_error_for_each_missing_domain()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 3 on sheet 'EntityDefinitions' due to undefined element group").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 4 on sheet 'EntityDefinitions' due to undefined element group").ShouldNotBeNull();
            }
        }
    }
}