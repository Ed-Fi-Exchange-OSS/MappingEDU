// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class CustomDetailMetadata : Entity, ICloneable<CustomDetailMetadata>
    {
        public Guid CustomDetailMetadataId { get; set; }

        public string DisplayName { get; set; }

        protected override Guid Id
        {
            get { return CustomDetailMetadataId; }
        }

        public bool IsBoolean { get; set; }

        public bool IsCoreDetail { get; set; }

        public virtual MappedSystem MappedSystem { get; set; }

        public Guid MappedSystemId { get; set; }

        public virtual ICollection<SystemItemCustomDetail> SystemItemCustomDetails { get; set; }

        public CustomDetailMetadata()
        {
            SystemItemCustomDetails = new HashSet<SystemItemCustomDetail>();
        }

        public CustomDetailMetadata Clone()
        {
            var clone = new CustomDetailMetadata()
            {
                CustomDetailMetadataId = CustomDetailMetadataId,
                DisplayName = DisplayName,
                IsBoolean = IsBoolean,
                IsCoreDetail = IsCoreDetail,
            };

            return clone;
        }
    }
}