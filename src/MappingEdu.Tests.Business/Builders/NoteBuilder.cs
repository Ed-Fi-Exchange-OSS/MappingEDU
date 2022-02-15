// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Tests.Business.Builders
{
    public class NoteBuilder
    {
        private readonly Note _note;

        public NoteBuilder(Note note)
        {
            _note = note;
        }

        public NoteBuilder()
        {
            _note = BuildDefault();
        }

        private static Note BuildDefault()
        {
            return new Note {NoteId = Guid.NewGuid()};
        }

        public NoteBuilder WithId(Guid id)
        {
            _note.NoteId = id;
            return this;
        }

        public NoteBuilder WithTitle(string title)
        {
            _note.Title = title;
            return this;
        }

        public NoteBuilder WithNotes(string notes)
        {
            _note.Notes = notes;
            return this;
        }

        public NoteBuilder WithCreateById(Guid createById)
        {
            _note.CreateById = createById;
            return this;
        }

        public NoteBuilder WithSystemItem(SystemItem systemItem)
        {
            _note.SystemItem = systemItem;
            _note.SystemItemId = systemItem.SystemItemId;
            return this;
        }

        public static implicit operator Note(NoteBuilder builder)
        {
            return builder._note;
        }
    }
}