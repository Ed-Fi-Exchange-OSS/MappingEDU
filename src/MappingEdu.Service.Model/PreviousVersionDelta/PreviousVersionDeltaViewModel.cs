// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.DataStandard;

namespace MappingEdu.Service.Model.PreviousVersionDelta
{
    public class PreviousVersionDeltaViewModel
    {
        public string Description { get; set; }

        public ItemChangeType ItemChangeType { get; set; }

        public DataStandardViewModel OldDataStandard { get; set; }

        public Guid? OldMappedSystemId { get; set; }

        public SystemItemNode OldSystemItem { get; set; }

        public Guid? OldSystemItemId { get; set; }

        public Guid SystemItemVersionDeltaId { get; set; }

        public class SystemItemNode
        {
            public string ItemName { get; set; }

            public string ItemTypeName { get; set; }

            public SystemItemNode ParentSystemItem { get; set; }

            public Guid SystemItemId { get; set; }
        }
    }
}