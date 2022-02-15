// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingEdu.Core.Domain.System
{
    public class SystemItem : Entity, ICloneable<SystemItem>
    {
        public SystemItem()
        {
            ChildSystemItems = new HashSet<SystemItem>();
            OldSystemItemVersionDeltas = new HashSet<SystemItemVersionDelta>();
            NewSystemItemVersionDeltas = new HashSet<SystemItemVersionDelta>();
            Notes = new HashSet<Note>();
            SystemEnumerationItems = new HashSet<SystemEnumerationItem>();
            SourceSystemItemMaps = new HashSet<SystemItemMap>();
            TargetSystemItemMaps = new HashSet<SystemItemMap>();
            EnumerationUsages = new HashSet<SystemItem>();
            SystemItemCustomDetails = new HashSet<SystemItemCustomDetail>();
        }

        public virtual ICollection<SystemItem> ChildSystemItems { get; set; }

        public Guid? CopiedFromSystemItemId { get; set; }

        public virtual SystemItem CopiedFromSystemItem { get; set; }

        public string DataTypeSource { get; set; }

        public string Definition { get; set; }

        // Element Groups have access to all system items in group
        public virtual ICollection<SystemItem> ElementGroupChildItems { get; set; }

        public virtual SystemItem ElementGroupSystemItem { get; set; }

        public Guid? ElementGroupSystemItemId { get; set; }

        public virtual SystemItem EnumerationSystemItem { get; set; }

        public Guid? EnumerationSystemItemId { get; set; }

        public virtual ICollection<SystemItem> EnumerationUsages { get; set; }

        public int? FieldLength { get; set; }

        protected override Guid Id
        {
            get { return SystemItemId; }
        }

        public bool IsActive { get; set; }

        public bool IsExtended { get; set; }

        public Enumerations.ItemDataType ItemDataType
        {
            get { return Enumerations.ItemDataType.GetByIdOrDefault(ItemDataTypeId); }
            set { ItemDataTypeId = value.Id; }
        }

        public virtual ItemDataType ItemDataType_DoNotUse { get; set; }

        public int? ItemDataTypeId { get; set; }

        public string ItemName { get; set; }

        public Enumerations.ItemType ItemType
        {
            get { return Enumerations.ItemType.GetByIdOrDefault(ItemTypeId); }
            set { ItemTypeId = value.Id; }
        }

        public virtual ItemType ItemType_DoNotUse { get; set; }

        public int ItemTypeId { get; set; }

        public string ItemUrl { get; set; }

        public virtual ICollection<MappingProjectQueueFilter> MappingProjectQueueFilters { get; set; } 

        public virtual MappedSystem MappedSystem { get; set; }

        public Guid MappedSystemId { get; set; }

        public virtual MappedSystemExtension MappedSystemExtension { get; set; }

        public Guid? MappedSystemExtensionId { get; set; }

        public virtual ICollection<SystemItemVersionDelta> NewSystemItemVersionDeltas { get; set; }

        // these exist to make the usage more straight forward
        public ICollection<SystemItemVersionDelta> NextSystemItemVersionDeltas
        {
            get { return OldSystemItemVersionDeltas; }
        }

        public virtual ICollection<Note> Notes { get; set; }
       
        public virtual ICollection<SystemItemVersionDelta> OldSystemItemVersionDeltas { get; set; }

        public virtual SystemItem ParentSystemItem { get; set; }

        public Guid? ParentSystemItemId { get; set; }

        public ICollection<SystemItemVersionDelta> PreviousSystemItemVersionDeltas
        {
            get { return NewSystemItemVersionDeltas; }
        }

        public virtual ICollection<EntityHint> SourceEntityHints { get; set; }

        public virtual ICollection<SystemItemMap> SourceSystemItemMaps { get; set; }

        public virtual ICollection<SystemEnumerationItem> SystemEnumerationItems { get; set; }

        public virtual ICollection<SystemItem> SystemItemCopies { get; set; }

        public virtual ICollection<SystemItemCustomDetail> SystemItemCustomDetails { get; set; }

        public Guid SystemItemId { get; set; }

        public virtual ICollection<EntityHint> TargetEntityHints { get; set; }

        public virtual ICollection<SystemItemMap> TargetSystemItemMaps { get; set; }

        public string TechnicalName { get; set; }

        //Used for Preprocessed Data

        public string DomainItemPath { get; set; }

        public string DomainItemPathIds { get; set; }

        public string IsExtendedPath { get; set; }

        public SystemItem Clone()
        {
            return Clone(false);
        }

        public SystemItem Clone(bool withExtensions)
        {
            var clone = new SystemItem()
            {
                ChildSystemItems = new List<SystemItem>(),
                DataTypeSource = DataTypeSource,
                Definition = Definition,
                FieldLength = FieldLength,
                IsActive = IsActive,
                ItemDataTypeId = ItemDataTypeId,
                ItemName = ItemName,
                ItemTypeId = ItemTypeId,
                ItemUrl = ItemUrl,
                SystemEnumerationItems = SystemEnumerationItems.Select(enumerationItem => enumerationItem.Clone()).ToList(),
                SystemItemCustomDetails = SystemItemCustomDetails.Select(customDetail => customDetail.Clone()).ToList(),
                TechnicalName = TechnicalName
            };

            var childSystemItems = ChildSystemItems.Where(systemItem => systemItem.IsActive);
            if (!withExtensions) childSystemItems = childSystemItems.Where(systemItem => !systemItem.MappedSystemExtensionId.HasValue);
            else clone.MappedSystemExtensionId = MappedSystemExtensionId;

            clone.ChildSystemItems = childSystemItems
                .Select(systemItem => systemItem.Clone(withExtensions))
                .ToList();
           
            return clone;
        }

        /// <summary>
        ///     Supports Cloning
        /// </summary>
        /// <param name="meta"></param>
        internal void SetMeta(CustomDetailMetadata meta)
        {
            if (null == meta)
                return;

            foreach (var child in ChildSystemItems) child.SetMeta(meta);

            var customDetail = SystemItemCustomDetails.FirstOrDefault(x => x.CustomDetailMetadataId == meta.CustomDetailMetadataId);
            if (customDetail == null) return;

            customDetail.CustomDetailMetadataId = Guid.Empty;
            meta.SystemItemCustomDetails.Add(customDetail);
        }


        /// <summary>
        ///     Supports Extensions
        /// </summary>
        /// <param name="extension"></param>
        internal void SetExtensions(MappedSystemExtension extension)
        {
            if (null == extension)
                return;

            foreach (var child in ChildSystemItems) child.SetExtensions(extension);

            if (MappedSystemExtensionId.HasValue && extension.MappedSystemExtensionId == MappedSystemExtensionId.Value) {
                MappedSystemExtensionId = Guid.Empty;
                extension.SystemItems.Add(this);
            }
        }
    }
}