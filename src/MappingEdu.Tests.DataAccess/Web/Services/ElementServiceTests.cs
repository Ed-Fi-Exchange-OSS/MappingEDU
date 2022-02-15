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
    public class ElementServiceTests
    {
        [TestFixture]
        public class When_deleting_an_element : EmptyDatabaseTestBase
        {
            private Guid _entityId;
            private Guid _elementId;
            private Guid _mappingId;
            private Guid _noteId;
            private Guid _nextDeltaId;
            private Guid _previousDeltaId;
            private Guid _itemCustomDetailId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                var otherMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(dbContext, "Project Name", "Map This to That", mappedSystem, otherMappedSystem);
                var domain = CreateDomain(dbContext, mappedSystem, "Domain Name", "Domain Definition");
                var entity = CreateEntity(dbContext, domain, "Entity Name", "Entity Definition");
                var element = CreateElement(dbContext, entity, "Element Name", "Element Definition");
                var mapping = CreateSystemItemMap(dbContext, mappingProject, element);
                var note = CreateNote(dbContext, element, "Title", "Text");
                var nextElement = CreateElement(dbContext, entity, "Next Element", "Next Element Definition");
                var nextVersionDelta = CreateVersion(dbContext, element, nextElement, ItemChangeType.ChangedElement);
                var previousElement = CreateElement(dbContext, entity, "Previous Element", "Previous Element Definition");
                var previousVersionDelta = CreateVersion(dbContext, previousElement, element, ItemChangeType.ChangedElement);
                var customDetailMetadata = CreateCustomDetailMetadata(dbContext, mappedSystem, "Custom Detail", false, true);
                var itemCustomDetail = CreateSystemItemCustomDetail(dbContext, element, customDetailMetadata, "Custom Detail Value");

                _entityId = entity.SystemItemId;
                _elementId = element.SystemItemId;
                _mappingId = mapping.SystemItemMapId;
                _noteId = note.NoteId;
                _nextDeltaId = nextVersionDelta.SystemItemVersionDeltaId;
                _previousDeltaId = previousVersionDelta.SystemItemVersionDeltaId;
                _itemCustomDetailId = itemCustomDetail.SystemItemCustomDetailId;

                var newDbContext = CreateDbContext();
                var foundEntity = newDbContext.SystemItems.Find(_entityId);
                foundEntity.ShouldNotBeNull();
                var foundElement = newDbContext.SystemItems.Find(_elementId);
                foundElement.ShouldNotBeNull();
                var foundMapping = newDbContext.SystemItemMaps.Find(_mappingId);
                foundMapping.ShouldNotBeNull();
                var foundNote = newDbContext.Notes.Find(_noteId);
                foundNote.ShouldNotBeNull();
                var foundNextDelta = newDbContext.SystemItemVersionDeltas.Find(_nextDeltaId);
                foundNextDelta.ShouldNotBeNull();
                var foundPreviousDelta = newDbContext.SystemItemVersionDeltas.Find(_previousDeltaId);
                foundPreviousDelta.ShouldNotBeNull();
                var foundItemCustomDetail = newDbContext.SystemItemCustomDetails.Find(_itemCustomDetailId);
                foundItemCustomDetail.ShouldNotBeNull();

                var elementService = GetInstance<IElementService>();
                elementService.Delete(_elementId);
            }

            [Test]
            public void Should_delete_the_element_and_related_data()
            {
                var dbContext = CreateDbContext();
                var entity = dbContext.SystemItems.Find(_entityId);
                entity.ShouldNotBeNull();
                var element = dbContext.SystemItems.Find(_elementId);
                element.ShouldBeNull();
                var mapping = dbContext.SystemItemMaps.Find(_mappingId);
                mapping.ShouldBeNull();
                var note = dbContext.Notes.Find(_noteId);
                note.ShouldBeNull();
                var nextDelta = dbContext.SystemItemVersionDeltas.Find(_nextDeltaId);
                nextDelta.ShouldBeNull();
                var previousDelta = dbContext.SystemItemVersionDeltas.Find(_previousDeltaId);
                previousDelta.ShouldBeNull();
                var itemCustomDetail = dbContext.SystemItemCustomDetails.Find(_itemCustomDetailId);
                itemCustomDetail.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_deleting_an_element_that_is_a_mapping_target : EmptyDatabaseTestBase
        {
            private Guid _entityId;
            private Guid _elementId;
            private bool _exceptionThrown;
            private Exception _exception;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "v 1.0");
                var domain = CreateDomain(dbContext, mappedSystem, "domain name");
                var entity = CreateEntity(dbContext, domain, "Test Entity Name");
                var element = CreateElement(dbContext, entity, "Test Element Name");

                var mappingSystem = CreateMappedSystem(dbContext, "Mapping System", "v 1.1");
                var otherMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(
                    dbContext, "Project Name", "Map This to That", mappedSystem, otherMappedSystem);
                var mappingDomain = CreateDomain(dbContext, mappingSystem, "Mapping Domain", "Mapping Domain Def");
                var mappingEntity = CreateEntity(dbContext, mappingDomain, "Mapping Entity", "Mapping Entity Def");
                var mappingElement = CreateElement(dbContext, mappingEntity, "Mapping Element Name", "Mapping Element Def");
                var mapping = CreateSystemItemMap(dbContext, mappingProject, mappingElement);
                mapping.TargetSystemItems.Add(element);
                dbContext.SaveChanges();

                var anotherMappingElement = CreateElement(dbContext, mappingEntity, "Another Mapping Element Name", "Another Mapping Element Def");
                var anotherMapping = CreateSystemItemMap(dbContext, mappingProject, anotherMappingElement);
                anotherMapping.TargetSystemItems.Add(element);
                dbContext.SaveChanges();

                _entityId = entity.SystemItemId;
                _elementId = element.SystemItemId;

                var elementService = GetInstance<IElementService>();

                try
                {
                    elementService.Delete(_elementId);
                }
                catch (Exception ex)
                {
                    _exceptionThrown = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(
                    "Cannot delete this item because it or a child element is mapped by [Mapping System.Mapping Domain.Mapping Entity.Mapping Element Name] and [Mapping System.Mapping Domain.Mapping Entity.Another Mapping Element Name].");
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }
    }
}