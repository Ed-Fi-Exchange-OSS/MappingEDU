// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Service.Model.Datatables;

namespace MappingEdu.Service.Model.ElementList
{
    public class ElementListViewModel
    {
        public ElementPathViewModel[] Elements { get; set; }

        public class ElementPathViewModel
        {
            public ElementSegment Element { get; set; }

            public PathSegment[] PathSegments { get; set; }

            public Guid SystemItemId { get; set; }

            public class PathSegment
            {
                public string Definition { get; set; }

                public string Name { get; set; }

                public Guid SystemItemId { get; set; }

                public bool IsExtended { get; set; }
                
                public string ExtensionShortName { get; set; }
            }

            public class ElementSegment : PathSegment
            {
                public string ItemTypeName { get; set; }

                public int? Length { get; set; }

                public string TypeName { get; set; }
                
            }
        }
    }

    public class ElementListDatatablesModel : DatatablesModel
    {
        public Guid[] ElementGroups { get; set; }
        public Guid[] MappedSystemExtensions { get; set; }
        public int[] ItemTypes { get; set; }
        public bool? IsExtended { get; set; }
    }
}