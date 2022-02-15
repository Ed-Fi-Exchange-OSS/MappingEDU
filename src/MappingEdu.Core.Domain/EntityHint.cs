// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class EntityHint : Entity, ICloneable<EntityHint>
    {
        protected override Guid Id
        {
            get { return EntityHintId; }
        }

        public Guid EntityHintId { get; set; }

        public virtual MappingProject MappingProject { get; set; }

        public Guid MappingProjectId { get; set; }

        public Guid SourceEntityId { get; set; }

        public virtual SystemItem SourceEntity { get; set; }

        public Guid TargetEntityId { get; set; }

        public virtual SystemItem TargetEntity { get; set; }

        public EntityHint Clone()
        {
            var clone = new EntityHint
            {
                SourceEntityId = SourceEntityId,
                TargetEntityId = TargetEntityId
            };

            return clone;
        }
    }
}
