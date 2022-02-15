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
    public class SerializedEntityToSystemItemMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_entity_to_system_item : TestBase
        {
            private SerializedEntity _serializedEntity;
            private SerializedElement _serializedElement1, _serializedElement2;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private ImportOptions _importOptions;
            private ISerializedElementToSystemItemMapper _elemementMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);

                _elemementMapper = GenerateStub<ISerializedElementToSystemItemMapper>();

                _serializedElement1 = new SerializedElement();
                _serializedElement2 = new SerializedElement();
                _importOptions = new ImportOptions();

                _serializedEntity = new SerializedEntity
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C",
                    Elements = new[] {_serializedElement1, _serializedElement2}
                };

                var mapper = new SerializedEntityToSystemItemMapper(_elemementMapper, new Auditor());
                mapper.Map(_serializedEntity, _domain, _importOptions);
            }

            [Test]
            public void Should_create_new_entity_system_item_for_domain()
            {
                var entity = _domain.ChildSystemItems.FirstOrDefault();
                entity.ShouldNotBeNull();
                entity.ItemName.ShouldEqual("ABC Data");
                entity.Definition.ShouldEqual("Data about A, B, and C");
                entity.IsActive.ShouldBeTrue();
                entity.MappedSystem.ShouldEqual(_mappedSystem);
                entity.ParentSystemItem.ShouldEqual(_domain);
                entity.ItemType.ShouldEqual(ItemType.Entity);
            }

            [Test]
            public void Should_map_elements()
            {
                var entity = _domain.ChildSystemItems.FirstOrDefault();
                _elemementMapper.AssertWasCalled(x => x.Map(_serializedElement1, entity, _importOptions));
                _elemementMapper.AssertWasCalled(x => x.Map(_serializedElement2, entity, _importOptions));
            }
        }

        [TestFixture]
        public class When_mapping_serialized_entity_to_system_item_and_matching_on_name : TestBase
        {
            private SerializedEntity _serializedEntity;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _entity;
            private ImportOptions _importOptions;
            private ISerializedElementToSystemItemMapper _elemementMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _entity = New.SystemItem.AsEntity.WithParentSystemItem(_domain).WithMappedSystem(_mappedSystem).WithName("ABC Data");

                _elemementMapper = GenerateStub<ISerializedElementToSystemItemMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedEntity = new SerializedEntity
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C"
                };

                var mapper = new SerializedEntityToSystemItemMapper(_elemementMapper, new Auditor());
                mapper.Map(_serializedEntity, _domain, _importOptions);
            }

            [Test]
            public void Should_not_create_new_system_item()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(2);
                _domain.ChildSystemItems.Count.ShouldEqual(1);
                _entity.SystemItemId.ShouldNotEqual(Guid.Empty);
            }

            [Test]
            public void Should_update_matching_entity()
            {
                _entity.ItemName.ShouldEqual("ABC Data");
                _entity.Definition.ShouldEqual("Data about A, B, and C");
            }
        }

        [TestFixture]
        public class When_mapping_serialized_entity_to_system_item_and_matching_on_name_with_no_match : TestBase
        {
            private SerializedEntity _serializedEntity;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _entity;
            private ImportOptions _importOptions;
            private ISerializedElementToSystemItemMapper _elemementMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _entity = New.SystemItem.AsEntity.WithParentSystemItem(_domain).WithMappedSystem(_mappedSystem).WithName("ABC Data");

                _elemementMapper = GenerateStub<ISerializedElementToSystemItemMapper>();

                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedEntity = new SerializedEntity
                {
                    Name = "New ABC Data",
                    Definition = "Data about A, B, and C"
                };

                var mapper = new SerializedEntityToSystemItemMapper(_elemementMapper, new Auditor());
                mapper.Map(_serializedEntity, _domain, _importOptions);
            }

            [Test]
            public void Should_create_new_domain()
            {
                _mappedSystem.SystemItems.Count.ShouldEqual(3);
                _domain.ChildSystemItems.Count(x => x.ItemType == ItemType.Entity).ShouldEqual(2);
            }

            [Test]
            public void Should_create_new_entity_system_item_for_mapped_system()
            {
                var entity = _domain.ChildSystemItems.FirstOrDefault(x => x.ItemName == "New ABC Data" && x.ItemType == ItemType.Entity);
                entity.ShouldNotBeNull();
                entity.ItemName.ShouldEqual("New ABC Data");
                entity.Definition.ShouldEqual("Data about A, B, and C");
                entity.MappedSystem.ShouldEqual(_mappedSystem);
                entity.ParentSystemItem.ShouldEqual(_domain);
                entity.ItemType.ShouldEqual(ItemType.Entity);
            }
        }

        [TestFixture]
        public class When_mapping_serialized_entity_with_subentities_to_system_item : TestBase
        {
            private SerializedEntity _serializedEntity;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private ImportOptions _importOptions;
            private ISerializedElementToSystemItemMapper _elemementMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);

                _elemementMapper = GenerateStub<ISerializedElementToSystemItemMapper>();

                _importOptions = new ImportOptions();

                _serializedEntity = new SerializedEntity
                {
                    Name = "ABC Data",
                    Definition = "Data about A, B, and C",
                    SubEntities = new[]
                    {
                        new SerializedEntity
                        {
                            Name = "Subentity1",
                            Definition = "some other stuff"
                        }
                    }
                };

                var mapper = new SerializedEntityToSystemItemMapper(_elemementMapper, new Auditor());
                mapper.Map(_serializedEntity, _domain, _importOptions);
            }

            [Test]
            public void Should_create_new_entity_system_item_for_subentities()
            {
                var entity = _domain.ChildSystemItems.FirstOrDefault();
                var subentity = entity.ChildSystemItems.FirstOrDefault(x => x.ItemName == "Subentity1");
                subentity.ShouldNotBeNull();
                subentity.ItemName.ShouldEqual("Subentity1");
                subentity.Definition.ShouldEqual("some other stuff");
                subentity.MappedSystem.ShouldEqual(_mappedSystem);
                subentity.ParentSystemItem.ShouldEqual(entity);
                subentity.ItemType.ShouldEqual(ItemType.SubEntity);
            }
        }
    }
}