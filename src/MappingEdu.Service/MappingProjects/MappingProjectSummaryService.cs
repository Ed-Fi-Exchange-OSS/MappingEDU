// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectSummaryService
    {
        MappingProjectSummaryViewModel[] Get(Guid mappingProjectId, int? itemTypeId = null, Guid? parentSystemItemId = null);

        MappingProjectSummaryDetailViewModel[] GetDetail(Guid mappingProjectId, int? itemTypeId = null, Guid? systemItemId = null);
    }

    public class MappingProjectSummaryService : IMappingProjectSummaryService
    {
        private readonly IMappingProjectRepository _mappingProjectRepository;

        public MappingProjectSummaryService(IMappingProjectRepository mappingProjectRepository)
        {
            _mappingProjectRepository = mappingProjectRepository;
        }

        public MappingProjectSummaryViewModel[] Get(Guid mappingProjectId, int? itemTypeId = null, Guid? parentSystemItemId = null)
        {
            if (!Principal.Current.IsAdministrator) {
                var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
                if (!mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                    throw new SecurityException("User needs at least View Access to peform this action");
            }

            var stats = _mappingProjectRepository.GetSummary(mappingProjectId, itemTypeId, parentSystemItemId);
            return stats;
        }

        public MappingProjectSummaryDetailViewModel[] GetDetail(Guid mappingProjectId, int? itemTypeId = null, Guid? systemItemId = null)
        {
            if (!Principal.Current.IsAdministrator)
            {
                var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
                if (!mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                    throw new SecurityException("User needs at least View Access to peform this action");
            }

            var stats = _mappingProjectRepository.GetSummaryDetail(mappingProjectId, itemTypeId, systemItemId);
            return stats;
        }
    }
}