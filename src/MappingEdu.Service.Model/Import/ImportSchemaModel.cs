// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;

namespace MappingEdu.Service.Model.Import
{
    public class ImportSchemaModel
    {
        public Stream ImportData { get; set; }

        public Guid MappedSystemId { get; set; }

        public bool OverrideData { get; set; }

    }

    public class ImportSwaggerSchemaModel
    {
        public string Url { get; set; }

        public bool ImportAll { get; set; }
    }
}