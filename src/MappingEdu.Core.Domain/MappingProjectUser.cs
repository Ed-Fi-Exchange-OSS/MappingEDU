// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using CuttingEdge.Conditions;
using MappingEdu.Core.Domain.Security;

namespace MappingEdu.Core.Domain
{
    public class MappingProjectUser
    {
        public enum MappingProjectUserRole
        {
            Guest = 0,
            View = 1,
            Edit = 2,
            Owner = 99
        }

        protected MappingProjectUser()
        {
        }

        public MappingProjectUser(MappingProject mappingProject, ApplicationUser user, MappingProjectUserRole role)
        {
            Condition.Requires(mappingProject, "mappingProject").IsNotNull();
            Condition.Requires(user, "user").IsNotNull();

            MappingProject = mappingProject;
            Role = role;
            User = user;
        }

        public MappingProjectUser(Guid mappingProjectId, string userId, MappingProjectUserRole role)
        {
            Condition.Requires(userId, "userId").IsNotNullOrWhiteSpace();

            MappingProjectId = mappingProjectId;
            Role = role;
            UserId = userId;
        }

        public virtual MappingProject MappingProject { get; set; }

        public Guid MappingProjectId { get; set; }

        public MappingProjectUserRole Role { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }
    }
}