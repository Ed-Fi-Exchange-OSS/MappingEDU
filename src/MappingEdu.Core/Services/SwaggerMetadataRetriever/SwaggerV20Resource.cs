// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace MappingEdu.Core.Services.SwaggerMetadataRetriever
{
    public class SwaggerV20Resource
    {
        public class Items
        {
            [JsonProperty(PropertyName = "ref")]
            public string reference { get; set; }
        }

        public class Property
        {
            public string description { get; set; }
            public string type { get; set; }
            public string format { get; set; }
            public int maxLength { get; set; }
            public Items items { get; set; }

            [JsonProperty(PropertyName = "ref")]
            public string reference { get; set; }
        }

        public class Definition
        {
            public string[] required { get; set; }
            public Dictionary<string, Property> properties { get; set; }
        }

        public class Path
        {
            public PathDefinition get { get; set; }
            public PathDefinition post { get; set; }
        }

        public class PathDefinition
        {
            public string[] tags { get; set; }
            public string summary { get; set; }
            public string description { get; set; }
            public Parameter[] parameters { get; set; }
        }

        public class Parameter
        {
            public string name { get; set; }

            public string definition { get; set; }

            public bool required { get; set; }

            public Schema schema { get; set; }
        }

        public class Schema
        {
            [JsonProperty(PropertyName = "ref")]
            public string reference { get; set; }
        }

        public class Tag
        {
            public string name { get; set; }

            public string description { get; set; }
        }

        public Dictionary<string, Definition> definitions { get; set; }


        public Dictionary<string, Path> paths { get; set; }

        public Tag[] tags { get; set; }


    }
}
