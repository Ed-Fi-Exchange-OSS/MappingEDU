// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.EnumerationItemMapping;
using MappingEdu.Service.Model.MapNote;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Service.Model.SystemItemMapping
{
    public class SystemItemMappingViewModel
    {
        public string BusinessLogic { get; set; }

        public string CreateBy { get; set; }
        
        public Guid? CreateById { get; set; }

        public DateTime? CreateDate { get; set; }

        public CompleteStatusType CompleteStatusType { get; set; }

        public bool DeferredMapping { get; set; }

        public EnumerationItemMappingViewModel[] EnumerationItemMappings { get; set; }

        public bool ExcludeInExternalReports { get; set; }

        public bool? Flagged { get; set; }

        public bool IsAutoMapped { get; set; }

        public MapNoteViewModel[] MapNotes { get; set; }

        public MappingMethodType MappingMethodType { get; set; }

        public int MappingMethodTypeId { get; set; }

        public MappingProjectViewModel MappingProject { get; set; }

        public Guid MappingProjectId { get; set; }

        public MappingStatusReasonType MappingStatusReasonType { get; set; }

        public MappingStatusType MappingStatusType { get; set; }

        public string OmissionReason { get; set; }

        public Guid SourceSystemItemId { get; set; }

        public string StatusNote { get; set; }

        public Guid SystemItemMapId { get; set; }

        public TargetEnumerationItemViewModel[] TargetEnumerationItems { get; set; }

        public string UpdateBy { get; set; }

        public Guid? UpdateById { get; set; }

        public DateTime? UpdateDate { get; set; }

        public WorkflowStatusType WorkflowStatusType { get; set; }

        public int WorkflowStatusTypeId { get; set; }
    }
}