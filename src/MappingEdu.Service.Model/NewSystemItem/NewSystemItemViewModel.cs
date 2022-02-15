// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.NewSystemItem
{
    public class NewSystemItemViewModel
    {
        public string ItemName { get; set; }

        public int ItemTypeId { get; set; }
        
        public string Definition { get; set; }

        public Guid ParentSystemItemId { get; set; }

        public Guid SystemItemId { get; set; }
    }
}