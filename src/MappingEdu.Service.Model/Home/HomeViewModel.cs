// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MappingEdu.Service.Model.Home
{
    public class HomeViewModel
    {
        public ICollection<DataStandardListItem> DataStandardList { get; set; }

        public ICollection<MappingProjectListItem> MappingProjectList { get; set; }
    }

    public class DataStandardListItem
    {
        public bool Flagged { get; set; }

        public DateTime UserUpdateDate { get; set; }

        public string SystemName { get; set; }

        public string SystemVersion { get; set; }

        public Guid DataStandardId { get; set; }

        public bool ContainsExtensions { get; set; }
    }

    public class MappingProjectListItem
    {
        public bool Flagged { get; set; }

        public DateTime UserUpdateDate { get; set; }

        public Guid MappingProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectStatusTypeName { get; set; }

        public DataStandardListItem SourceDataStandard { get; set; }

        public DataStandardListItem TargetDataStandard { get; set; }

        public int Notifications { get; set; }
    }
}