// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class MappingProjectRepository : Repository<MappingProject>, IMappingProjectRepository
    {
        public MappingProjectRepository(EntityContext databaseContext) : base(databaseContext)
        {
        }

        public MappingProject[] GetSourceMappingProjects(Guid dataStandardId)
        {
            var entities = GetAll().Where(x => x.IsActive && x.SourceDataStandardMappedSystemId == dataStandardId);
            return entities.ToArray();
        }

        public MappingProject[] GetTargetMappingProjects(Guid dataStandardId)
        {
            var entities = GetAll().Where(x => x.IsActive && x.TargetDataStandardMappedSystemId == dataStandardId);
            return entities.ToArray();
        }

        public MappingProjectSummaryViewModel[] GetSummary(Guid mappingProjectId, int? itemTypeId = null, Guid? parentSystemItemId = null)
        {
            var summaryDetails = new List<MappingProjectSummaryViewModel>();

            using (var connection = _databaseContext.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "[GetMappingProjectSummary] @MappingProjectId, @ParentSystemItemId";
                if (itemTypeId.HasValue) cmd.CommandText += ", @itemTypeId";

                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingProjectId", Value = mappingProjectId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ParentSystemItemId", Value = parentSystemItemId.HasValue ? parentSystemItemId.Value : Guid.Empty });
                if (itemTypeId.HasValue)  cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeId", Value = itemTypeId.Value });

                using (var reader = cmd.ExecuteReader())
                {
                    summaryDetails =((IObjectContextAdapter)_databaseContext).ObjectContext.Translate<MappingProjectSummaryViewModel>(reader).ToList();
                }
            }

            if(!parentSystemItemId.HasValue)
                summaryDetails.Add(new MappingProjectSummaryViewModel
                {
                    Total = summaryDetails.Sum(x => x.Total),
                    Unmapped = summaryDetails.Sum(x => x.Unmapped),
                    Incomplete = summaryDetails.Sum(x => x.Incomplete),
                    Completed = summaryDetails.Sum(x => x.Completed),
                    Reviewed = summaryDetails.Sum(x => x.Reviewed),
                    Approved = summaryDetails.Sum(x => x.Approved),
                    Mapped = summaryDetails.Sum(x => x.Mapped),
                    Extended = summaryDetails.Sum(x => x.Extended),
                    Omitted = summaryDetails.Sum(x => x.Omitted),
                    ItemName = "All",
                    SystemItemId = Guid.Empty
                });

            foreach (var detail in summaryDetails)
                detail.Percent = (int) ((double)(((double)detail.Approved / (double)detail.Total) * 100.0));
           
            return summaryDetails.ToArray();
        }

        public MappingProjectSummaryDetailViewModel[] GetSummaryDetail(Guid mappingProjectId, int? itemTypeId = null, Guid? systemItemId = null)
        {
            var details = new List<MappingProjectSummaryDetailViewModel>();

            using (var connection = _databaseContext.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "[GetMappingProjectSummaryDetail] @MappingProjectId, @SystemItemId";
                if (itemTypeId.HasValue) cmd.CommandText += ", @itemTypeId";

                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingProjectId", Value = mappingProjectId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SystemItemId", Value = systemItemId.HasValue ? systemItemId.Value : Guid.Empty });
                if (itemTypeId.HasValue) cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeId", Value = itemTypeId.Value });

                using (var reader = cmd.ExecuteReader())
                {
                    details = ((IObjectContextAdapter)_databaseContext).ObjectContext.Translate<MappingProjectSummaryDetailViewModel>(reader).ToList();
                }
            }

            return details.ToArray();
        }

        public override MappingProject[] GetAll()
        {
            var entities = _databaseContext.MappingProjects.Where(x => x.IsActive);

            if (!Principal.Current.IsAdministrator)
            {
                entities = entities.Where(x => x.Users.Any(m => m.UserId == Principal.Current.UserId) || x.IsPublic);
            }

            return entities.ToArray();
        }
    }
}