// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;

namespace MappingEdu.Service.Model.SystemItemMapping
{
    public class SystemItemMappingCreateModel
    {
        public string BusinessLogic { get; set; }

        public int? CompleteStatusTypeId { get; set; }

        public bool DeferredMapping { get; set; }

        public bool ExcludeInExternalReports { get; set; }

        public bool? Flagged { get; set; }

        public int MappingMethodTypeId { get; set; }

        public Guid MappingProjectId { get; set; }

        public int? MappingStatusReasonTypeId { get; set; }

        public int? MappingStatusTypeId { get; set; }

        public string OmissionReason { get; set; }

        public int WorkflowStatusTypeId { get; set; }

        public SystemItemMappingCreateModel()
        {
            MappingStatusTypeId = MappingStatusType.Unknown.Id;
            CompleteStatusTypeId = CompleteStatusType.Unknown.Id;
            MappingStatusReasonTypeId = MappingStatusReasonType.Unknown.Id;
            WorkflowStatusTypeId = WorkflowStatusType.Incomplete.Id;
        }
    }
}