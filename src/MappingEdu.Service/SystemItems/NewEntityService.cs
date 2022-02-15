// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.Model.Entity;
using MappingEdu.Service.Model.NewSystemItem;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Util;
using ItemChangeType = MappingEdu.Core.Domain.Enumerations.ItemChangeType;
using ItemDataType = MappingEdu.Core.Domain.Enumerations.ItemDataType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.SystemItems
{
    public interface INewEntityService
    {
        NewEntityViewModel Get(Guid entityId, Guid mappingProjectId);

        NewSystemItemViewModel[] GetFirstLevelEntities(Guid mappedSystemId);

        void Delete(Guid entityId);
    }

    public class NewEntityService : INewEntityService
    {
        private readonly IBriefElementService _briefElementService;
        private readonly IRepository<SystemItemCustomDetail> _customDetailRepository;
        private readonly IElementService _elementService;
        private readonly IRepository<SystemItemMap> _mappingRepository;
        private readonly INextVersionDeltaService _nextVersionDeltaService;
        private readonly ISystemItemCustomDetailService _systemItemCustomDetailService;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;
        private readonly IUserRepository _userRepository;

        public NewEntityService(IBriefElementService briefElementService,
            IElementService elementService,
            INextVersionDeltaService nextVersionDeltaService,
            ISystemItemCustomDetailService systemItemCustomDetailService,
            ISystemItemRepository systemItemRepository,
            IRepository<SystemItemVersionDelta> versionRepository,
            IRepository<SystemItemCustomDetail> customDetailRepository,
            IRepository<SystemItemMap> mappingRepository,
            IUserRepository userRepository)
        {
            _systemItemRepository = systemItemRepository;
            _versionRepository = versionRepository;
            _customDetailRepository = customDetailRepository;
            _mappingRepository = mappingRepository;
            _briefElementService = briefElementService;
            _elementService = elementService;
            _nextVersionDeltaService = nextVersionDeltaService;
            _systemItemCustomDetailService = systemItemCustomDetailService;
            _userRepository = userRepository;
        }

        public NewEntityViewModel Get(Guid entityId, Guid mappingProjectId)
        {
            var entity = _systemItemRepository.Get(entityId);
            if (null == entity)
                throw new NotFoundException(String.Format("Unable to find entity with id {0}", entityId));
            if (!Principal.Current.IsAdministrator && !entity.MappedSystem.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var entityItem = new
            {
                entity.SystemItemId,
                entity.ItemName,
                entity.Definition,
                entity.ParentSystemItem,
                entity.IsExtended,
                Elements = entity.ChildSystemItems.Select(si => new
                {
                    si.SystemItemId,
                    si.ItemName,
                    si.Definition,
                    si.TechnicalName,
                    si.DataTypeSource,
                    si.FieldLength,
                    si.ItemTypeId,
                    si.ItemDataTypeId,
                    si.ItemUrl,
                    si.IsExtended,
                    WorkflowStatusTypeId =
                        si.SourceSystemItemMaps
                            .Where(sim => sim.MappingProjectId == mappingProjectId)
                            .Select(sim => sim.WorkflowStatusTypeId)
                            .FirstOrDefault()
                }),
                PreviousVersions = entity.NewSystemItemVersionDeltas.Select(pv => new
                {
                    pv.SystemItemVersionDeltaId,
                    pv.ItemChangeTypeId,
                    pv.OldSystemItemId,
                    PreviousVersionItem = pv.OldSystemItem,
                    pv.Description
                }),
                NextVersions = entity.OldSystemItemVersionDeltas.Select(pv => new
                {
                    pv.SystemItemVersionDeltaId,
                    pv.ItemChangeTypeId,
                    pv.NewSystemItemId,
                    NextVersionItem = pv.NewSystemItem,
                    pv.Description
                }),
                Notes = entity.Notes.Select(n => new NoteViewModel
                {
                    NoteId = n.NoteId,
                    Title = n.Title,
                    Notes = n.Notes,
                    IsEdited = n.CreateDate != n.UpdateDate,
                    CreateDate = n.CreateDate,
                    CreateBy = n.CreateById.HasValue ? GetUserName(n.CreateById) : null
                })
            };

            return new NewEntityViewModel
            {
                SystemItemId = entityItem.SystemItemId,
                ItemName = entityItem.ItemName,
                IsExtended = entity.IsExtended,
                Definition = string.IsNullOrEmpty(entityItem.Definition) ? "None" : entityItem.Definition,
                Elements = entityItem.Elements.Select(si => new NewElementViewModel
                {
                    ElementDetails = new ElementDetailsViewModel
                    {
                        SystemItemId = si.SystemItemId,
                        ItemName = si.ItemName,
                        Definition = si.Definition,
                        TechnicalName = si.TechnicalName,
                        DataTypeSource = si.DataTypeSource,
                        FieldLength = si.FieldLength,
                        IsExtended = si.IsExtended,
                        ItemType = (ItemType) si.ItemTypeId,
                        ItemDataType = (ItemDataType) si.ItemDataTypeId,
                        ItemUrl = si.ItemUrl
                    },
                    MappingStatus = (WorkflowStatusType) si.WorkflowStatusTypeId
                }).ToArray(),
                PreviousVersions = entityItem.PreviousVersions.Select(pv => new PreviousVersionViewModel
                {
                    PreviousVersionId = pv.SystemItemVersionDeltaId,
                    ItemChangeType = (ItemChangeType) pv.ItemChangeTypeId,
                    OldSystemItemId = pv.OldSystemItemId,
                    PreviousVersionItems = null == pv.PreviousVersionItem
                        ? new[] {new ElementListViewModel.ElementPathViewModel.PathSegment {Name = "None"}}
                        : Utility.GetAllItemSegments(pv.PreviousVersionItem).ToArray(),
                    Description = string.IsNullOrEmpty(pv.Description) ? "None" : pv.Description
                }).ToArray(),
                NextVersions = entityItem.NextVersions.Select(nv => new NextVersionViewModel
                {
                    NextVersionId = nv.SystemItemVersionDeltaId,
                    ItemChangeType = (ItemChangeType) nv.ItemChangeTypeId,
                    NewSystemItemId = nv.NewSystemItemId,
                    NextVersionItems = null == nv.NextVersionItem
                        ? new[] {new ElementListViewModel.ElementPathViewModel.PathSegment {Name = "None"}}
                        : Utility.GetAllItemSegments(nv.NextVersionItem).ToArray(),
                    Description = string.IsNullOrEmpty(nv.Description) ? "None" : nv.Description
                }).ToArray(),
                Notes = entityItem.Notes.ToArray(),
                PathSegments = Utility.GetAllItemSegments(new SystemItem
                {
                    SystemItemId = entityItem.SystemItemId,
                    ItemName = entityItem.ItemName,
                    IsExtended = entityItem.IsExtended,
                    Definition = entityItem.Definition,
                    ParentSystemItem = entityItem.ParentSystemItem
                }, false).ToArray()
            };
        }

        public NewSystemItemViewModel[] GetFirstLevelEntities(Guid mappedSystemId)
        {
            var entities = _systemItemRepository.GetAllQueryable()
                .Where(x => x.ParentSystemItem.ItemTypeId == 1 && x.MappedSystemId == mappedSystemId &&
                        x.ItemTypeId == ItemType.Entity.Id).OrderBy(x => x.ItemName);
            var viewModels = entities.Select(x => new NewSystemItemViewModel
            {
                ItemName = x.ItemName,
                SystemItemId = x.SystemItemId,
                ParentSystemItemId = x.ParentSystemItemId.Value,
                ItemTypeId = x.ItemTypeId
            }).ToArray();
            return viewModels;
        }

        public void Delete(Guid entityId)
        {
            if(!_systemItemRepository.Get(entityId).MappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to peform this action");

            RemoveElements(entityId);            

            var nextVersionDeltas = _nextVersionDeltaService.Get(entityId);
            foreach (var nextDelta in nextVersionDeltas)
            {
                _versionRepository.Delete(nextDelta.SystemItemVersionDeltaId);
            }

            var itemCustomDetails = _systemItemCustomDetailService.Get(entityId);
            foreach (var itemCustomDetail in itemCustomDetails)
            {
                _customDetailRepository.Delete(itemCustomDetail.SystemItemCustomDetailId);
            }

            _systemItemRepository.Delete(entityId);
            _systemItemRepository.SaveChanges();
        }


        private string GetUserName(Guid? createdById = null)
        {
            if (!createdById.HasValue) return null;

            var user = _userRepository.GetAllUsers().FirstOrDefault(x => x.Id == createdById.Value.ToString());
            if (user != null) return user.FirstName + " " + user.LastName;

            return null;
        }

        private void RemoveElements(Guid entityId)
        {
            var briefElements = _briefElementService.Get(entityId);
            foreach (var element in briefElements)
            {
                _elementService.CheckIfElementIsSystemItemMapTarget(element.SystemItemId);

                // For each entity/element recursively remove its child entities/elements first
                RemoveElements(element.SystemItemId);
                foreach (var mapping in element.SystemItemMappings)
                {
                    _mappingRepository.Delete(mapping.SystemItemMapId);
                }

                var itemCustomDetails = _systemItemCustomDetailService.Get(element.SystemItemId);
                foreach (var itemCustomDetail in itemCustomDetails)
                {
                    _customDetailRepository.Delete(itemCustomDetail.SystemItemCustomDetailId);
                }

                _systemItemRepository.Delete(element.SystemItemId);
            }
        }
    }
}