// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.MappedSystemExtension
{
    public class MappedSystemExtensionDetailRequestModel
    {
        public Guid SystemItemId { get; set; }

        public Guid MappedSystemExtensionId { get; set; }
    }

    public class MappedSystemExtensionSystemItemModel
    {
        public string Definition { get; set; }

        public string DomainItemPath { get; set; }

        public Guid SystemItemId { get; set; }
    }
}