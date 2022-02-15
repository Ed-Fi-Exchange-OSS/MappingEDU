// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.Model.SystemItemMapping;

namespace MappingEdu.Service.Model.MappingProject
{
    public class MappingProjectReviewQueueViewModel
    {
        public Guid MappingProjectId { get; set; }

        public ReviewItemViewModel[] ReviewItems { get; set; }

        public class ReviewItemViewModel : ElementListViewModel.ElementPathViewModel
        {
            public SystemItemMappingBriefViewModel Mapping { get; set; }

            public int MappedEnumerations { get; set; }

            public int TotalEnumerations { get; set; }
        }
    }

    public class ReviewQueueDatatablesModel : DatatablesModel
    {
        public Guid[] ElementGroups { get; set; }
        public Guid[] CreatedByUserIds { get; set; }
        public Guid[] UpdatedByUserIds { get; set; }
        public int[] ItemTypes { get; set; }
        public int[] MappingMethods { get; set; }
        public int[] WorkflowStatuses { get; set; }
        public bool Flagged { get; set; }
        public bool Unflagged { get; set; }
        public bool Extended { get; set; }
        public bool Base { get; set; }
        public bool AutoMapped { get; set; }
        public bool UserMapped { get; set; }
    }
}