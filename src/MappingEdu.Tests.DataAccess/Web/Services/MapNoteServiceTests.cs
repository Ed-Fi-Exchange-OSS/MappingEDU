// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Service.Model.MapNote;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class MapNoteServiceTests
    {
        [TestFixture]
        public class When_adding_map_note : EmptyDatabaseTestBase
        {
            private const string mapNoteTitle = "Note Title";
            private const string mapNoteNotes = "This is the text of the note.";
            private MapNoteViewModel _mapNoteViewModel;
            private Guid _systemItemMapId;

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
                var sourceSystemItem = CreateElement(dbContext, entity, "Element Name", "Element Definition");
                var systemItemMap = CreateSystemItemMap(dbContext, mappingProject, sourceSystemItem);
                _systemItemMapId = systemItemMap.SystemItemMapId;

                var model = new MapNoteCreateModel
                {
                    Title = mapNoteTitle,
                    Notes = mapNoteNotes
                };

                var mapNoteService = GetInstance<IMapNoteService>();
                _mapNoteViewModel = mapNoteService.Post(_systemItemMapId, model);
            }

            [Test]
            public void Should_create_a_new_map_note()
            {
                var dbContext = CreateDbContext();
                var systemItemMap = dbContext.SystemItemMaps.Find(_systemItemMapId);
                var mapNote = systemItemMap.MapNotes.Single();
                mapNote.MapNoteId.ShouldNotEqual(Guid.Empty);
                mapNote.Title.ShouldEqual(mapNoteTitle);
                mapNote.Notes.ShouldEqual(mapNoteNotes);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _mapNoteViewModel.ShouldNotBeNull();
                _mapNoteViewModel.MapNoteId.ShouldNotEqual(Guid.Empty);
                _mapNoteViewModel.Title.ShouldEqual(mapNoteTitle);
                _mapNoteViewModel.Notes.ShouldEqual(mapNoteNotes);
            }
        }

        [TestFixture]
        public class When_updating_map_note : EmptyDatabaseTestBase
        {
            private const string mapNoteTitle = "Note Title";
            private const string mapNoteNotes = "This is the text of the note.";
            private const string mapNoteTitleUpdated = "Updated Note Title";
            private const string mapNoteNotesUpdated = "This is the text of the note. With additional text.";
            private MapNoteViewModel _mapNoteViewModel;
            private Guid _systemItemMapId;
            private Guid _mapNoteId;

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
                var sourceSystemItem = CreateElement(dbContext, entity, "Element Name", "Element Definition");
                var systemItemMap = CreateSystemItemMap(dbContext, mappingProject, sourceSystemItem);
                var mapNote = CreateMapNote(dbContext, systemItemMap, mapNoteTitle, mapNoteNotes);

                _systemItemMapId = systemItemMap.SystemItemMapId;
                _mapNoteId = mapNote.MapNoteId;

                var model = new MapNoteEditModel
                {
                    Title = mapNoteTitleUpdated,
                    Notes = mapNoteNotesUpdated
                };

                var mapNoteService = GetInstance<IMapNoteService>();
                _mapNoteViewModel = mapNoteService.Put(_systemItemMapId, _mapNoteId, model);
            }

            [Test]
            public void Should_create_a_new_map_note()
            {
                var dbContext = CreateDbContext();
                var mapNote = dbContext.MapNotes.Find(_mapNoteId);
                mapNote.MapNoteId.ShouldNotEqual(Guid.Empty);
                mapNote.Title.ShouldEqual(mapNoteTitleUpdated);
                mapNote.Notes.ShouldEqual(mapNoteNotesUpdated);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _mapNoteViewModel.ShouldNotBeNull();
                _mapNoteViewModel.MapNoteId.ShouldNotEqual(Guid.Empty);
                _mapNoteViewModel.Title.ShouldEqual(mapNoteTitleUpdated);
                _mapNoteViewModel.Notes.ShouldEqual(mapNoteNotesUpdated);
            }
        }

        [TestFixture]
        public class When_deleting_map_note : EmptyDatabaseTestBase
        {
            private const string mapNoteTitle = "Note Title";
            private const string mapNoteNotes = "This is the text of the note.";
            private Guid _systemItemMapId;
            private Guid _mapNoteId;

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
                var sourceSystemItem = CreateElement(dbContext, entity, "Element Name", "Element Definition");
                var systemItemMap = CreateSystemItemMap(dbContext, mappingProject, sourceSystemItem);
                var mapNote = CreateMapNote(dbContext, systemItemMap, mapNoteTitle, mapNoteNotes);

                _systemItemMapId = systemItemMap.SystemItemMapId;
                _mapNoteId = mapNote.MapNoteId;

                var mapNoteService = GetInstance<IMapNoteService>();
                mapNoteService.Delete(_systemItemMapId, _mapNoteId);
            }

            [Test]
            public void Should_delete_the_map_note()
            {
                var dbContext = CreateDbContext();
                var mapNote = dbContext.MapNotes.Find(_mapNoteId);
                mapNote.ShouldBeNull();
            }
        }
    }
}