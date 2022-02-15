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
    internal class MappingProjectChangeTracker
    {
        /// <summary>
        /// Takes any changes to a Mapping Project and adds/updates 
        /// the users most recent update time on the Mapping Project
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tracker"></param>
        public void Process(EntityContext context, DbChangeTracker tracker)
        {
            var mappingProjectsToUpdate = new List<Guid>();
            if (Principal.Current == null) return;


            foreach (var entry in tracker.Entries<MappingProject>().Where(x => x.State == EntityState.Added))
            {
                if (entry.Entity.UserUpdates == null) entry.Entity.UserUpdates = new List<MappingProjectUpdate>();
                if (entry.Entity.UserUpdates.All(x => x.UserId != Principal.Current.UserId))
                    entry.Entity.UserUpdates.Add(new MappingProjectUpdate(entry.Entity.MappingProjectId, Principal.Current.UserId));
            }

            foreach (var entry in tracker.Entries<MappingProject>().Where(x => x.State == EntityState.Modified))
            {
                mappingProjectsToUpdate.Add(entry.Entity.MappingProjectId);
            }

            foreach (var entry in tracker.Entries<SystemItemMap>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added))
            {
                if(entry.Entity.MappingProjectId.HasValue)
                    mappingProjectsToUpdate.Add(entry.Entity.MappingProjectId.Value);
            }

            foreach (var entry in tracker.Entries<SystemEnumerationItemMap>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added))
            {
                var mappingProjectId = context.SystemItemMaps.Where(x => x.SystemItemMapId == entry.Entity.SystemItemMapId).Select(x => x.MappingProjectId).FirstOrDefault();
                if(mappingProjectId.HasValue)
                    mappingProjectsToUpdate.Add(mappingProjectId.Value);
            }

            foreach (var entry in tracker.Entries<SystemEnumerationItemMap>().Where(x => x.State == EntityState.Deleted))
            {
                var mappingProjectId = context.SystemItemMaps.Where(x => x.SystemItemMapId == entry.Entity.SystemItemMapId).Select(x => x.MappingProjectId).FirstOrDefault();
                if (mappingProjectId.HasValue)
                    mappingProjectsToUpdate.Add(mappingProjectId.Value);
            }

            foreach (var entry in tracker.Entries<MapNote>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added))
            {
                var mappingProjectId = context.SystemItemMaps.Where(x => x.SystemItemMapId == entry.Entity.SystemItemMapId).Select(x => x.MappingProjectId).FirstOrDefault();
                if (mappingProjectId.HasValue)
                    mappingProjectsToUpdate.Add(mappingProjectId.Value);
            }

            foreach (var entry in tracker.Entries<MapNote>().Where(x => x.State == EntityState.Deleted))
            {
                var mappingProjectId = context.SystemItemMaps.Where(x => x.SystemItemMapId == entry.Entity.SystemItemMapId).Select(x => x.MappingProjectId).FirstOrDefault();
                if (mappingProjectId.HasValue)
                    mappingProjectsToUpdate.Add(mappingProjectId.Value);
            }


            foreach (var mappingProjectId in mappingProjectsToUpdate.Distinct().Where(x => x != Guid.Empty))
            {
                var update = context.MappingProjectUpdates.SingleOrDefault(m => m.MappingProjectId == mappingProjectId && m.UserId == Principal.Current.UserId);
                if (null == update)
                {
                    update = context.MappingProjectUpdates.Local.SingleOrDefault(m => m.MappingProjectId == mappingProjectId && m.UserId == Principal.Current.UserId);
                    if (null == update)
                    {
                        update = new MappingProjectUpdate(mappingProjectId, Principal.Current.UserId);
                        context.MappingProjectUpdates.Add(new MappingProjectUpdate(mappingProjectId, Principal.Current.UserId));
                    }
                }
                update.UpdateDate = DateTime.Now;
            }
        }
    }
}