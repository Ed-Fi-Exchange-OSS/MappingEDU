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
    public class SerializedEnumerationToSystemItemMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_enumeration_to_system_item : TestBase
        {
            private SerializedEnumeration _serializedEnumeration;
            private SerializedEnumerationValue _serializedEnumerationValue1, _serializedEnumerationValue2;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private ImportOptions _importOptions;
            private ISerializedToSystemItemEnumerationMapper _enumerationValueMapper;
            private ISerializedToSystemItemCustomDetailMapper _customDetailMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);

                _enumerationValueMapper = GenerateStub<ISerializedToSystemItemEnumerationMapper>();
                _customDetailMapper = GenerateStub<ISerializedToSystemItemCustomDetailMapper>();

                _serializedEnumerationValue1 = new SerializedEnumerationValue();
                _serializedEnumerationValue2 = new SerializedEnumerationValue();
                _importOptions = new ImportOptions();

                _serializedEnumeration = new SerializedEnumeration
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C",
                    EnumerationValues = new[] {_serializedEnumerationValue1, _serializedEnumerationValue2}
                };

                var mapper = new SerializedEnumerationToSystemItemMapper(_enumerationValueMapper, _customDetailMapper, new Auditor());
                mapper.Map(_serializedEnumeration, _domain, _importOptions);
            }

            [Test]
            public void Should_create_new_enumeration_system_item_for_domain()
            {
                var enumeration = _domain.ChildSystemItems.FirstOrDefault();
                enumeration.ShouldNotBeNull();
                enumeration.ItemName.ShouldEqual("ABC Data");
                enumeration.Definition.ShouldEqual("Data about A, B, and C");
                enumeration.IsActive.ShouldBeTrue();
                enumeration.MappedSystem.ShouldEqual(_mappedSystem);
                enumeration.ParentSystemItem.ShouldEqual(_domain);
                enumeration.ItemType.ShouldEqual(ItemType.Enumeration);
            }

            [Test]
            public void Should_map_elements()
            {
                var enumeration = _domain.ChildSystemItems.FirstOrDefault();
                _enumerationValueMapper.AssertWasCalled(x => x.Map(_serializedEnumerationValue1, enumeration, _importOptions));
                _enumerationValueMapper.AssertWasCalled(x => x.Map(_serializedEnumerationValue2, enumeration, _importOptions));
            }
        }

        [TestFixture]
        public class When_mapping_serialized_enumeration_to_system_item_and_matching_on_name : TestBase
        {
            private SerializedEnumeration _serializedEnumeration;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _enumeration;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _enumeration = New.SystemItem.AsEnumeration.WithParentSystemItem(_domain).WithMappedSystem(_mappedSystem).WithName("ABC Data");

                var enumerationValueMapper = GenerateStub<ISerializedToSystemItemEnumerationMapper>();
                var customDetailMapper = GenerateStub<ISerializedToSystemItemCustomDetailMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedEnumeration = new SerializedEnumeration
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C"
                };

                var mapper = new SerializedEnumerationToSystemItemMapper(enumerationValueMapper, customDetailMapper, new Auditor());
                mapper.Map(_serializedEnumeration, _domain, _importOptions);
            }

            [Test]
            public void Should_not_create_new_system_item()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(2);
                _domain.ChildSystemItems.Count.ShouldEqual(1);
                _enumeration.SystemItemId.ShouldNotEqual(Guid.Empty);
            }

            [Test]
            public void Should_update_matching_entity()
            {
                _enumeration.ItemName.ShouldEqual("ABC Data");
                _enumeration.Definition.ShouldEqual("Data about A, B, and C");
            }
        }

        [TestFixture]
        public class When_mapping_serialized_enumeration_to_system_item_and_matching_on_name_with_no_match : TestBase
        {
            private SerializedEnumeration _serializedEnumeration;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                New.SystemItem.AsEnumeration.WithParentSystemItem(_domain).WithMappedSystem(_mappedSystem).WithName("ABC Data");

                var enumerationValueMapper = GenerateStub<ISerializedToSystemItemEnumerationMapper>();
                var customDetailMapper = GenerateStub<ISerializedToSystemItemCustomDetailMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedEnumeration = new SerializedEnumeration
                {
                    Name = "New ABC Data",
                    Definition = "Data about A, B, and C"
                };

                var mapper = new SerializedEnumerationToSystemItemMapper(enumerationValueMapper, customDetailMapper, new Auditor());
                mapper.Map(_serializedEnumeration, _domain, _importOptions);
            }

            [Test]
            public void Should_create_new_entity_system_item_for_mapped_system()
            {
                var enumeration = _domain.ChildSystemItems.FirstOrDefault(x => x.ItemName == "New ABC Data" && x.ItemType == ItemType.Enumeration);
                enumeration.ShouldNotBeNull();
                enumeration.ItemName.ShouldEqual("New ABC Data");
                enumeration.Definition.ShouldEqual("Data about A, B, and C");
                enumeration.MappedSystem.ShouldEqual(_mappedSystem);
                enumeration.ParentSystemItem.ShouldEqual(_domain);
                enumeration.ItemType.ShouldEqual(ItemType.Enumeration);
            }

            [Test]
            public void Should_create_new_enumeration()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(3);
                _domain.ChildSystemItems.Count(x => x.ItemType == ItemType.Enumeration).ShouldEqual(2);
            }
        }
    }
}