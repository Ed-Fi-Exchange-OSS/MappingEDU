// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MappingEdu.Service.Model.Import
{
    public class XSDComplexType
    {
        public string Name { get; set; }

        public string Definition { get; set; }

        public string TypeGroup { get; set; }

        public string Base { get; set; }

        public bool IsRestriction { get; set; }

        public ICollection<XSDElement> Elements { get; set; }

    }

    public class XSDElement
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Definition { get; set; }

        public XSDSimpleType SimpleType { get; set; }
    }

    public class XSDSimpleType
    {
        public string Name { get; set; }

        public string Definition { get; set; }

        public string TypeGroup { get; set; }

        public string Base { get; set; }

        public ICollection<XSDSimpleRestriction> Restrictions { get; set; }
    }

    public class XSDSimpleRestriction
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }

    public class XsdModel
    {
        public ICollection<XSDSimpleType> SimpleTypes { get; set; }

        public ICollection<XSDComplexType> ComplexTypes { get; set; }
    }
}