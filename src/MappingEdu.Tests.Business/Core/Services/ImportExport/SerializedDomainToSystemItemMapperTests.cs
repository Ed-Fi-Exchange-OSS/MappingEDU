// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Auditing;
using MappingEdu.Core.Services.Import;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    public class SerializedDomainToSystemItemMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_domain_to_system_item : TestBase
        {
            private SerializedDomain _serializedDomain;
            private SerializedEntity _serializedEntity1, _serializedEntity2;
            private SerializedEnumeration _serializedEnumeration1, _serializedEnumeration2;
            private MappedSystem _mappedSystem;
            private ImportOptions _importOptions;
            private IAuditor _auditor;
            private ISerializedEntityToSystemItemMapper _entityMapper;
            private ISerializedEnumerationToSystemItemMapper _enumerationMapper;
            private ISerializedElementToEnumerationTypeMapper _elementEnumerationMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;

                _entityMapper = GenerateStub<ISerializedEntityToSystemItemMapper>();
                _enumerationMapper = GenerateStub<ISerializedEnumerationToSystemItemMapper>();
                _elementEnumerationMapper = GenerateStub<ISerializedElementToEnumerationTypeMapper>();

                _serializedEntity1 = new SerializedEntity();
                _serializedEntity2 = new SerializedEntity();
                _serializedEnumeration1 = new SerializedEnumeration();
                _serializedEnumeration2 = new SerializedEnumeration();
                _importOptions = new ImportOptions();
                _auditor = new Auditor();

                _serializedDomain = new SerializedDomain
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C",
                    Entities = new[] {_serializedEntity1, _serializedEntity2},
                    Enumerations = new[] {_serializedEnumeration1, _serializedEnumeration2}
                };

                var mapper = new SerializedDomainToSystemItemMapper(_entityMapper, _enumerationMapper, _elementEnumerationMapper, _auditor);
                mapper.Map(_serializedDomain, _mappedSystem, _importOptions);
            }

            [Test]
            public void Should_create_new_domain_system_item_for_mapped_system()
            {
                var domain = _mappedSystem.SystemItems.FirstOrDefault();
                domain.ShouldNotBeNull();
                domain.ItemName.ShouldEqual("ABC Data");
                domain.Definition.ShouldEqual("Data about A, B, and C");
                domain.IsActive.ShouldBeTrue();
                domain.MappedSystem.ShouldEqual(_mappedSystem);
                domain.ItemType.ShouldEqual(ItemType.Domain);
            }

            [Test]
            public void Should_map_entities()
            {
                var domain = _mappedSystem.SystemItems.FirstOrDefault();
                _entityMapper.AssertWasCalled(x => x.Map(_serializedEntity1, domain, _importOptions));
                _entityMapper.AssertWasCalled(x => x.Map(_serializedEntity2, domain, _importOptions));
            }

            [Test]
            public void Should_map_enumerations()
            {
                var domain = _mappedSystem.SystemItems.FirstOrDefault();
                _enumerationMapper.AssertWasCalled(x => x.Map(_serializedEnumeration1, domain, _importOptions));
                _enumerationMapper.AssertWasCalled(x => x.Map(_serializedEnumeration2, domain, _importOptions));
            }
        }

        [TestFixture]
        public class When_mapping_serialized_domain_to_system_item_and_matching_on_name : TestBase
        {
            private SerializedDomain _serializedDomain;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain
                    .WithId(Guid.NewGuid())
                    .WithMappedSystem(_mappedSystem)
                    .WithName("ABC Data")
                    .WithDefinition("original definition");

                var entityMapper = GenerateStub<ISerializedEntityToSystemItemMapper>();
                var enumerationMapper = GenerateStub<ISerializedEnumerationToSystemItemMapper>();
                var elementEnumerationMapper = GenerateStub<ISerializedElementToEnumerationTypeMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedDomain = new SerializedDomain
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C"
                };

                var mapper = new SerializedDomainToSystemItemMapper(entityMapper, enumerationMapper, elementEnumerationMapper, new Auditor());
                mapper.Map(_serializedDomain, _mappedSystem, _importOptions);
            }

            [Test]
            public void Should_not_create_new_domain()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(1);
                _domain.SystemItemId.ShouldNotEqual(Guid.Empty);
            }

            [Test]
            public void Should_update_matching_domain()
            {
                _domain.ItemName.ShouldEqual("ABC Data");
                _domain.Definition.ShouldEqual("Data about A, B, and C");
            }
        }

        [TestFixture]
        public class When_mapping_serialized_domain_to_system_item_and_matching_on_name_with_no_match : TestBase
        {
            private SerializedDomain _serializedDomain;
            private MappedSystem _mappedSystem;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                var domain = New.SystemItem.AsDomain
                    .WithMappedSystem(_mappedSystem)
                    .WithName("ABC Data");
                New.SystemItem.AsEntity
                    .WithMappedSystem(_mappedSystem)
                    .WithParentSystemItem(domain)
                    .WithName("New ABC Data");

                var entityMapper = GenerateStub<ISerializedEntityToSystemItemMapper>();
                var enumerationMapper = GenerateStub<ISerializedEnumerationToSystemItemMapper>();
                var elementEnumerationMapper = GenerateStub<ISerializedElementToEnumerationTypeMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedDomain = new SerializedDomain
                {
                    Name = "New ABC Data",
                    Definition = "Data about A, B, and C"
                };

                var mapper = new SerializedDomainToSystemItemMapper(entityMapper, enumerationMapper, elementEnumerationMapper, new Auditor());
                mapper.Map(_serializedDomain, _mappedSystem, _importOptions);
            }

            [Test]
            public void Should_create_new_domain()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(3);
                _mappedSystem.SystemItems.Count(x => x.ItemType == ItemType.Domain).ShouldEqual(2);
            }

            [Test]
            public void Should_create_new_domain_system_item_for_mapped_system()
            {
                var domain = _mappedSystem.SystemItems.FirstOrDefault(x => x.ItemName == "New ABC Data" && x.ItemType == ItemType.Domain);
                domain.ShouldNotBeNull();
                domain.ItemName.ShouldEqual("New ABC Data");
                domain.Definition.ShouldEqual("Data about A, B, and C");
                domain.MappedSystem.ShouldEqual(_mappedSystem);
                domain.ItemType.ShouldEqual(ItemType.Domain);
            }
        }
    }
}