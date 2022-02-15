// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Domain.System
{
    public class SystemItemCustomDetail : Entity, ICloneable<SystemItemCustomDetail>
    {
        public virtual CustomDetailMetadata CustomDetailMetadata { get; set; }

        public Guid CustomDetailMetadataId { get; set; }

        protected override Guid Id
        {
            get { return SystemItemCustomDetailId; }
        }

        public virtual SystemItem SystemItem { get; set; }

        public Guid SystemItemCustomDetailId { get; set; }

        public Guid SystemItemId { get; set; }

        public string Value { get; set; }

        public SystemItemCustomDetail Clone()
        {
            var clone = new SystemItemCustomDetail()
            {
                CustomDetailMetadataId = CustomDetailMetadataId,
                Value = Value,
            };

            return clone;
        }
    }
}