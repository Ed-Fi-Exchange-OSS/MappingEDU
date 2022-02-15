// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Service.Model.MappingProject
{
    public class MappingProjectDashboardViewModel
    {
        public MappingGrouping[] ElementGroups { get; set; }

        public Guid MappingProjectId { get; set; }

        public MappingGrouping[] Statuses { get; set; }

        public MappingGrouping[] WorkQueue { get; set; }

        public class MappingGrouping
        {
            public int Count { get; set; }

            public string Filter { get; set; }

            public string GroupName { get; set; }
        }
    }
}