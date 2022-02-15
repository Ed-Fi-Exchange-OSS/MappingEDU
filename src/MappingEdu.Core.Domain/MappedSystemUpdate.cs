// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core.Lifetime;
using CuttingEdge.Conditions;
using MappingEdu.Core.Domain.Security;

namespace MappingEdu.Core.Domain
{
    public class MappedSystemUpdate
    {

        protected MappedSystemUpdate()
        {
        }

        public MappedSystemUpdate(MappedSystem mappedSystem, ApplicationUser user)
        {
            Condition.Requires(mappedSystem, "mappedSystem").IsNotNull();
            Condition.Requires(user, "user").IsNotNull();

            MappedSystem = mappedSystem;
            UpdateDate = DateTime.Now;
            User = user;
        }

        public MappedSystemUpdate(Guid mappedSystemId, string userId)
        {
            Condition.Requires(userId, "userId").IsNotNullOrWhiteSpace();

            MappedSystemId = mappedSystemId;
            UpdateDate = DateTime.Now;
            UserId = userId;
        }

        public virtual MappedSystem MappedSystem { get; set; }

        public Guid MappedSystemId { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }
    }
}
