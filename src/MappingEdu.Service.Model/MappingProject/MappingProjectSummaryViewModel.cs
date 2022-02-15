// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.MappingProject
{
    public class MappingProjectSummaryViewModel
    {
        public string ItemName { get; set; }

        public Guid SystemItemId { get; set; }

        public int ItemTypeId { get; set; }

        public int Total { get; set; }

        public int Unmapped { get; set; }

        public int Incomplete { get; set; }

        public int Completed { get; set; }

        public int Reviewed { get; set; }

        public int Approved { get; set; }

        public int Mapped { get; set; }

        public int Extended { get; set; }

        public int Omitted { get; set; }

        public int Percent { get; set; }
    }

    public class MappingProjectSummaryDetailViewModel
    {
        public int Total { get; set; }
        
        public int MappingMethodTypeId { get; set; }

        public int WorkflowStatusTypeId { get; set; }
    }
}