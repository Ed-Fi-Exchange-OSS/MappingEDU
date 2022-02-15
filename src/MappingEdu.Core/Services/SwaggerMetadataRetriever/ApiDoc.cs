// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingEdu.Core.Services.SwaggerMetadataRetriever
{
    public class ApiDoc
    {
        public Api[] apis { get; set; }
    }

    public class Api
    {
        public string path { get; set; }
        public string description { get; set; }
    }

}
