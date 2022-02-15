﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.SystemItemSelector
{
    public class MappedSystemViewModel
    {
        public DomainViewModel[] Domains { get; set; }

        public Guid MappedSystemId { get; set; }

        public string SystemName { get; set; }

        public string SystemVersion { get; set; }
    }

    public class DomainViewModel
    {
        public SystemItemViewModel[] Entities { get; set; }

        public SystemItemViewModel[] Enumerations { get; set; }

        public string ItemName { get; set; }

        public string ItemTypeName { get; set; }

        public Guid SystemItemId { get; set; }
    }

    public class SystemItemViewModel
    {
        public SystemItemViewModel[] ChildSystemItems { get; set; }

        public string ItemName { get; set; }

        public string ItemTypeName { get; set; }

        public Guid SystemItemId { get; set; }
    }
}