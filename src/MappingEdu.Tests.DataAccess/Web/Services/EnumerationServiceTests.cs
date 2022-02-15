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
    public class EnumerationServiceTests
    {
        [TestFixture]
        public class When_deleting_an_enumeration : EmptyDatabaseTestBase
        {
            private Guid _entityId;
            private Guid _enumerationId;
            private Guid _enumerationItemId;
            private Guid _noteId;
            private Guid _nextDeltaId;
            private Guid _previousDeltaId;
            private Guid _mappingId;
            private Guid _itemMappingId;

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
                var enumeration = CreateEnumeration(dbContext, entity, "Enumeration Name", "Enumeration Definition");
                var enumerationItem = CreateEnumerationItem(dbContext, enumeration, "CODE", "Description", "Short Description");
                var note = CreateNote(dbContext, enumeration, "Title", "Text");
                var nextEnumeration = CreateElement(dbContext, entity, "Next Element", "Next Element Definition");
                var nextVersionDelta = CreateVersion(dbContext, enumeration, nextEnumeration, ItemChangeType.ChangedElement);
                var previousEnumeration = CreateElement(dbContext, entity, "Previous Element", "Previous Element Definition");
                var previousVersionDelta = CreateVersion(dbContext, previousEnumeration, enumeration, ItemChangeType.ChangedElement);
                var mapping = CreateSystemItemMap(dbContext, mappingProject, enumeration);
                var itemMapping = CreatEnumerationItemMap(dbContext, mapping, enumerationItem);

                _entityId = entity.SystemItemId;
                _enumerationId = enumeration.SystemItemId;
                _enumerationItemId = enumerationItem.SystemEnumerationItemId;
                _noteId = note.NoteId;
                _nextDeltaId = nextVersionDelta.SystemItemVersionDeltaId;
                _previousDeltaId = previousVersionDelta.SystemItemVersionDeltaId;
                _mappingId = mapping.SystemItemMapId;
                _itemMappingId = itemMapping.SystemEnumerationItemMapId;

                var newDbContext = CreateDbContext();
                var foundEntity = newDbContext.SystemItems.Find(_entityId);
                foundEntity.ShouldNotBeNull();
                var foundEnumeration = newDbContext.SystemItems.Find(_enumerationId);
                foundEnumeration.ShouldNotBeNull();
                var foundEnumerationItem = newDbContext.SystemEnumerationItems.Find(_enumerationItemId);
                foundEnumerationItem.ShouldNotBeNull();
                var foundNote = newDbContext.Notes.Find(_noteId);
                foundNote.ShouldNotBeNull();
                var foundNextDelta = newDbContext.SystemItemVersionDeltas.Find(_nextDeltaId);
                foundNextDelta.ShouldNotBeNull();
                var foundPreviousDelta = newDbContext.SystemItemVersionDeltas.Find(_previousDeltaId);
                foundPreviousDelta.ShouldNotBeNull();
                var foundMapping = newDbContext.SystemItemMaps.Find(_mappingId);
                foundMapping.ShouldNotBeNull();
                var foundItemMapping = newDbContext.SystemEnumerationItemMaps.Find(_itemMappingId);
                foundItemMapping.ShouldNotBeNull();

                var enumerationService = GetInstance<IEnumerationService>();
                enumerationService.Delete(_enumerationId);
            }

            [Test]
            public void Should_delete_the_enumeration_and_related_data()
            {
                var dbContext = CreateDbContext();
                var entity = dbContext.SystemItems.Find(_entityId);
                entity.ShouldNotBeNull();
                var enumeration = dbContext.SystemItems.Find(_enumerationId);
                enumeration.ShouldBeNull();
                var enumerationItem = dbContext.SystemEnumerationItems.Find(_enumerationItemId);
                enumerationItem.ShouldBeNull();
                var note = dbContext.Notes.Find(_noteId);
                note.ShouldBeNull();
                var nextDelta = dbContext.SystemItemVersionDeltas.Find(_nextDeltaId);
                nextDelta.ShouldBeNull();
                var previousDelta = dbContext.SystemItemVersionDeltas.Find(_previousDeltaId);
                previousDelta.ShouldBeNull();
                var mapping = dbContext.SystemItemMaps.Find(_mappingId);
                mapping.ShouldBeNull();
                var itemMapping = dbContext.SystemEnumerationItemMaps.Find(_itemMappingId);
                itemMapping.ShouldBeNull();
            }
        }
    }
}