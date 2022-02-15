// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.ElementDetails
{
    public class ElementDetailsCreateModel
    {
        public string DataTypeSource { get; set; }

        public string Definition { get; set; }

        public Guid? EnumerationSystemItemId { get; set; }

        public int? FieldLength { get; set; }

        public int? ItemDataTypeId { get; set; }

        public string ItemName { get; set; }

        public string ItemUrl { get; set; }

        public Guid ParentSystemItemId { get; set; }

        public string TechnicalName { get; set; }
    }
}