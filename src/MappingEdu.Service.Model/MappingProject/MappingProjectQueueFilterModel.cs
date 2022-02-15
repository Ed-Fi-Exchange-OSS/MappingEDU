// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Service.Model.Datatables;

namespace MappingEdu.Service.Model.MappingProject
{
    public class MappingProjectQueueFilterBaseModel
    {
        public bool AutoMapped { get; set; }

        public bool Base { get; set; }

        public bool CreatedByColumn { get; set; }

        public bool CreationDateColumn { get; set; }

        public Guid[] CreatedByUserIds { get; set; }

        public Guid[] ElementGroups { get; set; }

        public bool Extended { get; set; }

        public bool Flagged { get; set; }

        public int[] ItemTypes { get; set; }

        public int Length { get; set; }

        public bool MappedByColumn { get; set; }

        public Guid MappingProjectId { get; set; }

        public int[] MappingMethods { get; set; }

        public string Name { get; set; }

        public int OrderColumn { get; set; }

        public string OrderDirection { get; set; }

        public string Search { get; set; }

        public bool ShowInDashboard { get; set; }

        public int[] Statuses { get; set; }

        public bool Unflagged { get; set; }

        public bool UpdatedByColumn { get; set; }

        public bool UpdateDateColumn { get; set; }

        public bool UserMapped { get; set; }

        public Guid[] UpdatedByUserIds { get; set; }

        public int[] WorkflowStatuses { get; set; }
    }

    public class MappingProjectQueueFilterCreateModel : MappingProjectQueueFilterBaseModel { }

    public class MappingProjectQueueFilterEditModel : MappingProjectQueueFilterBaseModel
    {
        public Guid MappingProjectQueueFilterId { get; set; }
    }

    public class MappingProjectQueueFilterViewModel : MappingProjectQueueFilterBaseModel
    {
        public Guid MappingProjectQueueFilterId { get; set; }
    }

    public class MappingProjectQueueFilterDashboardModel
    {
        public string Name { get; set; }

        public Guid MappingProjectQueueFilterId { get; set; }

        public int Total { get; set; }
    }
}