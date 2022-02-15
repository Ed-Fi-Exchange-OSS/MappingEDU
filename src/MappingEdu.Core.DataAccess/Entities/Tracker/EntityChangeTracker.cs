// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;

namespace MappingEdu.Core.DataAccess.Entities.Tracker
{
    internal class EntityChangeTracker
    {
        public void Process(EntityContext context, DbChangeTracker tracker)
        {
            // created

            foreach (var entry in tracker.Entries<IEntity>().Where(x => x.State == EntityState.Added))
            {
                entry.Entity.CreateBy = entry.Entity.UpdateBy = Thread.CurrentPrincipal.Identity.Name;

                Guid userId;
                if (Principal.Current != null && Principal.Current.UserId != null && Guid.TryParse(Principal.Current.UserId, out userId))
                    entry.Entity.CreateById = entry.Entity.UpdateById = userId;

                entry.Entity.CreateDate = entry.Entity.UpdateDate = DateTime.Now;
            }

            // updated

            foreach (var entry in tracker.Entries<IEntity>().Where(x => x.State == EntityState.Modified))
            {
                entry.Entity.UpdateBy = Thread.CurrentPrincipal.Identity.Name;
                Guid userId;
                if (Principal.Current != null && Principal.Current.UserId != null && Guid.TryParse(Principal.Current.UserId, out userId))
                    entry.Entity.UpdateById = userId;

                entry.Entity.UpdateDate = DateTime.Now;
            }
        }
    }
}