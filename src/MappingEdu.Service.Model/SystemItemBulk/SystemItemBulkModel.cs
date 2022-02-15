// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Service.Model.Common;

namespace MappingEdu.Service.Model.SystemItemBulk
{
    public class BulkAddMappingsModel
    {
        public ICollection<Guid> SystemItemIds { get; set; }
        
        public DisplayEnumeration Method { get; set; }

        public DisplayEnumeration Status { get; set; }

        public DisplayEnumeration ItemType { get; set; }

        public string Detail { get; set; }
    }

    public class BulkUpdateMappingsModel
    {
        public ICollection<Guid> SystemItemIds { get; set; }

        public ICollection<DisplayEnumeration> Methods { get; set; }

        public ICollection<DisplayEnumeration> Statuses { get; set; }

        public DisplayEnumeration ChangeStatus { get; set; }

        public DisplayEnumeration ItemType  { get; set; }

        public string Detail { get; set; }
    }
}