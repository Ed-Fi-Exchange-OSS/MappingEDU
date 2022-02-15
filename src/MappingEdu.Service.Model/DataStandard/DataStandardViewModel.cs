// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain;

namespace MappingEdu.Service.Model.DataStandard
{
    public class DataStandardViewModel
    {
        public bool AreExtensionsPublic { get; set; }

        public DataStandardViewModel ClonedFromDataStandard { get; set; }

        public Guid? ClonedFromDataStandardId { get; set; }

        public ICollection<DataStandardCreateModel> Clones { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid DataStandardId { get; set; }

        public bool Flagged { get; set; }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }

        public DataStandardViewModel PreviousDataStandard { get; set; }

        public Guid? PreviousDataStandardId { get; set; }

        public DataStandardViewModel NextDataStandard { get; set; }

        public Guid? NextDataStandardId { get; set; }

        public MappedSystemUser.MappedSystemUserRole Role { get; set; }

        public string SystemName { get; set; }

        public string SystemVersion { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime UserUpdateDate { get; set; }
    }

    public class DataStandardCloningViewModel : DataStandardViewModel
    {
        public bool SimilarVersioning { get; set; }
    }
}