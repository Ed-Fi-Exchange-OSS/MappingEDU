// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.EnumerationItemMapping
{
    public class EnumerationItemMappingCreateModel
    {
        public bool DeferredMapping { get; set; }

        public int? EnumerationMappingStatusReasonTypeId { get; set; }

        public int? EnumerationMappingStatusTypeId { get; set; }

        public Guid SourceSystemEnumerationItemId { get; set; }

        public Guid? TargetSystemEnumerationItemId { get; set; }
    }
}