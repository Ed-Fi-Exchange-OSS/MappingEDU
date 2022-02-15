// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectDashboardService
    {
        MappingProjectDashboardViewModel Get(Guid mappingProjectId);
    }

    public class MappingProjectDashboardService : IMappingProjectDashboardService
    {
        private readonly IRepository<MappingProject> _mappingProjectRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly EntityContext _context;

        public MappingProjectDashboardService(ISystemItemRepository systemItemRepository,
            IRepository<MappingProject> mappingProjectRepository,
            EntityContext context)
        {
            _systemItemRepository = systemItemRepository;
            _mappingProjectRepository = mappingProjectRepository;
            _context = context;
        }

        public MappingProjectDashboardViewModel Get(Guid mappingProjectId)
        {
            var byGroup = new List<MappingProjectDashboardViewModel.MappingGrouping>();
            var byStatus = new List<MappingProjectDashboardViewModel.MappingGrouping>();

            var mappingProjectDashboardViewModel = new MappingProjectDashboardViewModel();

            var project = _mappingProjectRepository.Get(mappingProjectId);
            if(!Principal.Current.IsAdministrator && !project.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var hasViewAccess = Principal.Current.IsAdministrator || project.HasAccess(MappingProjectUser.MappingProjectUserRole.View);

            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                cmd.CommandText = "[GetDashboard] @MappingProjectId";
                cmd.Parameters.Add(new SqlParameter {ParameterName = "MappingProjectId", Value = mappingProjectId});

                using (var reader = cmd.ExecuteReader())
                {
                    byGroup = ((IObjectContextAdapter)_context).ObjectContext.Translate<MappingProjectDashboardViewModel.MappingGrouping>(reader).ToList();
                    reader.NextResult();
                    byStatus = ((IObjectContextAdapter)_context).ObjectContext.Translate<MappingProjectDashboardViewModel.MappingGrouping>(reader).ToList();
                }
            }

            var unmapped = (byStatus.FirstOrDefault(x => x.Filter == "0") == null) ? 0 : byStatus.First(x => x.Filter == "0").Count;
            var incomplete = (byStatus.FirstOrDefault(x => x.Filter == "1") == null) ? 0 : byStatus.First(x => x.Filter == "1").Count;
            var completed = (byStatus.FirstOrDefault(x => x.Filter == "2") == null) ? 0 : byStatus.First(x => x.Filter == "2").Count;
            var reviewed = (byStatus.FirstOrDefault(x => x.Filter == "3") == null) ? 0 : byStatus.First(x => x.Filter == "3").Count;
            var approved = (byStatus.FirstOrDefault(x => x.Filter == "4") == null) ? 0 : byStatus.First(x => x.Filter == "4").Count;

            if (hasViewAccess)
            {
                mappingProjectDashboardViewModel = new MappingProjectDashboardViewModel
                {
                    MappingProjectId = mappingProjectId,
                    WorkQueue = new[]
                    {
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = "Incomplete Mappings",
                            Count = unmapped + incomplete,
                            Filter = "AllIncomplete"
                        },
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = "Mappings Ready for Review",
                            Count = completed,
                            Filter = WorkflowStatusType.Complete.Id.ToString()
                        },
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = "Mappings Ready for Approval",
                            Count = reviewed,
                            Filter = WorkflowStatusType.Reviewed.Id.ToString()
                        }
                    },
                    ElementGroups = new[]
                    {
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = "All Incomplete",
                            Count = unmapped + incomplete,
                            Filter = "AllIncomplete"
                        }
                    }
                        .Union(byGroup).ToArray(),
                    Statuses = new[]
                    {
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = "All Statuses",
                            Count = byStatus.Sum(x => x.Count),
                            Filter = "All"
                        },
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = "Unmapped",
                            Count = unmapped,
                            Filter = "Unmapped"
                        },
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = WorkflowStatusType.Incomplete.Name,
                            Count = incomplete,
                            Filter = WorkflowStatusType.Incomplete.Id.ToString()
                        },
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = WorkflowStatusType.Complete.Name,
                            Count = completed,
                            Filter = WorkflowStatusType.Complete.Id.ToString()
                        },
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = WorkflowStatusType.Reviewed.Name,
                            Count = reviewed,
                            Filter = WorkflowStatusType.Reviewed.Id.ToString()
                        },
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = WorkflowStatusType.Approved.Name,
                            Count = approved,
                            Filter = WorkflowStatusType.Approved.Id.ToString()
                        }
                    }
                };
            }
            else
            {
                mappingProjectDashboardViewModel = new MappingProjectDashboardViewModel
                {
                    MappingProjectId = mappingProjectId,
                    WorkQueue = new MappingProjectDashboardViewModel.MappingGrouping[] {},
                    ElementGroups = new MappingProjectDashboardViewModel.MappingGrouping[] {},
                    Statuses = new[]
                    {
                        new MappingProjectDashboardViewModel.MappingGrouping
                        {
                            GroupName = "All Statuses",
                            Count = byStatus.Sum(x => x.Count),
                            Filter = "All"
                        },
                        
                    }
                };
            }

            return mappingProjectDashboardViewModel;
        }
    }
}