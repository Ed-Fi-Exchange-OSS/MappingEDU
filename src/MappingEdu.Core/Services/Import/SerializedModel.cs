// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Import
{
    public class SerializedMappedSystem
    {
        public SerializedCustomDetailMetadata[] CustomDetails { get; set; }

        public SerializedDomain[] Domains { get; set; }

        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }
    }

    public class SerializedDomain
    {
        public string Definition { get; set; }

        public SerializedEntity[] Entities { get; set; }

        public SerializedEnumeration[] Enumerations { get; set; }

        public Guid? Id { get; set; }

        public string IsExtended { get; set; }

        public string Name { get; set; }
    }

    public class SerializedEntity
    {
        public string Definition { get; set; }

        public SerializedElement[] Elements { get; set; }

        public string Name { get; set; }

        public string IsExtended { get; set; }

        public SerializedEntity[] SubEntities { get; set; }
    }

    public class SerializedElement
    {
        public SerializedElementCustomDetail[] CustomDetails { get; set; }

        public string DataType { get; set; }

        public string Definition { get; set; }

        public int? FieldLength { get; set; }

        public string IsExtended { get; set; }

        public string ItemUrl { get; set; }

        public string Name { get; set; }

        public string TechnicalName { get; set; }
    }

    public class SerializedEnumeration
    {
        public SerializedElementCustomDetail[] CustomDetails { get; set; }

        public string Definition { get; set; }

        public SerializedEnumerationValue[] EnumerationValues { get; set; }

        public string IsExtended { get; set; }

        public string Name { get; set; }
    }

    public class SerializedEnumerationValue
    {
        public string CodeValue { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }
    }

    public class SerializedCustomDetailMetadata
    {
        public bool IsBoolean { get; set; }

        public string Name { get; set; }
    }

    public class SerializedElementCustomDetail
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}