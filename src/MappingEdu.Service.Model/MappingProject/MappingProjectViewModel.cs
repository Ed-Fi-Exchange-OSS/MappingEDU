// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Service.Model.DataStandard;

namespace MappingEdu.Service.Model.MappingProject
{
    public class MappingProjectViewModel
    {
        public string CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string Description { get; set; }

        public bool Flagged { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }

        public Guid MappingProjectId { get; set; }

        public string ProjectName { get; set; }

        public int ProjectStatusTypeId { get; set; }

        public string ProjectStatusTypeName { get; set; }

        public MappingProjectUser.MappingProjectUserRole Role { get; set; }

        public DataStandardViewModel SourceDataStandard { get; set; }

        public Guid SourceDataStandardId { get; set; }

        public DataStandardViewModel TargetDataStandard { get; set; }

        public Guid TargetDataStandardId { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime UserUpdateDate { get; set; }
    }
}