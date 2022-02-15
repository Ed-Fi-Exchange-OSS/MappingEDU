// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class EntityServiceTests
    {
        [TestFixture]
        public class When_deleting_an_entity : EmptyDatabaseTestBase
        {
            private Guid _entityId;
            private Guid _subEntityId;
            private Guid _elementId;
            private Guid _noteId;
            private Guid _nextDeltaId;
            private Guid _previousDeltaId;
            private Guid _mappingId;
            private Guid _itemCustomDetailId;
            private Guid _entityCustomDetailId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                var otherMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(
                    dbContext, "Project Name", "Map This to That", mappedSystem, otherMappedSystem);
                var domain = CreateDomain(dbContext, mappedSystem, "Domain Name", "Domain Definition");
                var entity = CreateEntity(dbContext, domain, "Entity Name", "Entity Definition");
                var subentity = CreateEntity(dbContext, entity, "SubEntity Name", "SubEntity Def");
                var element = CreateElement(dbContext, subentity, "Element Name", "Element Definition");
                var note = CreateNote(dbContext, entity, "Title", "Text");
                var nextEntity = CreateEntity(dbContext, domain, "Next Entity", "Next Entity Definition");
                var nextVersionDelta = CreateVersion(dbContext, entity, nextEntity, ItemChangeType.ChangedEntity);
                var previousEntity = CreateEntity(dbContext, domain, "Previous Entity", "Previous Entity Definition");
                var previousVersionDelta = CreateVersion(dbContext, previousEntity, entity, ItemChangeType.ChangedEntity);
                var mapping = CreateSystemItemMap(dbContext, mappingProject, element);
                var customDetailMetadata = CreateCustomDetailMetadata(dbContext, mappedSystem, "Custom Detail", false, true);
                var itemCustomDetail = CreateSystemItemCustomDetail(dbContext, element, customDetailMetadata, "Custom Detail Value");
                var entityCustomDetail = CreateSystemItemCustomDetail(dbContext, entity, customDetailMetadata, "Entity Custom Detail Value");

                _entityId = entity.SystemItemId;
                _subEntityId = subentity.SystemItemId;
                _elementId = element.SystemItemId;
                _noteId = note.NoteId;
                _nextDeltaId = nextVersionDelta.SystemItemVersionDeltaId;
                _previousDeltaId = previousVersionDelta.SystemItemVersionDeltaId;
                _mappingId = mapping.SystemItemMapId;
                _itemCustomDetailId = itemCustomDetail.SystemItemCustomDetailId;
                _entityCustomDetailId = entityCustomDetail.SystemItemCustomDetailId;

                var newDbContext = CreateDbContext();
                var foundEntity = newDbContext.SystemItems.Find(_entityId);
                foundEntity.ShouldNotBeNull();
                var foundSubEntity = newDbContext.SystemItems.Find(_subEntityId);
                foundSubEntity.ShouldNotBeNull();
                var foundElement = newDbContext.SystemItems.Find(_elementId);
                foundElement.ShouldNotBeNull();
                var foundNote = newDbContext.Notes.Find(_noteId);
                foundNote.ShouldNotBeNull();
                var foundNextDelta = newDbContext.SystemItemVersionDeltas.Find(_nextDeltaId);
                foundNextDelta.ShouldNotBeNull();
                var foundPreviousDelta = newDbContext.SystemItemVersionDeltas.Find(_previousDeltaId);
                foundPreviousDelta.ShouldNotBeNull();
                var foundMapping = newDbContext.SystemItemMaps.Find(_mappingId);
                foundMapping.ShouldNotBeNull();
                var foundItemCustomDetail = newDbContext.SystemItemCustomDetails.Find(_itemCustomDetailId);
                foundItemCustomDetail.ShouldNotBeNull();
                var foundEntityCustomDetail = newDbContext.SystemItemCustomDetails.Find(_entityCustomDetailId);
                foundEntityCustomDetail.ShouldNotBeNull();

                var entityService = GetInstance<IEntityService>();
                entityService.Delete(entity.SystemItemId);
            }

            [Test]
            public void Should_delete_the_entity_and_related_data()
            {
                var dbContext = CreateDbContext();
                var entity = dbContext.SystemItems.Find(_entityId);
                entity.ShouldBeNull();
                var subEntity = dbContext.SystemItems.Find(_subEntityId);
                subEntity.ShouldBeNull();
                var element = dbContext.SystemItems.Find(_elementId);
                element.ShouldBeNull();
                var note = dbContext.Notes.Find(_noteId);
                note.ShouldBeNull();
                var nextDelta = dbContext.SystemItemVersionDeltas.Find(_nextDeltaId);
                nextDelta.ShouldBeNull();
                var previousDelta = dbContext.SystemItemVersionDeltas.Find(_previousDeltaId);
                previousDelta.ShouldBeNull();
                var mapping = dbContext.SystemItemMaps.Find(_mappingId);
                mapping.ShouldBeNull();
                var itemCustomDetail = dbContext.SystemItemCustomDetails.Find(_itemCustomDetailId);
                itemCustomDetail.ShouldBeNull();
                var entityCustomDetail = dbContext.SystemItemCustomDetails.Find(_entityCustomDetailId);
                entityCustomDetail.ShouldBeNull();
            }
        }
    }
}