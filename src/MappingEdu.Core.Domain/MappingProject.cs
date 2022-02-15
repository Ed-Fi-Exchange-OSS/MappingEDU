// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Core.Resolving;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class MappingProject : Entity, ICloneable<MappingProject>
    {
        public string Description { get; set; }

        public virtual ICollection<EntityHint> EntityHints { get; set; } 

        public virtual ICollection<ApplicationUser> FlaggedBy { get; set; }

        protected override Guid Id
        {
            get { return MappingProjectId; }
        }

        public bool IsActive { get; set; }

        public bool IsPublic { get; set; }

        public Guid MappingProjectId { get; set; }

        public virtual ICollection<MappingProjectQueueFilter> MappingProjectQueueFilters { get; set; }

        public virtual ICollection<MappingProjectSynonym> MappingProjectSynonyms { get; set; }

        public string ProjectName { get; set; }

        public Enumerations.ProjectStatusType ProjectStatusType
        {
            get { return Enumerations.ProjectStatusType.GetByIdOrDefault(ProjectStatusTypeId); }
            set { ProjectStatusTypeId = value.Id; }
        }

        public int ProjectStatusTypeId { get; set; }

        public virtual MappedSystem SourceDataStandard { get; set; }

        public Guid SourceDataStandardMappedSystemId { get; set; }

        public virtual ICollection<SystemItemMap> SystemItemMaps { get; set; }

        public virtual MappedSystem TargetDataStandard { get; set; }

        public Guid TargetDataStandardMappedSystemId { get; set; }

        public virtual ICollection<MappingProjectTemplate> Templates { get; set; } 

        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public virtual ICollection<MappingProjectUpdate> UserUpdates { get; set; } 

        public virtual ICollection<MappingProjectUser> Users { get; set; }

        public bool HasAccess(MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            return Principal.Current.IsAdministrator ||
                   (IsPublic && role == MappingProjectUser.MappingProjectUserRole.Guest && Principal.Current.IsGuest) ||
                   (IsPublic && role <= MappingProjectUser.MappingProjectUserRole.View && !Principal.Current.IsGuest) ||
                   (Users != null && Users.Count > 0 && Users.Any(x => x.UserId == Principal.Current.UserId && x.Role >= role));
        }

        public MappingProject Clone()
        {
            var clone = new MappingProject
            {
                Description = Description,
                EntityHints = EntityHints.Select(x => x.Clone()).ToList(),
                FlaggedBy = new List<ApplicationUser>(),
                IsActive = true,
                IsPublic = IsPublic,
                MappingProjectSynonyms = MappingProjectSynonyms.Select(x => x.Clone()).ToList(),
                ProjectName = ProjectName,
                ProjectStatusTypeId = ProjectStatusTypeId,
                SourceDataStandardMappedSystemId = SourceDataStandardMappedSystemId,
                SystemItemMaps = SystemItemMaps.Select(x => x.Clone()).ToList(),
                TargetDataStandardMappedSystemId = TargetDataStandardMappedSystemId,
                Templates = Templates.Select(x => x.Clone()).ToList(),
                UserNotifications = new List<UserNotification>(),
                UserUpdates = new List<MappingProjectUpdate>(),
                Users = new List<MappingProjectUser>()
            };

            return clone;
        }
    }
}
