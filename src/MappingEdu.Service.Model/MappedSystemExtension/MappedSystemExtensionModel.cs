// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MappingEdu.Service.Model.MappedSystemExtension
{
    public class MappedSystemExtensionBaseModel
    {
        public Guid? ExtensionMappedSystemId { get; set; }

        public string ShortName { get; set; }
    }

    public class MappedSystemExtensionDownloadDetailModel : MappedSystemExtensionBaseModel
    {
        public Guid? MappedSystemExtensionId { get; set; }

        public bool IncludeMarkedExtended { get; set; }

        public bool IncludeNotMarkedExtended { get; set; }

        public bool IncludeUpdated { get; set; }

        public bool IncludeRemoved { get; set; }
    }

    public class MappedSystemExtensionModel : MappedSystemExtensionBaseModel
    {
        public Guid MappedSystemExtensionId { get; set; }

        public string ExtensionMappedSystemName { get; set; }

        public string ExtensionMappedSystemVersion { get; set; }
    }

    public class MappedSystemExtensionCreateModel : MappedSystemExtensionBaseModel
    {
        public bool ImportAll { get; set; }
    }

    public class MappedSystemExtensionEditModel : MappedSystemExtensionBaseModel
    {
        public Guid MappedSystemExtensionId { get; set; }

        public bool ImportAll { get; set; }
    }

    public class MappedSystemExtensionLinkingDetailSystemItem
    {
        public string ItemName { get; set; }

        public string Definition { get; set; }

        public int Extensions { get; set; }

        public ICollection<MappedSystemExtensionLinkingDetailSystemItem> ChildSystemItems { get; set; }
    }

    public class MappedSystemExtensionLinkingDetail
    {
        public int MarkedExtensionCount { get; set; }

        public int NotMarkedExtensionCount { get; set; }

        public int ToBeRemovedCount { get; set; }

        public int UpdatedCount { get; set; }
    }
}