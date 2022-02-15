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
    public class SerializedElementToSystemItemMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_element_to_system_item : TestBase
        {
            private SerializedElement _serializedElement;
            private SerializedElementCustomDetail _elementCustomDetail1, _elementCustomDetail2;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _entity;
            private ImportOptions _importOptions;
            private ISerializedToSystemItemCustomDetailMapper _systemItemCustomDetailMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _entity = New.SystemItem.AsEntity.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);

                _systemItemCustomDetailMapper = GenerateStub<ISerializedToSystemItemCustomDetailMapper>();

                _elementCustomDetail1 = new SerializedElementCustomDetail();
                _elementCustomDetail2 = new SerializedElementCustomDetail();
                _importOptions = new ImportOptions();

                _serializedElement = new SerializedElement
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C",
                    DataType = "My Datatype",
                    FieldLength = 50,
                    ItemUrl = "some url",
                    TechnicalName = "a different name",
                    CustomDetails = new[] {_elementCustomDetail1, _elementCustomDetail2}
                };

                var auditor = new Auditor();
                var mapper = new SerializedElementToSystemItemMapper(_systemItemCustomDetailMapper, auditor);
                mapper.Map(_serializedElement, _entity, _importOptions);
            }

            [Test]
            public void Should_create_new_entity_system_item_for_domain()
            {
                var element = _entity.ChildSystemItems.FirstOrDefault();
                element.ShouldNotBeNull();
                element.ItemName.ShouldEqual("ABC Data");
                element.Definition.ShouldEqual("Data about A, B, and C");
                element.DataTypeSource.ShouldEqual("My Datatype");
                element.FieldLength.ShouldEqual(50);
                element.ItemUrl.ShouldEqual("some url");
                element.TechnicalName.ShouldEqual("a different name");
                element.IsActive.ShouldBeTrue();
                element.MappedSystem.ShouldEqual(_mappedSystem);
                element.ParentSystemItem.ShouldEqual(_entity);
                element.ItemType.ShouldEqual(ItemType.Element);
            }

            [Test]
            public void Should_map_elements()
            {
                var element = _entity.ChildSystemItems.FirstOrDefault();
                _systemItemCustomDetailMapper.AssertWasCalled(x => x.Map(_elementCustomDetail1, element, _importOptions));
                _systemItemCustomDetailMapper.AssertWasCalled(x => x.Map(_elementCustomDetail2, element, _importOptions));
            }
        }

        [TestFixture]
        public class When_mapping_serialized_element_to_system_item_and_matching_on_name : TestBase
        {
            private SerializedElement _serializedElement;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _entity;
            private SystemItem _element;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _entity = New.SystemItem.AsEntity.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);
                _element = New.SystemItem.AsElement.WithMappedSystem(_mappedSystem).WithParentSystemItem(_entity).WithName("ABC Data");

                var systemItemCustomDetailMapper = GenerateStub<ISerializedToSystemItemCustomDetailMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedElement = new SerializedElement
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C"
                };

                var mapper = new SerializedElementToSystemItemMapper(systemItemCustomDetailMapper, new Auditor());
                mapper.Map(_serializedElement, _entity, _importOptions);
            }

            [Test]
            public void Should_not_create_new_system_item()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(3);
                _entity.ChildSystemItems.Count.ShouldEqual(1);
                _element.SystemItemId.ShouldNotEqual(Guid.Empty);
            }

            [Test]
            public void Should_update_matching_entity()
            {
                _element.ItemName.ShouldEqual("ABC Data");
                _element.Definition.ShouldEqual("Data about A, B, and C");
            }
        }

        [TestFixture]
        public class When_mapping_serialized_element_to_system_item_and_matching_on_name_with_no_match : TestBase
        {
            private SerializedElement _serializedElement;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _entity;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _entity = New.SystemItem.AsEntity.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);
                New.SystemItem.AsElement.WithMappedSystem(_mappedSystem).WithParentSystemItem(_entity).WithName("ABC Data");

                var systemItemCustomDetailMapper = GenerateStub<ISerializedToSystemItemCustomDetailMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedElement = new SerializedElement
                {
                    Name = "New ABC Data",
                    Definition = "Data about A, B, and C"
                };
                
                var mapper = new SerializedElementToSystemItemMapper(systemItemCustomDetailMapper, new Auditor());
                mapper.Map(_serializedElement, _entity, _importOptions);
            }

            [Test]
            public void Should_create_new_domain_system_item_for_mapped_system()
            {
                var element = _entity.ChildSystemItems.FirstOrDefault(x => x.ItemName == "New ABC Data" && x.ItemType == ItemType.Element);
                element.ShouldNotBeNull();
                element.ItemName.ShouldEqual("New ABC Data");
                element.Definition.ShouldEqual("Data about A, B, and C");
                element.MappedSystem.ShouldEqual(_mappedSystem);
                element.ParentSystemItem.ShouldEqual(_entity);
                element.ItemType.ShouldEqual(ItemType.Element);
            }

            [Test]
            public void Should_create_new_element()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(4);
                _entity.ChildSystemItems.Count(x => x.ItemType == ItemType.Element).ShouldEqual(2);
            }
        }
    }
}