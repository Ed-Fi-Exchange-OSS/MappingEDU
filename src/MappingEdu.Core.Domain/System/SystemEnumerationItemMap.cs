// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Domain.System
{
    public class SystemEnumerationItemMap : Entity, ICloneable<SystemEnumerationItemMap>
    {
        public bool DeferredMapping { get; set; }

        public Enumerations.EnumerationMappingStatusReasonType EnumerationMappingStatusReasonType
        {
            get { return Enumerations.EnumerationMappingStatusReasonType.GetByIdOrDefault(EnumerationMappingStatusReasonTypeId); }
            set { EnumerationMappingStatusReasonTypeId = value.Id; }
        }

        public virtual EnumerationMappingStatusReasonType EnumerationMappingStatusReasonType_DoNotUse { get; set; }

        public int? EnumerationMappingStatusReasonTypeId { get; set; }

        public Enumerations.EnumerationMappingStatusType EnumerationMappingStatusType
        {
            get { return Enumerations.EnumerationMappingStatusType.GetByIdOrDefault(EnumerationMappingStatusTypeId); }
            set { EnumerationMappingStatusTypeId = value.Id; }
        }

        public virtual EnumerationMappingStatusType EnumerationMappingStatusType_DoNotUse { get; set; }

        public int? EnumerationMappingStatusTypeId { get; set; }

        protected override Guid Id
        {
            get { return SystemEnumerationItemMapId; }
        }

        public virtual SystemEnumerationItem SourceSystemEnumerationItem { get; set; }

        public Guid SourceSystemEnumerationItemId { get; set; }

        public Guid SystemEnumerationItemMapId { get; set; }

        public virtual SystemItemMap SystemItemMap { get; set; }

        public Guid SystemItemMapId { get; set; }

        public virtual SystemEnumerationItem TargetSystemEnumerationItem { get; set; }

        public Guid? TargetSystemEnumerationItemId { get; set; }

        public SystemEnumerationItemMap Clone()
        {
            var clone = new SystemEnumerationItemMap
            {
                DeferredMapping = DeferredMapping,
                EnumerationMappingStatusReasonTypeId = EnumerationMappingStatusReasonTypeId,
                EnumerationMappingStatusTypeId = EnumerationMappingStatusTypeId,
                SourceSystemEnumerationItemId = SourceSystemEnumerationItemId,
                TargetSystemEnumerationItemId = TargetSystemEnumerationItemId,
            };

            return clone;
        }
    }
}