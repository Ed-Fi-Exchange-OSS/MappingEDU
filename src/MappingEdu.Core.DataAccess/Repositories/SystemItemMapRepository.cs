// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class SystemItemMapRepository : Repository<SystemItemMap>, ISystemItemMapRepository
    {
        public SystemItemMapRepository(EntityContext databaseContext) : base(databaseContext)
        {
        }

        public SystemItemMap[] GetByMappingProject(Guid mappingProjectId)
        {
            var entities = _databaseContext.SystemItemMaps.Where(x => x.MappingProjectId.Equals(mappingProjectId));
            return entities.ToArray();
        }

        public IQueryable<SystemItemMap> GetAllMaps()
        {
            return _databaseContext.SystemItemMaps;
        }

        public void SaveChangesWithoutValidation()
        {
            _databaseContext.Configuration.ValidateOnSaveEnabled = false;
            try
            {
                SaveChanges();
            }
            finally
            {
                _databaseContext.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        public SystemItemMap[] GetAllForComparison(Guid mappingProjectId)
        {
            var clientIdParameter = new SqlParameter("@MappingProjectId", mappingProjectId);

            return _databaseContext.Database.SqlQuery<SystemItemMapFlatModel>("GetMappingProjectItemMaps @MappingProjectId", clientIdParameter)
                .GroupBy(x => x.SystemItemMapId).Select(@group => new SystemItemMap
                {
                    SystemItemMapId = @group.First().SystemItemMapId,
                    SourceSystemItemId = @group.First().SourceSystemItemId,
                    BusinessLogic = @group.First().BusinessLogic,
                    OmissionReason = @group.First().OmissionReason,
                    Flagged = @group.First().Flagged,
                    MappingMethodTypeId = @group.First().MappingMethodTypeId,
                    WorkflowStatusTypeId = @group.First().WorkflowStatusTypeId,
                    CreateDate = @group.First().CreateDate,
                    CreateBy = @group.First().CreateBy,
                    UpdateDate = @group.First().UpdateDate,
                    UpdateBy = @group.First().UpdateBy,
                    TargetSystemItems = (@group.First().TargetSystemItem_SystemItemId != null) 
                        ? @group.Select(x => new SystemItem
                          {
                             SystemItemId = x.TargetSystemItem_SystemItemId.Value,
                             ItemTypeId = x.TargetSystemItem_ItemTypeId.Value,
                             MappedSystemId = x.TargetSystemItem_MappedSystemId.Value
                          }).ToList()
                       : new List<SystemItem>()
            }).ToArray();
        }
    }
}