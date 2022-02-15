// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.Domain
{
    public class DomainViewModel
    {
        public string Definition { get; set; }

        public string ItemName { get; set; }

        public string ItemUrl { get; set; }

        public Guid MappedSystemId { get; set; }

        public Guid SystemItemId { get; set; }
    }
}