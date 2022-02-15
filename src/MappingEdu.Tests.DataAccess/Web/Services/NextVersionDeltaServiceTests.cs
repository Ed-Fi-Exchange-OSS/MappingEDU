// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class NextVersionDeltaServiceTests
    {
        [TestFixture]
        public class When_creating_new_version_delta : EmptyDatabaseTestBase
        {
            private NextVersionDeltaViewModel _nextVersionDeltaViewModel;
            private Guid _elementId;
            private Guid _newElementId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "1.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Test Domain Name");
                var entity = CreateEntity(dbContext, domain, "Test Entity Name");
                var element = CreateElement(dbContext, entity, "New Test Element Name");
                _elementId = element.SystemItemId;

                var newMappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "2.0");
                var newDomain = CreateDomain(dbContext, newMappedSystem, "Test Domain Name");
                var newEntity = CreateEntity(dbContext, newDomain, "New Test Entity Name");
                var newElement = CreateElement(dbContext, newEntity, "New Test Element Name");
                _newElementId = newElement.SystemItemId;

                var version = new NextVersionDeltaCreateModel
                {
                    NewSystemItemId = _newElementId,
                    ItemChangeTypeId = ItemChangeType.ChangedElement.Id
                };

                var versionService = GetInstance<INextVersionDeltaService>();
                _nextVersionDeltaViewModel = versionService.Post(element.SystemItemId, version);
            }

            [Test]
            public void Should_create_version()
            {
                var dbContext = CreateDbContext();

                var version = dbContext.SystemItemVersionDeltas.SingleOrDefault(x => x.SystemItemVersionDeltaId == _nextVersionDeltaViewModel.SystemItemVersionDeltaId);

                version.ShouldNotBeNull();
                version.NewSystemItem.ShouldNotBeNull();
                version.NewSystemItemId.ShouldEqual(_newElementId);
                version.OldSystemItem.ShouldNotBeNull();
                version.OldSystemItemId.ShouldEqual(_elementId);
                version.ItemChangeTypeId.ShouldEqual(ItemChangeType.ChangedElement.Id);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _nextVersionDeltaViewModel.ShouldNotBeNull();
                _nextVersionDeltaViewModel.SystemItemVersionDeltaId.ShouldNotEqual(Guid.Empty);
                _nextVersionDeltaViewModel.ItemChangeType.ShouldEqual(ItemChangeType.ChangedElement);
                _nextVersionDeltaViewModel.NewSystemItem.ShouldNotBeNull();
                _nextVersionDeltaViewModel.NewSystemItem.ItemName.ShouldEqual("New Test Element Name");
                _nextVersionDeltaViewModel.NewDataStandard.SystemName.ShouldEqual("Test Mapped System");
                _nextVersionDeltaViewModel.NewDataStandard.SystemVersion.ShouldEqual("2.0");
                _nextVersionDeltaViewModel.NewSystemItem.ParentSystemItem.ItemName.ShouldEqual("New Test Entity Name");
            }
        }

        [TestFixture]
        public class When_updating_existing_version_delta : EmptyDatabaseTestBase
        {
            private NextVersionDeltaViewModel _nextVersionDeltaViewModel;
            private Guid _newElementId;
            private Guid _elementId;
            private Guid _versionDeltaId;

            protected override void EstablishContext()
            {
                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "1.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Test Domain Name");
                var entity = CreateEntity(dbContext, domain, "Test Entity Name");
                var element = CreateElement(dbContext, entity, "New Test Element Name");
                _elementId = element.SystemItemId;

                var newMappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "2.0");
                var newDomain = CreateDomain(dbContext, newMappedSystem, "Test Domain Name");
                var newEntity = CreateEntity(dbContext, newDomain, "New Test Entity Name");
                var newElement = CreateElement(dbContext, newEntity, "New Test Element Name");
                var anotherNewElement = CreateElement(dbContext, newEntity, "Another New Test Element Name");
                _newElementId = anotherNewElement.SystemItemId;

                var version = CreateVersion(dbContext, element, newElement, ItemChangeType.ChangedElement);
                _versionDeltaId = version.SystemItemVersionDeltaId;

                var versionEditModel = new NextVersionDeltaEditModel
                {
                    NewSystemItemId = _newElementId,
                    ItemChangeTypeId = ItemChangeType.ChangedEntity.Id
                };

                var versionService = GetInstance<INextVersionDeltaService>();
                _nextVersionDeltaViewModel = versionService.Put(_elementId, _versionDeltaId, versionEditModel);
            }

            [Test]
            public void Should_have_updated_data()
            {
                var dbContext = CreateDbContext();

                var version = dbContext.SystemItemVersionDeltas.Single(data => data.SystemItemVersionDeltaId == _versionDeltaId);

                version.ShouldNotBeNull();
                version.NewSystemItemId.ShouldEqual(_newElementId);
                version.NewSystemItem.ShouldNotBeNull();
                version.OldSystemItemId.ShouldEqual(_elementId);
                version.OldSystemItem.ShouldNotBeNull();
                version.ItemChangeTypeId.ShouldEqual(ItemChangeType.ChangedEntity.Id);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _nextVersionDeltaViewModel.ShouldNotBeNull();
                _nextVersionDeltaViewModel.SystemItemVersionDeltaId.ShouldNotEqual(Guid.Empty);
                _nextVersionDeltaViewModel.NewSystemItem.ItemName.ShouldEqual("Another New Test Element Name");
                _nextVersionDeltaViewModel.ItemChangeType.ShouldEqual(ItemChangeType.ChangedEntity);
            }
        }

        public class When_deleting_existing_version_delta : EmptyDatabaseTestBase
        {
            private Guid _elementId;
            private Guid _newElementId;
            private Guid _versionDeltaId;

            protected override void EstablishContext()
            {
                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "1.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Test Domain Name");
                var entity = CreateEntity(dbContext, domain, "Test Entity Name");
                var element = CreateElement(dbContext, entity, "New Test Element Name");
                _elementId = element.SystemItemId;

                var newMappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "2.0");
                var newDomain = CreateDomain(dbContext, newMappedSystem, "Test Domain Name");
                var newEntity = CreateEntity(dbContext, newDomain, "New Test Entity Name");
                var newElement = CreateElement(dbContext, newEntity, "New Test Element Name");
                _newElementId = newElement.SystemItemId;

                var version = CreateVersion(dbContext, element, newElement, ItemChangeType.ChangedElement);
                _versionDeltaId = version.SystemItemVersionDeltaId;

                var versionService = GetInstance<INextVersionDeltaService>();
                versionService.Delete(_elementId, version.SystemItemVersionDeltaId);
            }

            [Test]
            public void Should_have_no_version_records()
            {
                var dbContext = CreateDbContext();

                var element = dbContext.SystemItems.SingleOrDefault(x => x.SystemItemId == _elementId);
                element.ShouldNotBeNull();
                element.NewSystemItemVersionDeltas.Any().ShouldBeFalse();

                var newElement = dbContext.SystemItems.SingleOrDefault(x => x.SystemItemId == _newElementId);
                newElement.ShouldNotBeNull();
                newElement.OldSystemItemVersionDeltas.Count.ShouldEqual(0);

                var versionDelta = dbContext.SystemItemVersionDeltas.SingleOrDefault(x => x.SystemItemVersionDeltaId == _versionDeltaId);
                versionDelta.ShouldBeNull();
            }
        }
    }
}