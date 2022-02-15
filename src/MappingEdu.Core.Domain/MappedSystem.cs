// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class MappedSystem : Entity, ICloneable<MappedSystem>
    {
        public MappedSystem()
        {
            Clones = new HashSet<MappedSystem>();
            CustomDetailMetadata = new HashSet<CustomDetailMetadata>();
            Extensions = new HashSet<MappedSystemExtension>();
            ExtensionOfs = new HashSet<MappedSystemExtension>();
            FlaggedBy = new HashSet<ApplicationUser>();
            MappingProjectsWhereSource = new HashSet<MappingProject>();
            MappingProjectsWhereTarget = new HashSet<MappingProject>();
            NextVersionMappedSystems = new HashSet<MappedSystem>();
            SystemItems = new HashSet<SystemItem>();
            Users = new HashSet<MappedSystemUser>();
            UserUpdates = new HashSet<MappedSystemUpdate>();
        }

        public virtual ICollection<CustomDetailMetadata> CustomDetailMetadata { get; set; }

        public Guid? ClonedFromMappedSystemId { get; set; }

        public virtual MappedSystem ClonedFromMappedSystem { get; set; }

        public virtual ICollection<MappedSystem> Clones { get; set; }

        public virtual ICollection<MappedSystemExtension> Extensions { get; set; }

        public virtual ICollection<MappedSystemExtension> ExtensionOfs { get; set; }

        public virtual ICollection<ApplicationUser> FlaggedBy { get; set; }

        protected override Guid Id
        {
            get { return MappedSystemId; }
        }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }

        public bool AreExtensionsPublic { get; set; }

        public Guid MappedSystemId { get; set; }

        public virtual ICollection<MappingProject> MappingProjectsWhereSource { get; set; }

        public virtual ICollection<MappingProject> MappingProjectsWhereTarget { get; set; }

        public virtual ICollection<MappedSystem> NextVersionMappedSystems { get; set; }

        public Guid? PreviousMappedSystemId { get; set; }

        public virtual MappedSystem PreviousVersionMappedSystem { get; set; }

        public virtual ICollection<SystemItem> SystemItems { get; set; }

        [Required(ErrorMessage = "System Name is required.")]
        public string SystemName { get; set; }

        [Required(ErrorMessage = "System Version is required.")]
        public string SystemVersion { get; set; }

        public virtual ICollection<MappedSystemUpdate> UserUpdates { get; set; } 

        public virtual ICollection<MappedSystemUser> Users { get; set; }

        public MappedSystem Clone()
        {
            return Clone(false);
        }

        public MappedSystem Clone(bool withExtensions)
        {
            var clone = new MappedSystem
            {
                CustomDetailMetadata = CustomDetailMetadata.Select(detail => detail.Clone()).ToList(),
                IsActive = IsActive,
                SystemName = SystemName,
                SystemVersion = SystemVersion,
                SystemItems = new List<SystemItem>() 
            };

            var systemItems = SystemItems.Where(systemItem => systemItem.IsActive && systemItem.ParentSystemItemId == null);
            if (!withExtensions) systemItems = systemItems.Where(systemItem => !systemItem.MappedSystemExtensionId.HasValue);

            clone.SystemItems = systemItems
                .Select(systemItem => systemItem.Clone(withExtensions))
                .ToList();

            foreach (var metadata in clone.CustomDetailMetadata)
            {
                foreach (var systemItem in clone.SystemItems)
                    systemItem.SetMeta(metadata);

                metadata.CustomDetailMetadataId = Guid.Empty;
            }

            if (withExtensions)
            {
                clone.Extensions = Extensions.ToList()
                    .Where(extension => extension.ExtensionMappedSystem == null || 
                        extension.ExtensionMappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.View))
                    .Select(extension => extension.Clone(true))
                    .ToList();

                foreach (var extension in clone.Extensions)
                {
                    foreach (var systemItem in clone.SystemItems)
                        systemItem.SetExtensions(extension);

                    extension.MappedSystemExtensionId = Guid.Empty;
                }
            }

            return clone; 
        }

        public bool HasAccess(MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            return Principal.Current.IsAdministrator ||
                   (IsPublic && role == MappedSystemUser.MappedSystemUserRole.Guest && Principal.Current.IsGuest) ||
                   (IsPublic && role <= MappedSystemUser.MappedSystemUserRole.View && !Principal.Current.IsGuest) ||
                   (Users != null && Users.Count > 0 && Users.Any(x => x.UserId == Principal.Current.UserId && x.Role >= role));
        }
    }
}