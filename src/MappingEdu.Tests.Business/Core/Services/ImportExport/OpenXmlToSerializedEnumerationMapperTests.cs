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
    public class OpenXmlToSerializedEnumerationMapperTests
    {
        [TestFixture]
        public class When_mapping_enumerations : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEnumerationValueMapper _serializedEnumerationValueMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEnumerationValueMapper = GenerateStub<IOpenXmlToSerializedEnumerationValueMapper>();
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

                var worksheet = _workbook.Worksheets.Add("EnumerationDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("B1").Value = "Definition";

                worksheet.Cell("A2").Value = "Domain2";
                worksheet.Cell("C2").Value = "Enumeration1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "Domain1";
                worksheet.Cell("C3").Value = "Different Enumeration";
                worksheet.Cell("B3").Value = "pear";

                var openXmlToSerializelEnumerationMapper = new OpenXmlToSerializedEnumerationMapper(_serializedEnumerationValueMapper, _auditor);
                openXmlToSerializelEnumerationMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_serialized_enumeration_models()
            {
                var domain1 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain1");
                var enumeration = domain1.Enumerations[0];
                enumeration.Name.ShouldEqual("Different Enumeration");
                enumeration.Definition.ShouldEqual("pear");

                var domain2 = _serializedMappedSystem.Domains.Single(x => x.Name == "Domain2");
                enumeration = domain2.Enumerations[0];
                enumeration.Name.ShouldEqual("Enumeration1");
                enumeration.Definition.ShouldEqual("apple");
            }

            [Test]
            public void Should_create_serialized_enumeration_values()
            {
                _serializedEnumerationValueMapper.AssertWasCalled(x => x.Map(_workbook, _serializedMappedSystem, _options));
            }

            [Test]
            public void Should_not_log_any_errors()
            {
                _auditor.GetAll().Count().ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_enumeration_sheet : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEnumerationValueMapper _serializedEnumerationValueMapper;
            private ImportOptions _options;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEnumerationValueMapper = GenerateStub<IOpenXmlToSerializedEnumerationValueMapper>();
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

                var openXmlToSerializelEnumerationMapper = new OpenXmlToSerializedEnumerationMapper(_serializedEnumerationValueMapper, _auditor);
                openXmlToSerializelEnumerationMapper.Map(new XLWorkbook(), _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Info);
                _auditor.GetAll().Single().Message.ShouldEqual("Not importing enumerations because sheet 'EnumerationDefinitions' could not be found");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.Count(x => x.Enumerations != null).ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_domain_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEnumerationValueMapper _serializedEnumerationValueMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEnumerationValueMapper = GenerateStub<IOpenXmlToSerializedEnumerationValueMapper>();
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

                var worksheet = _workbook.Worksheets.Add("EnumerationDefinitions");
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("B1").Value = "Definition";

                var openXmlToSerializelEnumerationMapper = new OpenXmlToSerializedEnumerationMapper(_serializedEnumerationValueMapper, _auditor);
                openXmlToSerializelEnumerationMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Element Group' on sheet 'EnumerationDefinitions'");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.Count(x => x.Enumerations != null).ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_missing_enumeration_column : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEnumerationValueMapper _serializedEnumerationValueMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEnumerationValueMapper = GenerateStub<IOpenXmlToSerializedEnumerationValueMapper>();
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

                var worksheet = _workbook.Worksheets.Add("EnumerationDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("B1").Value = "Definition";

                var openXmlToSerializelEnumerationMapper = new OpenXmlToSerializedEnumerationMapper(_serializedEnumerationValueMapper, _auditor);
                openXmlToSerializelEnumerationMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_log_error()
            {
                _auditor.GetAll().Single().AuditLevel.ShouldEqual(AuditLevel.Error);
                _auditor.GetAll().Single().Message.ShouldEqual("Missing column 'Enumeration' on sheet 'EnumerationDefinitions'");
            }

            [Test]
            public void Should_not_create_any_models()
            {
                _serializedMappedSystem.Domains.Count(x => x.Enumerations != null).ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_duplicate_enumeration_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEnumerationValueMapper _serializedEnumerationValueMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEnumerationValueMapper = GenerateStub<IOpenXmlToSerializedEnumerationValueMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[] {new SerializedDomain {Name = "My Domain name"}}
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("B1").Value = "Definition";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "enumeration1";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "My Domain name";
                worksheet.Cell("C3").Value = "enumeration1";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "My Domain name";
                worksheet.Cell("C4").Value = "enumeration2";
                worksheet.Cell("B4").Value = "pear";

                var openXmlToSerializelEnumerationMapper = new OpenXmlToSerializedEnumerationMapper(_serializedEnumerationValueMapper, _auditor);
                openXmlToSerializelEnumerationMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_models_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Enumerations.Single().Name.ShouldEqual("enumeration2");
            }

            [Test]
            public void Should_log_error_for_each_duplication()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate enumeration defined in row 2 on sheet 'EnumerationDefinitions'").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Duplicate enumeration defined in row 3 on sheet 'EnumerationDefinitions'").ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_missing_domain_found_in_data : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private IAuditor _auditor;
            private IOpenXmlToSerializedEnumerationValueMapper _serializedEnumerationValueMapper;
            private ImportOptions _options;
            private XLWorkbook _workbook;

            [OneTimeSetUp]
            public void Setup()
            {
                _serializedEnumerationValueMapper = GenerateStub<IOpenXmlToSerializedEnumerationValueMapper>();
                _options = new ImportOptions();
                _auditor = new Auditor();
                _serializedMappedSystem = new SerializedMappedSystem
                {
                    Domains = new[] {new SerializedDomain {Name = "My Domain name"}}
                };
                _workbook = new XLWorkbook();

                var worksheet = _workbook.Worksheets.Add("EnumerationDefinitions");
                worksheet.Cell("A1").Value = "Element Group";
                worksheet.Cell("C1").Value = "Enumeration";
                worksheet.Cell("B1").Value = "Definition";

                worksheet.Cell("A2").Value = "My Domain name";
                worksheet.Cell("C2").Value = "enumeration2";
                worksheet.Cell("B2").Value = "apple";
                worksheet.Cell("A3").Value = "Missing Domain";
                worksheet.Cell("C3").Value = "enumeration1";
                worksheet.Cell("B3").Value = "orange";
                worksheet.Cell("A4").Value = "Missing Domain";
                worksheet.Cell("C4").Value = "enumeration1";
                worksheet.Cell("B4").Value = "orange";

                var openXmlToSerializelEnumerationMapper = new OpenXmlToSerializedEnumerationMapper(_serializedEnumerationValueMapper, _auditor);
                openXmlToSerializelEnumerationMapper.Map(_workbook, _serializedMappedSystem, _options);
            }

            [Test]
            public void Should_create_model_for_correct_data()
            {
                _serializedMappedSystem.Domains.Single().Enumerations.Single().Name.ShouldEqual("enumeration2");
            }

            [Test]
            public void Should_log_error_for_each_missing_domain()
            {
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 3 on sheet 'EnumerationDefinitions' due to undefined element group").ShouldNotBeNull();
                _auditor.GetAll().SingleOrDefault(x => x.Message == "Skipping row 4 on sheet 'EnumerationDefinitions' due to undefined element group").ShouldNotBeNull();
            }
        }
    }
}