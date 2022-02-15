// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Tests.Business.Builders
{
    public class MapNoteBuilder
    {
        private readonly MapNote _mapNote;

        public MapNoteBuilder(MapNote mapNote)
        {
            _mapNote = mapNote;
        }

        public MapNoteBuilder()
        {
            _mapNote = BuildDefault();
        }

        private static MapNote BuildDefault()
        {
            return new MapNote {MapNoteId = Guid.NewGuid()};
        }

        public MapNoteBuilder WithId(Guid id)
        {
            _mapNote.MapNoteId = id;
            return this;
        }

        public MapNoteBuilder WithTitle(string title)
        {
            _mapNote.Title = title;
            return this;
        }

        public MapNoteBuilder WithNotes(string notes)
        {
            _mapNote.Notes = notes;
            return this;
        }

        public MapNoteBuilder WithCreateById(Guid createById)
        {
            _mapNote.CreateById = createById;
            return this;
        }

        public MapNoteBuilder WithSystemItemMap(SystemItemMap systemItemMap)
        {
            _mapNote.SystemItemMap = systemItemMap;
            _mapNote.SystemItemMapId = systemItemMap.SystemItemMapId;
            systemItemMap.MapNotes.Add(this);
            return this;
        }

        public static implicit operator MapNote(MapNoteBuilder builder)
        {
            return builder._mapNote;
        }
    }
}