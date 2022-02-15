// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class NoteServiceTests
    {
        [TestFixture]
        public class When_getting_specific_note : TestBase
        {
            private NoteViewModel _result;
            private readonly NoteViewModel _expected = new NoteViewModel();
            private readonly Guid _noteId = Guid.NewGuid();
            private readonly Guid _systemItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem systemItem = New.SystemItem.WithId(_systemItemId);
                Note retunedNote = New.Note.WithId(_noteId).WithSystemItem(systemItem);

                var noteRepository = GenerateStub<IRepository<Note>>();
                noteRepository.Stub(x => x.Get(_noteId)).Return(retunedNote);

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<NoteViewModel>(retunedNote)).Return(_expected);

                var noteService = new NoteService(noteRepository, null, mapper);
                _result = noteService.Get(_systemItemId, _noteId);
            }

            [Test]
            public void Should_populate_note_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_notes_for_system_item : TestBase
        {
            private NoteViewModel[] _result;
            private NoteViewModel[] _expected;
            private readonly Guid _entityId = Guid.NewGuid();
            private readonly Guid _note1Id = Guid.NewGuid();
            private readonly Guid _note2Id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                _expected = new[]
                {
                    new NoteViewModel {CreateDate = new DateTime(2014, 1, 2), NoteId = _note1Id},
                    new NoteViewModel {CreateDate = new DateTime(2013, 4, 2), NoteId = _note2Id}
                };

                SystemItem entity = New.SystemItem.AsEntity.WithId(_entityId);
                New.Note.WithId(_note1Id).WithSystemItem(entity);
                New.Note.WithId(_note2Id).WithSystemItem(entity);

                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Expect(x => x.Get(_entityId)).Return(entity);

                var mapper = GenerateStub<IMapper>();
                mapper.Expect(x => x.Map<NoteViewModel[]>(entity.Notes)).Return(_expected);

                var noteService = new NoteService(null, systemItemRepository, mapper);
                _result = noteService.Get(_entityId);
            }

            [Test]
            public void Should_populate_note_view_model()
            {
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(2);
                _result[0].NoteId.ShouldEqual(_note2Id);
                _result[1].NoteId.ShouldEqual(_note1Id);
            }
        }

        [TestFixture]
        public class When_getting_invalid_note : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _invalidNoteId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var noteRepository = GenerateStub<IRepository<Note>>();
                noteRepository.Stub(x => x.Get(_invalidNoteId)).Return(null);

                var noteService = new NoteService(noteRepository, null, null);

                try
                {
                    noteService.Get(Guid.NewGuid(), _invalidNoteId);
                }
                catch (Exception exception)
                {
                    _hasException = true;
                    _exception = exception;
                }
            }

            [Test]
            public void Should_have_meaningful_error_message()
            {
                _exception.Message.ShouldEqual("The note with id '" + _invalidNoteId + "' does not exist.");
            }

            [Test]
            public void Should_throw_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_notes_for_non_existant_system_item : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_systemItemId)).Return(null);

                var noteService = new NoteService(null, systemItemRepository, null);

                try
                {
                    noteService.Get(_systemItemId);
                }
                catch (Exception exception)
                {
                    _hasException = true;
                    _exception = exception;
                }
            }

            [Test]
            public void Should_have_meaningful_error_message()
            {
                _exception.Message.ShouldEqual("The system item with id '" + _systemItemId + "' does not exist.");
            }

            [Test]
            public void Should_throw_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }
    }
}