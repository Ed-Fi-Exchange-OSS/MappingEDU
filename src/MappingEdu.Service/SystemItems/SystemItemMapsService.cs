// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using AutoMapper.Internal;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Model.SystemItemBulk;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.SystemItems
{
    public interface ISystemItemMapsService
    {
        BulkActionResultsModel CreateMappings(Guid mappingProjectId, BulkAddMappingsModel model);

        BulkActionResultsModel MarkReviewApproved(Guid mappingProjectId);

        BulkActionResultsModel NumberToCreate(Guid mappingProjectId, BulkAddMappingsModel model);

        BulkActionResultsModel NumberToUpdate(Guid mappingProjectId, BulkUpdateMappingsModel model);

        BulkActionResultsModel UpdateMappings(Guid mappingProjectId, BulkUpdateMappingsModel model);
    }

    public class SystemItemMapsService : ISystemItemMapsService
    {
        private readonly ISystemItemMapRepository _systemItemMapRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IMappingProjectRepository _mappingProjectRepository;

        public SystemItemMapsService(ISystemItemRepository systemItemRepository, ISystemItemMapRepository systemItemMapRepository, IMappingProjectRepository mappingProjectRepository)
        {
            _systemItemRepository = systemItemRepository;
            _systemItemMapRepository = systemItemMapRepository;
            _mappingProjectRepository = mappingProjectRepository;
        }

        public BulkActionResultsModel CreateMappings(Guid mappingProjectId, BulkAddMappingsModel model)
        {
            IEnumerable<SystemItem> systemItems;

            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            if (model.SystemItemIds.Any())
            {
                var children = _systemItemRepository.GetAllQueryable().Where(x => model.SystemItemIds.Contains(x.SystemItemId)).SelectMany(x => x.ChildSystemItems);
                systemItems = GetAllWithoutMappings(mappingProjectId, children, model.ItemType.Id);
            }
            else systemItems = _systemItemRepository.GetAllQueryable().Where(x => x.MappedSystemId == mappingProject.SourceDataStandardMappedSystemId
                    && x.SourceSystemItemMaps.All(y => y.MappingProjectId != mappingProjectId));

            if (model.ItemType.Id > 0)
                systemItems = systemItems.Where(x => x.ItemTypeId == model.ItemType.Id);

            foreach (var item in systemItems)
            {
                _systemItemMapRepository.Add(new SystemItemMap
                {
                    MappingProjectId = mappingProjectId,
                    SourceSystemItemId = item.SystemItemId,
                    SourceSystemItem = item,
                    WorkflowStatusTypeId = model.Status.Id,
                    MappingMethodTypeId = model.Method.Id,
                    BusinessLogic = model.Method.Id == MappingMethodType.MarkForExtension.Id ? model.Detail : "",
                    OmissionReason = model.Method.Id == MappingMethodType.MarkForOmission.Id ? model.Detail : "",
                    Flagged = false
                });
            }

            _systemItemMapRepository.SaveChanges();

            return new BulkActionResultsModel
            {
                MappingProjectId = mappingProjectId,
                CountUpdated = systemItems.Count()
            };
        }

        public BulkActionResultsModel MarkReviewApproved(Guid mappingProjectId)
        {
            var reviewedMaps = _systemItemMapRepository
                .GetAllMaps().Where(m => m.MappingProjectId == mappingProjectId
                                         && m.WorkflowStatusTypeId == WorkflowStatusType.Reviewed.Id);

            if (!Principal.Current.IsAdministrator && reviewedMaps.Any() && !reviewedMaps.First().MappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit))
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

        public BulkActionResultsModel NumberToCreate(Guid mappingProjectId, BulkAddMappingsModel model)
        {
            IEnumerable<SystemItem> systemItems;

            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            if (model.SystemItemIds.Any())
            {
                var children = _systemItemRepository.GetAllQueryable().Where(x => model.SystemItemIds.Contains(x.SystemItemId)).SelectMany(x => x.ChildSystemItems);
                systemItems = GetAllWithoutMappings(mappingProjectId, children, model.ItemType.Id);
            }
            else systemItems = _systemItemRepository.GetAllQueryable().Where(x => x.MappedSystemId == mappingProject.SourceDataStandardMappedSystemId
                    && x.SourceSystemItemMaps.All(y => y.MappingProjectId != mappingProjectId));

            return new BulkActionResultsModel
            {
                MappingProjectId = mappingProjectId,
                CountUpdated = systemItems.Count()
            };
        }

        public BulkActionResultsModel NumberToUpdate(Guid mappingProjectId, BulkUpdateMappingsModel model)
        {
            IEnumerable<SystemItemMap> systemItemMaps;

            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            if (model.SystemItemIds.Any())
            {
                var children = _systemItemRepository.GetAllQueryable().Where(x => model.SystemItemIds.Contains(x.SystemItemId)).SelectMany(x => x.ChildSystemItems);
                systemItemMaps = GetAllMappings(mappingProjectId, children, model.ItemType.Id);
            }
            else systemItemMaps = _systemItemMapRepository.GetAllQueryable().Where(x => x.MappingProjectId == mappingProjectId);

            var workflowStatusIds = model.Statuses.Select(x => x.Id);
            var mappingMethodIds = model.Methods.Select(x => x.Id);

            systemItemMaps = systemItemMaps
                .Where(x => workflowStatusIds.Contains(x.WorkflowStatusTypeId) &&
                            mappingMethodIds.Contains(x.MappingMethodTypeId));

            return new BulkActionResultsModel
            {
                MappingProjectId = mappingProjectId,
                CountUpdated = systemItemMaps.Count()
            };
        }

        public BulkActionResultsModel UpdateMappings(Guid mappingProjectId, BulkUpdateMappingsModel model)
        {
            IEnumerable<SystemItemMap> systemItemMaps;

            var mappingProject = GetMappingProject(mappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            if (model.SystemItemIds.Any())
            {
                var children = _systemItemRepository.GetAllQueryable().Where(x => model.SystemItemIds.Contains(x.SystemItemId)).SelectMany(x => x.ChildSystemItems);
                systemItemMaps = GetAllMappings(mappingProjectId, children, model.ItemType.Id);
            }
            else systemItemMaps = _systemItemMapRepository.GetAllQueryable().Where(x => x.MappingProjectId == mappingProjectId);

            var workflowStatusIds = model.Statuses.Select(x => x.Id);
            var mappingMethodIds = model.Methods.Select(x => x.Id);

            systemItemMaps = systemItemMaps
                .Where(x => workflowStatusIds.Contains(x.WorkflowStatusTypeId) &&
                            mappingMethodIds.Contains(x.MappingMethodTypeId));

            var count = systemItemMaps.Count();

            foreach (var map in systemItemMaps)
                map.WorkflowStatusTypeId = model.ChangeStatus.Id;

            _systemItemMapRepository.SaveChanges();

            return new BulkActionResultsModel
            {
                MappingProjectId = mappingProjectId,
                CountUpdated = count
            };
        }

        private static IEnumerable<SystemItemMap> GetAllMappings(Guid mappingProjectId, IEnumerable<SystemItem> systemItems, int itemTypeId)
        {
            var systemItemMaps = new List<SystemItemMap>();
            foreach (var item in systemItems)
            {
                if (item.IsActive && item.ItemTypeId >= 4)
                {
                    if(itemTypeId != 0 && item.ItemTypeId != itemTypeId)
                        continue;
                    var map = item.SourceSystemItemMaps.FirstOrDefault(x => x.MappingProjectId == mappingProjectId);
                    if (map != null) systemItemMaps.Add(map);
                }
                else if (item.IsActive && item.ItemTypeId < 4)
                    systemItemMaps.AddRange(GetAllMappings(mappingProjectId, item.ChildSystemItems, itemTypeId));
            }
            return systemItemMaps;
        }

        private static IEnumerable<SystemItem> GetAllWithoutMappings(Guid mappingProjectId, IEnumerable<SystemItem> systemItems, int itemTypeId)
        {
            var returnItems = new List<SystemItem>();
            foreach (var item in systemItems)
            {
                if (item.IsActive && item.ItemTypeId >= 4 && item.SourceSystemItemMaps.None(x => x.MappingProjectId == mappingProjectId))
                {
                    if (itemTypeId != 0 && item.ItemTypeId != itemTypeId) continue;
                    returnItems.Add(item);
                }
                else if (item.IsActive && item.ItemTypeId < 4)
                    returnItems.AddRange(GetAllWithoutMappings(mappingProjectId, item.ChildSystemItems, itemTypeId));
            }
            return returnItems;
        }

        private MappingProject GetMappingProject(Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (null == mappingProject)
                throw new Exception(string.Format("Mapping Project with id '{0}' does not exist.", mappingProjectId));
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return mappingProject;
        }
    }
}