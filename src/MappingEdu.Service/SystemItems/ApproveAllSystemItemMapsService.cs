// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using AutoMapper.Internal;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.SystemItems
{
    public interface IApproveAllSystemItemMapsService
    {
        BulkActionResultsModel Put(Guid mappingProjectId, MappingProjectViewModel model);
    }

    public class ApproveAllSystemItemMapsService : IApproveAllSystemItemMapsService
    {
        private readonly ISystemItemMapRepository _systemItemMapRepository;

        public ApproveAllSystemItemMapsService(ISystemItemMapRepository systemItemMapRepository)
        {
            _systemItemMapRepository = systemItemMapRepository;
        }

        public BulkActionResultsModel Put(Guid mappingProjectId, MappingProjectViewModel model)
        {
            var reviewedMaps = _systemItemMapRepository
                .GetAllMaps().Where(m => m.MappingProjectId == mappingProjectId
                                         && m.WorkflowStatusTypeId == WorkflowStatusType.Reviewed.Id);

            if(!Principal.Current.IsAdministrator && reviewedMaps.Any() && !reviewedMaps.First().MappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to peform this action");

            var countUpdated = reviewedMaps.Select(x => x.SourceSystemItemId).Distinct().Count();
            reviewedMaps.Each(delegate(SystemItemMap map) { map.WorkflowStatusType = WorkflowStatusType.Approved; });

            _systemItemMapRepository.SaveChanges();

            return new BulkActionResultsModel
            {
                MappingProjectId = mappingProjectId,
                CountUpdated = countUpdated
            };
        }
    }
}