// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.Entity;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.SystemItemCustomDetail;

namespace MappingEdu.Service.Model.SystemItem
{
    public class NextVersionViewModel
    {
        public string Description { get; set; }

        public ItemChangeType ItemChangeType { get; set; }

        public Guid? NewSystemItemId { get; set; }

        public Guid NextVersionId { get; set; }

        public List<PathSegment> NewSystemItemPathSegments { get; set; }
    }

    public class MappedSystemExtensionModel
    {
        public Guid MappedSystemExtensionId { get; set; }

        public string ShortName { get; set; }
    }

    public class PathSegment
    {
        public string Definition { get; set; }

        public bool IsExtended { get; set; }

        public MappedSystemExtensionModel Extension { get; set; }

        public string ItemName { get; set; }

        public int ItemTypeId { get; set; }

        public Guid SystemItemId { get; set; }
    }

    public class PreviousVersionViewModel
    {
        public string Description { get; set; }

        public ItemChangeType ItemChangeType { get; set; }

        public Guid? OldSystemItemId { get; set; }

        public Guid PreviousVersionId { get; set; }

        public List<PathSegment> OldSystemItemPathSegments { get; set; }
    }

    public class SystemItemViewModel : SystemItemCreateModel
    {
        public Guid SystemItemId { get; set; } 

        public string ExtensionShortName { get; set; }

        public List<PathSegment> PathSegments { get; set; }
        
        public List<SystemItemCustomDetailViewModel> SystemItemCustomDetails { get; set; } 
    }

    public class SystemItemViewWithSummaryModel : SystemItemViewModel
    {
        public MappingProjectSummaryViewModel Summary { get; set; }
    }

    public class SystemItemDetailViewModel : SystemItemViewModel
    {
        public List<SystemItemViewWithSummaryModel> ChildSystemItems { get; set; }

        public SystemItemViewModel EnumerationSystemItem { get; set; }

        public List<NextVersionViewModel> NextVersions { get; set; }

        public List<NoteViewModel> Notes { get; set; }

        public List<PreviousVersionViewModel> PreviousVersions { get; set; }
    }
}