// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectStatusService
    {
        MappingProjectStatusViewModel Get(Guid mappingProjectId);
    }

    public class MappingProjectStatusService : IMappingProjectStatusService
    {
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly ISystemItemRepository _systemItemRepository;

        public MappingProjectStatusService(IMappingProjectRepository mappingProjectRepository, ISystemItemRepository systemItemRepository)
        {
            _mappingProjectRepository = mappingProjectRepository;
            _systemItemRepository = systemItemRepository;
        }

        public MappingProjectStatusViewModel Get(Guid mappingProjectId)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if(!Principal.Current.IsAdministrator || !mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View))
                throw new SecurityException("User needs at least View Access to peform this action");

            var systemItemsForMappingProject =
                _systemItemRepository.GetAllItems()
                    .Where(i => i.MappedSystemId.Equals(mappingProject.SourceDataStandardMappedSystemId) && new[] {ItemType.Element.Id, ItemType.Enumeration.Id}.Contains(i.ItemTypeId));

            var hasUnmappedItems =
                systemItemsForMappingProject.Any(
                    i => i.SourceSystemItemMaps.All(m => m.MappingProjectId != mappingProjectId));

            var hasAllItemsApproved = systemItemsForMappingProject.All(
                i => i.SourceSystemItemMaps.Where(m => m.MappingProjectId == mappingProjectId).All(m => m.WorkflowStatusTypeId == WorkflowStatusType.Approved.Id));

            return new MappingProjectStatusViewModel
            {
                Approved = !hasUnmappedItems && hasAllItemsApproved
            };
        }
    }
}