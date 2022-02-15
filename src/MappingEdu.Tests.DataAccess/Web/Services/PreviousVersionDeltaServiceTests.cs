// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class PreviousVersionDeltaServiceTests
    {
        [TestFixture]
        public class When_creating_previous_version_delta : EmptyDatabaseTestBase
        {
            private PreviousVersionDeltaViewModel _previousVersionDeltaViewModel;
            private Guid _elementId;
            private Guid _oldElementId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "2.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Test Domain Name");
                var entity = CreateEntity(dbContext, domain, "Test Entity Name");
                var element = CreateElement(dbContext, entity, "Test Element Name");
                _elementId = element.SystemItemId;

                var oldMappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "1.0");
                var oldDomain = CreateDomain(dbContext, oldMappedSystem, "Test Domain Name");
                var oldEntity = CreateEntity(dbContext, oldDomain, "Old Test Entity Name");
                var oldElement = CreateElement(dbContext, oldEntity, "Old Test Element Name");
                _oldElementId = oldElement.SystemItemId;

                var version = new PreviousVersionDeltaCreateModel
                {
                    OldSystemItemId = _oldElementId,
                    ItemChangeTypeId = ItemChangeType.ChangedElement.Id
                };

                var versionService = GetInstance<IPreviousVersionDeltaService>();
                _previousVersionDeltaViewModel = versionService.Post(element.SystemItemId, version);
            }

            [Test]
            public void Should_create_version()
            {
                var dbContext = CreateDbContext();

                var version = dbContext.SystemItemVersionDeltas.SingleOrDefault(x => x.SystemItemVersionDeltaId == _previousVersionDeltaViewModel.SystemItemVersionDeltaId);

                version.ShouldNotBeNull();
                version.OldSystemItem.ShouldNotBeNull();
                version.OldSystemItemId.ShouldEqual(_oldElementId);
                version.NewSystemItem.ShouldNotBeNull();
                version.NewSystemItemId.ShouldEqual(_elementId);
                version.ItemChangeTypeId.ShouldEqual(ItemChangeType.ChangedElement.Id);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _previousVersionDeltaViewModel.ShouldNotBeNull();
                _previousVersionDeltaViewModel.SystemItemVersionDeltaId.ShouldNotEqual(Guid.Empty);
                _previousVersionDeltaViewModel.ItemChangeType.ShouldEqual(ItemChangeType.ChangedElement);
                _previousVersionDeltaViewModel.OldSystemItem.ShouldNotBeNull();
                _previousVersionDeltaViewModel.OldSystemItem.ItemName.ShouldEqual("Old Test Element Name");
                _previousVersionDeltaViewModel.OldDataStandard.SystemName.ShouldEqual("Test Mapped System");
                _previousVersionDeltaViewModel.OldDataStandard.SystemVersion.ShouldEqual("1.0");
                _previousVersionDeltaViewModel.OldSystemItem.ParentSystemItem.ItemName.ShouldEqual("Old Test Entity Name");
            }
        }

        [TestFixture]
        public class When_updating_existing_version_delta : EmptyDatabaseTestBase
        {
            private PreviousVersionDeltaViewModel _previousVersionDeltaViewModel;
            private Guid _oldElementId;
            private Guid _elementId;
            private Guid _versionDeltaId;

            protected override void EstablishContext()
            {
                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "2.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Test Domain Name");
                var entity = CreateEntity(dbContext, domain, "Test Entity Name");
                var element = CreateElement(dbContext, entity, "Old Test Element Name");
                _elementId = element.SystemItemId;

                var oldMappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "1.0");
                var oldDomain = CreateDomain(dbContext, oldMappedSystem, "Test Domain Name");
                var oldEntity = CreateEntity(dbContext, oldDomain, "Old Test Entity Name");
                var oldElement = CreateElement(dbContext, oldEntity, "Old Test Element Name");
                var anotherOldElement = CreateElement(dbContext, oldEntity, "Another Old Test Element Name");
                _oldElementId = anotherOldElement.SystemItemId;

                var version = CreateVersion(dbContext, oldElement, element, ItemChangeType.ChangedElement);
                _versionDeltaId = version.SystemItemVersionDeltaId;

                var versionEditModel = new PreviousVersionDeltaEditModel
                {
                    OldSystemItemId = _oldElementId,
                    ItemChangeTypeId = ItemChangeType.ChangedEntity.Id
                };

                var versionService = GetInstance<IPreviousVersionDeltaService>();
                _previousVersionDeltaViewModel = versionService.Put(_elementId, _versionDeltaId, versionEditModel);
            }

            [Test]
            public void Should_have_updated_data()
            {
                var dbContext = CreateDbContext();

                var version = dbContext.SystemItemVersionDeltas.Single(data => data.SystemItemVersionDeltaId == _versionDeltaId);

                version.ShouldNotBeNull();
                version.OldSystemItemId.ShouldEqual(_oldElementId);
                version.OldSystemItem.ShouldNotBeNull();
                version.NewSystemItemId.ShouldEqual(_elementId);
                version.NewSystemItem.ShouldNotBeNull();
                version.ItemChangeTypeId.ShouldEqual(ItemChangeType.ChangedEntity.Id);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _previousVersionDeltaViewModel.ShouldNotBeNull();
                _previousVersionDeltaViewModel.SystemItemVersionDeltaId.ShouldNotEqual(Guid.Empty);
                _previousVersionDeltaViewModel.OldSystemItem.ItemName.ShouldEqual("Another Old Test Element Name");
                _previousVersionDeltaViewModel.ItemChangeType.ShouldEqual(ItemChangeType.ChangedEntity);
            }
        }

        public class When_deleting_existing_version_delta : EmptyDatabaseTestBase
        {
            private Guid _elementId;
            private Guid _oldElementId;
            private Guid _versionDeltaId;

            protected override void EstablishContext()
            {
                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "1.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Test Domain Name");
                var entity = CreateEntity(dbContext, domain, "Test Entity Name");
                var element = CreateElement(dbContext, entity, "Old Test Element Name");
                _elementId = element.SystemItemId;

                var oldMappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "2.0");
                var oldDomain = CreateDomain(dbContext, oldMappedSystem, "Test Domain Name");
                var oldEntity = CreateEntity(dbContext, oldDomain, "Old Test Entity Name");
                var oldElement = CreateElement(dbContext, oldEntity, "Old Test Element Name");
                _oldElementId = oldElement.SystemItemId;

                var version = CreateVersion(dbContext, oldElement, element, ItemChangeType.ChangedElement);
                _versionDeltaId = version.SystemItemVersionDeltaId;

                var versionService = GetInstance<IPreviousVersionDeltaService>();
                versionService.Delete(_elementId, _versionDeltaId);
            }

            [Test]
            public void Should_have_no_version_records()
            {
                var dbContext = CreateDbContext();

                var element = dbContext.SystemItems.SingleOrDefault(x => x.SystemItemId == _elementId);
                element.ShouldNotBeNull();
                element.OldSystemItemVersionDeltas.Any().ShouldBeFalse();

                var oldElement = dbContext.SystemItems.SingleOrDefault(x => x.SystemItemId == _oldElementId);
                oldElement.ShouldNotBeNull();
                oldElement.OldSystemItemVersionDeltas.Count.ShouldEqual(0);

                var versionDelta = dbContext.SystemItemVersionDeltas.SingleOrDefault(x => x.SystemItemVersionDeltaId == _versionDeltaId);
                versionDelta.ShouldBeNull();
            }
        }
    }
}