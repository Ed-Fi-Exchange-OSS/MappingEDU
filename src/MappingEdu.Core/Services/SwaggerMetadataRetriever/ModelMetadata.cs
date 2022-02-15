// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingEdu.Core.Services.SwaggerMetadataRetriever
{
    public class Metadata
    {
        public string MetadataName => value ?? Name;
        public string MetadataPrefix => sectionPrefix ?? Prefix;

        // Swagger 1.2
        public string value { get; set; }
        public string sectionPrefix { get; set; }

        // Swagger 2.0
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string EndpointUri { get; set; }
    }


    public class ModelMetadata : IModelMetadata
    {
        public string Model { get; set; }
        public string Property { get; set; }
        public string Type { get; set; }
        public bool IsArray { get; set; }
        public bool IsRequired { get; set; }
        public virtual bool IsSimpleType { get; set; }
        public virtual bool IsAttribute { get; set; }

        /// <summary>
        /// Type.property.property.property for nested properties...Used for matching
        /// </summary>
        public string PropertyPath { get; set; }
    }

    public class XmlModelMetadata : ModelMetadata
    {
        public override bool IsSimpleType { get { return Constants.XmlAtomicTypes.Contains(Type); } }
    }

    public class JsonModelMetadata : ModelMetadata
    {
        public string Category { get; set; }
        public string Resource { get; set; }
        public string ResourceDescription { get; set; }
        public bool IsResourceExtension { get; set; }
        public bool IsReference { get; set; }
        public string Description { get; set; }
        public bool IsExtension { get; set; }
        public override bool IsAttribute { get { return false; } }
        public override bool IsSimpleType { get { return Constants.JsonAtomicTypes.Contains(Type); } }
    }

    public class ModelMetadataEqualityComparer<T> :
        IEqualityComparer<T> where T : IModelMetadata
    {
        public bool Equals(T x, T y)
        {
            return x.Model == y.Model &&
                   x.Property == y.Property &&
                   x.Type == y.Type;
        }

        public int GetHashCode(T obj)
        {
            var text = String.Format("{0},{1},{2}", obj.Model, obj.Property, obj.Type);
            return text.GetHashCode();
        }
    }
}