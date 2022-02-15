// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class NoteServiceTests
    {
        [TestFixture]
        public class When_adding_note : EmptyDatabaseTestBase
        {
            private Guid _entityId;
            private NoteViewModel _note1;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "v 1.0");
                var domain = CreateDomain(dbContext, mappedSystem, "domain name", string.Empty, string.Empty);
                var entity = CreateEntity(dbContext, domain, "Test Entity Name", string.Empty);
                _entityId = entity.SystemItemId;

                var noteCreate1 = new NoteCreateModel
                {
                    Title = "Test Note Title 1",
                    Notes = "Test Notes 1"
                };

                var noteService = GetInstance<INoteService>();

                _note1 = noteService.Post(_entityId, noteCreate1);
            }

            [Test]
            public void Should_be_persisted_to_database()
            {
                var dbContext = CreateDbContext();
                var entity = dbContext.SystemItems.Single(data => data.SystemItemId == _entityId);
                entity.ShouldNotBeNull();
                entity.Notes.Count.ShouldEqual(1);

                var note = entity.Notes.First();
                note.Title.ShouldEqual("Test Note Title 1");
                note.Notes.ShouldEqual("Test Notes 1");
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _note1.ShouldNotBeNull();
                _note1.NoteId.ShouldNotEqual(Guid.Empty);
                _note1.Title.ShouldEqual("Test Note Title 1");
                _note1.Notes.ShouldEqual("Test Notes 1");
            }
        }

        [TestFixture]
        public class When_updating_note : EmptyDatabaseTestBase
        {
            private const string noteTitle = "Note Title";
            private const string noteNotes = "This is the text of the note.";
            private const string noteTitleUpdated = "Updated Note Title";
            private const string noteNotesUpdated = "This is the text of the note. With additional text.";
            private NoteViewModel _noteViewModel;
            private Guid _systemItemId;
            private Guid _noteId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Domain Name", "Domain Definition");
                var entity = CreateEntity(dbContext, domain, "Entity Name", "Entity Definition");
                var element = CreateElement(dbContext, entity, "Element Name", "Element Definition");
                var note = CreateNote(dbContext, element, noteTitle, noteNotes);

                _systemItemId = element.SystemItemId;
                _noteId = note.NoteId;

                var model = new NoteEditModel
                {
                    Title = noteTitleUpdated,
                    Notes = noteNotesUpdated
                };

                var noteService = GetInstance<INoteService>();
                _noteViewModel = noteService.Put(_systemItemId, _noteId, model);
            }

            [Test]
            public void Should_create_a_new_map_note()
            {
                var dbContext = CreateDbContext();
                var note = dbContext.Notes.Find(_noteId);
                note.NoteId.ShouldNotEqual(Guid.Empty);
                note.Title.ShouldEqual(noteTitleUpdated);
                note.Notes.ShouldEqual(noteNotesUpdated);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _noteViewModel.ShouldNotBeNull();
                _noteViewModel.NoteId.ShouldNotEqual(Guid.Empty);
                _noteViewModel.Title.ShouldEqual(noteTitleUpdated);
                _noteViewModel.Notes.ShouldEqual(noteNotesUpdated);
            }
        }

        [TestFixture]
        public class When_deleting_note : EmptyDatabaseTestBase
        {
            private Guid _entityId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "Test Mapped System", "v 1.0");
                var domain = CreateDomain(dbContext, mappedSystem, "domain name", string.Empty, string.Empty);
                var entity = CreateEntity(dbContext, domain, "Test Entity Name", string.Empty);
                _entityId = entity.SystemItemId;
                var note1 = CreateNote(dbContext, entity, "Test Note Title 1", "Test Notes 1");
                CreateNote(dbContext, entity, "Test Note Title 2", "Test Notes 2");

                var noteService = GetInstance<INoteService>();
                noteService.Delete(_entityId, note1.NoteId);
            }

            [Test]
            public void Note_should_have_been_deleted()
            {
                var dbContext = CreateDbContext();
                var notes = dbContext.Notes.Where(data => data.SystemItemId == _entityId).ToList();

                notes.ShouldNotBeNull();
                notes.Count.ShouldEqual(1);
                notes[0].Title.ShouldEqual("Test Note Title 2");
                notes[0].Notes.ShouldEqual("Test Notes 2");
            }
        }
    }
}