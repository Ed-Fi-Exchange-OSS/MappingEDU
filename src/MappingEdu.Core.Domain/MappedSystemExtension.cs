// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class MappedSystemExtension: Entity, ICloneable<MappedSystemExtension>
    {
        public MappedSystemExtension()
        {
            SystemItems = new HashSet<SystemItem>();
        }

        public virtual MappedSystem ExtensionMappedSystem { get; set; }

        public Guid? ExtensionMappedSystemId { get; set; }

        public virtual MappedSystem MappedSystem { get; set; }

        public Guid MappedSystemId { get; set; }

        public Guid MappedSystemExtensionId { get; set; }

        public string ShortName { get; set; }

        public virtual ICollection<SystemItem> SystemItems { get; set; }

        protected override Guid Id
        {
            get { return MappedSystemExtensionId; }
        }

        public MappedSystemExtension Clone()
        {
            return Clone(false);
        }

        public MappedSystemExtension Clone(bool cloningMappedSystem)
        {
            var clone = new MappedSystemExtension
            {
                ExtensionMappedSystemId = ExtensionMappedSystemId,
                ShortName = ShortName,
                SystemItems = new List<SystemItem>()
            };

            if (cloningMappedSystem) clone.MappedSystemExtensionId = MappedSystemExtensionId;

            return clone;
        }
    }
}
