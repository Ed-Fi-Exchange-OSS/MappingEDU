// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MappingEdu.Core.Domain.System
{
    public class SystemEnumerationItem : Entity, ICloneable<SystemEnumerationItem>
    {
        public string CodeValue { get; set; }

        public string Description { get; set; }

        protected override Guid Id
        {
            get { return SystemEnumerationItemId; }
        }

        public string ShortDescription { get; set; }

        public virtual ICollection<SystemEnumerationItemMap> SourceSystemEnumerationItemMaps { get; set; }

        public Guid SystemEnumerationItemId { get; set; }

        public virtual SystemItem SystemItem { get; set; }

        public Guid SystemItemId { get; set; }

        public virtual ICollection<SystemEnumerationItemMap> TargetSystemEnumerationItemMaps { get; set; }

        public SystemEnumerationItem()
        {
            SourceSystemEnumerationItemMaps = new HashSet<SystemEnumerationItemMap>();
            TargetSystemEnumerationItemMaps = new HashSet<SystemEnumerationItemMap>();
        }

        public SystemEnumerationItem Clone()
        {
            var clone = new SystemEnumerationItem()
            {
                CodeValue = CodeValue,
                Description = Description,
                ShortDescription = ShortDescription
            };

            return clone;
        }
    }
}