// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Service.Model.BriefElement;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.Model.SystemItemDefinition;
using MappingEdu.Service.Model.SystemItemName;

namespace MappingEdu.Service.Model.Entity
{
    public class EntityViewModel
    {
        public BriefElementViewModel[] BriefElements { get; set; }

        public NextVersionDeltaViewModel[] NextVersionDeltas { get; set; }

        public NoteViewModel[] Notes { get; set; }

        public PreviousVersionDeltaViewModel[] PreviousVersionDeltas { get; set; }

        public SystemItemDefinitionViewModel SystemItemDefinition { get; set; }

        public Guid SystemItemId { get; set; }

        public SystemItemNameViewModel SystemItemName { get; set; }
    }
}