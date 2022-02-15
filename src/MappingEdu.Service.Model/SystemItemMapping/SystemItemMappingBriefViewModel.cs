// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.SystemItemMapping
{
    public class SystemItemMappingBriefViewModel
    {
        public string BusinessLogic { get; set; }

        public string CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? CompleteStatusTypeId { get; set; }

        public bool DeferredMapping { get; set; }

        public bool ExcludeInExternalReports { get; set; }

        public bool? Flagged { get; set; }

        public bool IsAutoMapped { get; set; }

        public int MappingMethodTypeId { get; set; }

        public Guid MappingProjectId { get; set; }

        public int? MappingStatusReasonTypeId { get; set; }

        public int? MappingStatusTypeId { get; set; }

        public string OmissionReason { get; set; }

        public Guid SourceSystemItemId { get; set; }

        public string StatusNote { get; set; }

        public Guid SystemItemMapId { get; set; }

        public string UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int WorkflowStatusTypeId { get; set; }
    }
}