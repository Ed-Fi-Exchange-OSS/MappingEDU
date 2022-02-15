// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.EnumerationItem;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.Model.SystemItemDefinition;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.Model.SystemItemName;

namespace MappingEdu.Service.Model.Enumeration
{
    public class EnumerationViewModel
    {
        public ElementDetailsViewModel[] ElementsOfEnumerationType { get; set; }

        public EnumerationItemViewModel[] EnumerationItems { get; set; }

        public NextVersionDeltaViewModel[] NextVersionDeltas { get; set; }

        public NoteViewModel[] Notes { get; set; }

        public PreviousVersionDeltaViewModel[] PreviousVersionDeltas { get; set; }

        public SystemItemDefinitionViewModel SystemItemDefinition { get; set; }

        public Guid SystemItemId { get; set; }

        public SystemItemMappingViewModel[] SystemItemMappings { get; set; }

        public SystemItemNameViewModel SystemItemName { get; set; }
    }
}