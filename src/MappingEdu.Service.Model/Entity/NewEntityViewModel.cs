// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.Model.SystemItemMapping;

namespace MappingEdu.Service.Model.Entity
{
    public class NewEntityViewModel
    {
        public string Definition { get; set; }

        public NewElementViewModel[] Elements { get; set; }

        public bool IsExtended { get; set; }

        public string ItemName { get; set; }

        public NextVersionViewModel[] NextVersions { get; set; }

        public NoteViewModel[] Notes { get; set; }

        public ElementListViewModel.ElementPathViewModel.PathSegment[] PathSegments { get; set; }

        public PreviousVersionViewModel[] PreviousVersions { get; set; }

        public Guid SystemItemId { get; set; }
    }

    public class NewElementViewModel
    {
        public ElementDetailsViewModel ElementDetails { get; set; }

        public WorkflowStatusType MappingStatus { get; set; }

        public NextVersionViewModel[] NextVersions { get; set; }

        public NoteViewModel[] Notes { get; set; }

        public PreviousVersionViewModel[] PreviousVersions { get; set; }

        public SystemItemCustomDetailsGroupViewModel SystemItemCustomDetailsContainer { get; set; }

        public Guid SystemItemId { get; set; }

        public SystemItemMappingViewModel[] SystemItemMappings { get; set; }
    }

    public class PreviousVersionViewModel
    {
        public string Description { get; set; }

        public ItemChangeType ItemChangeType { get; set; }

        public Guid? OldSystemItemId { get; set; }

        public Guid PreviousVersionId { get; set; }

        public ElementListViewModel.ElementPathViewModel.PathSegment[] PreviousVersionItems { get; set; }
    }

    public class NextVersionViewModel
    {
        public string Description { get; set; }

        public ItemChangeType ItemChangeType { get; set; }

        public Guid? NewSystemItemId { get; set; }

        public Guid NextVersionId { get; set; }

        public ElementListViewModel.ElementPathViewModel.PathSegment[] NextVersionItems { get; set; }
    }
}