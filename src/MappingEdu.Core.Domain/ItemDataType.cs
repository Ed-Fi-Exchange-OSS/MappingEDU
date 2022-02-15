// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class ItemDataType
    {
        public int ItemDataTypeId { get; set; }

        public string ItemDataTypeName { get; set; }

        public virtual ICollection<SystemItem> SystemItems { get; set; }

        public ItemDataType()
        {
            SystemItems = new HashSet<SystemItem>();
        }
    }
}