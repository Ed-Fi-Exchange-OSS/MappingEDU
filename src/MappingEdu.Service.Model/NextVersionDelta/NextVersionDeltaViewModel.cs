// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.DataStandard;

namespace MappingEdu.Service.Model.NextVersionDelta
{
    public class NextVersionDeltaViewModel
    {
        public string Description { get; set; }

        public ItemChangeType ItemChangeType { get; set; }

        public DataStandardViewModel NewDataStandard { get; set; }

        public Guid? NewMappedSystemId { get; set; }

        public SystemItemNode NewSystemItem { get; set; }

        public Guid NewSystemItemId { get; set; }

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