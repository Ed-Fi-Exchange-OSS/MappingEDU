// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class CompleteStatusType
    {
        public int CompleteStatusTypeId { get; set; }

        public string CompleteStatusTypeName { get; set; }

        public virtual ICollection<SystemItemMap> SystemItemMaps { get; set; }

        public CompleteStatusType()
        {
            SystemItemMaps = new HashSet<SystemItemMap>();
        }
    }
}