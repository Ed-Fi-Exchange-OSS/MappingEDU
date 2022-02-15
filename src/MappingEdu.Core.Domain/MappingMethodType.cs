﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class MappingMethodType
    {
        public int MappingMethodTypeId { get; set; }

        public string MappingMethodTypeName { get; set; }

        public virtual ICollection<MappingProjectQueueFilter> MappingProjectQueueFilters { get; set; }

        public virtual ICollection<SystemItemMap> SystemItemMaps { get; set; }

        public MappingMethodType()
        {
            SystemItemMaps = new HashSet<SystemItemMap>();
        }
    }
}