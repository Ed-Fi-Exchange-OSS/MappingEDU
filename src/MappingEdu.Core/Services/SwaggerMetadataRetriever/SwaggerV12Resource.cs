// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MappingEdu.Core.Services.SwaggerMetadataRetriever
{
    public class SwaggerV12Resource
    {
        public class Items
        {
            [JsonProperty(PropertyName = "ref")]
            public string reference { get; set; }
        }

        public class Property
        {
            public string type { get; set; }
            public bool required { get; set; }
            public Items items { get; set; }
            public string description { get; set; }
        }

        public class Model
        {
            public Dictionary<string, Property> properties { get; set; }
        }

        public Dictionary<string, Model> models { get; set; }
    }
}
