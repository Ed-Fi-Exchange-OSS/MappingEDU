// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Entities.Tracker
{
    internal class MappedSystemChangeTracker
    {
        /// <summary>
        ///     Takes any changes to a Mapped System and adds/updates
        ///     the users most recent update time on the Mapped System
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tracker"></param>
        public void Process(EntityContext context, DbChangeTracker tracker)
        {
            var mappedSystemsToUpdate = new List<Guid>();
            if (Principal.Current == null) return;

            foreach (var entry in tracker.Entries<MappedSystem>().Where(x => x.State == EntityState.Added))
            {
                if (entry.Entity.UserUpdates == null) entry.Entity.UserUpdates = new List<MappedSystemUpdate>();
                if (entry.Entity.UserUpdates.All(x => x.UserId != Principal.Current.UserId))
                    entry.Entity.UserUpdates.Add(new MappedSystemUpdate(entry.Entity.MappedSystemId, Principal.Current.UserId));
            }

            foreach (var entry in tracker.Entries<MappedSystem>().Where(x => x.State == EntityState.Modified))
            {
                mappedSystemsToUpdate.Add(entry.Entity.MappedSystemId);
            }

            foreach (var entry in tracker.Entries<CustomDetailMetadata>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added || x.State == EntityState.Deleted))
            {
                mappedSystemsToUpdate.Add(entry.Entity.MappedSystemId);
            }

            foreach (var entry in tracker.Entries<SystemItem>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added || x.State == EntityState.Deleted))
            {
                mappedSystemsToUpdate.Add(entry.Entity.MappedSystemId);
            }

            foreach (var entry in tracker.Entries<Note>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added))
            {
                var mappedSystemId = context.SystemItems.Where(x => x.SystemItemId == entry.Entity.SystemItemId).Select(x => x.MappedSystemId).FirstOrDefault();

                mappedSystemsToUpdate.Add(mappedSystemId);
            }

            foreach (var entry in tracker.Entries<Note>().Where(x => x.State == EntityState.Deleted))
            {
                var mappedSystemId = context.SystemItems.Where(x => x.SystemItemId == entry.Entity.SystemItemId).Select(x => x.MappedSystemId).FirstOrDefault();

                mappedSystemsToUpdate.Add(mappedSystemId);
            }

            foreach (var entry in tracker.Entries<SystemItemCustomDetail>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added))
            {
                var mappedSystemId = context.SystemItems.Where(x => x.SystemItemId == entry.Entity.SystemItemId).Select(x => x.MappedSystemId).FirstOrDefault();

                mappedSystemsToUpdate.Add(mappedSystemId);
            }

            foreach (var entry in tracker.Entries<SystemItemCustomDetail>().Where(x => x.State == EntityState.Deleted))
            {
                var mappedSystemId = context.SystemItems.Where(x => x.SystemItemId == entry.Entity.SystemItemId).Select(x => x.MappedSystemId).FirstOrDefault();

                mappedSystemsToUpdate.Add(mappedSystemId);
            }

            // Only add 1 update per save
            foreach (var mappedSystemId in mappedSystemsToUpdate.Distinct().Where(x => x != Guid.Empty))
            {
                var update = context.MappedSystemUpdates.SingleOrDefault(m => m.MappedSystemId == mappedSystemId && m.UserId == Principal.Current.UserId);
                if (null == update)
                {
                    update = context.MappedSystemUpdates.Local.SingleOrDefault(m => m.MappedSystemId == mappedSystemId && m.UserId == Principal.Current.UserId);
                    if (null == update)
                    {
                        update = new MappedSystemUpdate(mappedSystemId, Principal.Current.UserId);
                        context.MappedSystemUpdates.Add(new MappedSystemUpdate(mappedSystemId, Principal.Current.UserId));
                    }
                }
                update.UpdateDate = DateTime.Now;
            }
        }
    }
}
