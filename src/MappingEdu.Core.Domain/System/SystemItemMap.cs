// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingEdu.Core.Domain.System
{
    public class SystemItemMap : Entity, ICloneable<SystemItemMap>
    {
        public string BusinessLogic { get; set; }

        public Enumerations.CompleteStatusType CompleteStatusType
        {
            get { return Enumerations.CompleteStatusType.GetByIdOrDefault(CompleteStatusTypeId); }
            set { CompleteStatusTypeId = value.Id; }
        }

        public virtual CompleteStatusType CompleteStatusType_DoNotUse { get; set; }

        public int? CompleteStatusTypeId { get; set; }

        public bool DeferredMapping { get; set; }

        public bool ExcludeInExternalReports { get; set; }

        public bool? Flagged { get; set; }

        protected override Guid Id
        {
            get { return SystemItemMapId; }
        }

        public bool IsAutoMapped { get; set; }

        public virtual ICollection<MapNote> MapNotes { get; set; }

        public Enumerations.MappingMethodType MappingMethodType
        {
            get { return Enumerations.MappingMethodType.GetByIdOrDefault(MappingMethodTypeId); }
            set { MappingMethodTypeId = value.Id; }
        }

        public virtual MappingMethodType MappingMethodType_DoNotUse { get; set; }

        public int MappingMethodTypeId { get; set; }

        public virtual MappingProject MappingProject { get; set; }

        public Guid? MappingProjectId { get; set; }

        public Enumerations.MappingStatusReasonType MappingStatusReasonType
        {
            get { return Enumerations.MappingStatusReasonType.GetByIdOrDefault(MappingStatusReasonTypeId); }
            set { MappingStatusReasonTypeId = value.Id; }
        }

        public virtual MappingStatusReasonType MappingStatusReasonType_DoNotUse { get; set; }

        public int? MappingStatusReasonTypeId { get; set; }

        public Enumerations.MappingStatusType MappingStatusType
        {
            get { return Enumerations.MappingStatusType.GetByIdOrDefault(MappingStatusTypeId); }
            set { MappingStatusTypeId = value.Id; }
        }

        public virtual MappingStatusType MappingStatusType_DoNotUse { get; set; }

        public int? MappingStatusTypeId { get; set; }

        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public string OmissionReason { get; set; }

        public virtual SystemItem SourceSystemItem { get; set; }

        public Guid SourceSystemItemId { get; set; }

        public string StatusNote { get; set; }

        public virtual ICollection<SystemEnumerationItemMap> SystemEnumerationItemMaps { get; set; }

        public Guid SystemItemMapId { get; set; }

        public virtual ICollection<SystemItem> TargetSystemItems { get; set; }

        public Enumerations.WorkflowStatusType WorkflowStatusType
        {
            get { return Enumerations.WorkflowStatusType.GetByIdOrDefault(WorkflowStatusTypeId); }
            set { WorkflowStatusTypeId = value.Id; }
        }

        public virtual WorkflowStatusType WorkflowStatusType_DoNotUse { get; set; }

        public int WorkflowStatusTypeId { get; set; }

        public SystemItemMap()
        {
            TargetSystemItems = new HashSet<SystemItem>();
            SystemEnumerationItemMaps = new HashSet<SystemEnumerationItemMap>();
            MapNotes = new HashSet<MapNote>();
        }

        public SystemItemMap Clone()
        {
            var clone = new SystemItemMap
            {
                BusinessLogic = BusinessLogic,
                CompleteStatusTypeId = CompleteStatusTypeId,
                DeferredMapping = DeferredMapping,
                ExcludeInExternalReports = ExcludeInExternalReports,
                Flagged = Flagged,
                IsAutoMapped = IsAutoMapped,
                MapNotes = MapNotes.Select(x => x.Clone()).ToList(),
                MappingMethodTypeId = MappingMethodTypeId,
                MappingStatusReasonTypeId = MappingStatusReasonTypeId,
                MappingStatusTypeId = MappingStatusTypeId,
                OmissionReason = OmissionReason,
                SourceSystemItemId = SourceSystemItemId,
                StatusNote = StatusNote,
                SystemEnumerationItemMaps = SystemEnumerationItemMaps.Select(x => x.Clone()).ToList(),
                TargetSystemItems = TargetSystemItems,
                WorkflowStatusTypeId = WorkflowStatusTypeId
            };

            return clone;
        }
    }
}