// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using MappingEdu.Core.Domain.Security;

namespace MappingEdu.Core.Domain
{
    public class MappedSystemUser
    {
        public enum MappedSystemUserRole
        {
            Guest = 0,
            View = 1,
            Edit = 2,
            Owner = 99
        }

        protected MappedSystemUser()
        {
        }

        public MappedSystemUser(MappedSystem mappedSystem, ApplicationUser user, MappedSystemUserRole role)
        {
            Condition.Requires(mappedSystem, "mappedSystem").IsNotNull();
            Condition.Requires(user, "user").IsNotNull();

            MappedSystem = mappedSystem;
            Role = role;
            User = user;
        }

        public MappedSystemUser(Guid mappedSystemId, string userId, MappedSystemUserRole role)
        {
            Condition.Requires(userId, "userId").IsNotNullOrWhiteSpace();

            MappedSystemId = mappedSystemId;
            Role = role;
            UserId = userId;
        }

        public virtual MappedSystem MappedSystem { get; set; }

        public Guid MappedSystemId { get; set; }

        public MappedSystemUserRole Role { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }
    }
}
