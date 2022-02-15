// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.EnumerationItemMapping
{
    public class EnumerationItemMappingViewModel
    {
        public bool DeferredMapping { get; set; }

        public int? EnumerationMappingStatusReasonTypeId { get; set; }

        public int? EnumerationMappingStatusTypeId { get; set; }

        public string SourceCodeValue { get; set; }

        public Guid SourceSystemEnumerationItemId { get; set; }

        public Guid SystemEnumerationItemMapId { get; set; }

        public Guid SystemItemMapId { get; set; }

        public string TargetCodeValue { get; set; }

        public Guid? TargetSystemEnumerationItemId { get; set; }

        public SystemItemNode TargetSystemItem { get; set; }

        public class SystemItemNode
        {
            public string ItemName { get; set; }

            public SystemItemNode ParentSystemItem { get; set; }
        }
    }
}