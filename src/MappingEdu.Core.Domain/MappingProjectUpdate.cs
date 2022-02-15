// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using CuttingEdge.Conditions;
using MappingEdu.Core.Domain.Security;

namespace MappingEdu.Core.Domain
{
    public class MappingProjectUpdate
    {

        protected MappingProjectUpdate()
        {
        }

        public MappingProjectUpdate(MappingProject mappingProject, ApplicationUser user)
        {
            Condition.Requires(mappingProject, "mappingProject").IsNotNull();
            Condition.Requires(user, "user").IsNotNull();

            MappingProject = mappingProject;
            UpdateDate = DateTime.Now;
            User = user;
        }

        public MappingProjectUpdate(MappingProject mappingProject, string userId)
        {
            Condition.Requires(mappingProject, "mappingProject").IsNotNull();
            Condition.Requires(userId, "userId").IsNotNullOrWhiteSpace();

            MappingProject = mappingProject;
            UpdateDate = DateTime.Now;
            UserId = userId;
        }

        public MappingProjectUpdate(Guid mappedSystemId, string userId)
        {
            Condition.Requires(userId, "userId").IsNotNullOrWhiteSpace();

            MappingProjectId = mappedSystemId;
            UpdateDate = DateTime.Now;
            UserId = userId;
        }

        public virtual MappingProject MappingProject { get; set; }

        public Guid MappingProjectId { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }
    }
}
