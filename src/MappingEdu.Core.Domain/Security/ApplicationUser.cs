// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MappingEdu.Core.Domain.Security
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            FlaggedMappedSystems = new HashSet<MappedSystem>();
            FlaggedProjects = new HashSet<MappingProject>();
            MappedSystems = new HashSet<MappedSystemUser>();
            MappedSystemUpdates = new HashSet<MappedSystemUpdate>();
            Organizations = new HashSet<Organization>();
            Projects = new HashSet<MappingProjectUser>();
            ProjectUpdates = new HashSet<MappingProjectUpdate>();
        }

        public virtual ICollection<MappingProjectQueueFilter> CreatedByFilters { get; set; }

        public string FirstName { get; set; }

        public virtual ICollection<MappedSystem> FlaggedMappedSystems { get; set; }

        public virtual ICollection<MappingProject> FlaggedProjects { get; set; }

        public bool IsActive { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<MappingProjectQueueFilter> MappingProjectQueueFilters { get; set; } 

        public virtual ICollection<MappedSystemUser> MappedSystems { get; set; }

        public virtual ICollection<MappedSystemUpdate> MappedSystemUpdates { get; set; } 

        public virtual ICollection<UserNotification> Notifications { get; set; } 

        public virtual ICollection<Organization> Organizations { get; set; }

        public virtual ICollection<MappingProjectUser> Projects { get; set; }

        public virtual ICollection<MappingProjectUpdate> ProjectUpdates { get; set; }

        public virtual ICollection<MappingProjectQueueFilter> UpdatedByFilters { get; set; }

        /// <summary>
        ///     Generates a user with external bearer token
        /// </summary>
        /// <param name="manager">The user manager</param>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            return await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ExternalBearer);
        }
    }
}