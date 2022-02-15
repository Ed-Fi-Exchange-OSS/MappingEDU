// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.Enumeration
{
    public class EnumerationCreateModel
    {
        public Guid ParentSystemItemId { get; set; }

        public string SystemItemDefinition { get; set; }

        public string SystemItemName { get; set; }

        public string SystemItemType { get; set; }
    }
}