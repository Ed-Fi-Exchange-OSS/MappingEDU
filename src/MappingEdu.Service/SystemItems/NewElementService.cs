// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
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
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.Util;
using ItemChangeType = MappingEdu.Core.Domain.Enumerations.ItemChangeType;
using ItemDataType = MappingEdu.Core.Domain.Enumerations.ItemDataType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.SystemItems
{
    public interface INewElementService
    {
        NewElementViewModel Get(Guid systemItemId);

        IEnumerable<ElementDetailsViewModel> GetChildren(Guid systemItemId);

        NewElementViewModel Get(Guid mappingProjectId, Guid systemItemId);

        void Delete(Guid systemItemId);

        void CheckIfElementIsSystemItemMapTarget(Guid elementSystemItemId);
    }

    public class NewElementService : INewElementService
    {
        private readonly IRepository<SystemItemCustomDetail> _customDetailRepository;
        private readonly INextVersionDeltaService _nextVersionDeltaService;
        private readonly ISystemItemCustomDetailService _systemItemCustomDetailService;
        private readonly ISystemItemMappingService _systemItemMappingService;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;
        private readonly IUserRepository _userRepository;


        public NewElementService(INextVersionDeltaService nextVersionDeltaService,
            ISystemItemMappingService systemItemMappingService,
            ISystemItemCustomDetailService systemItemCustomDetailService,
            ISystemItemRepository systemItemRepository,
            IRepository<SystemItemVersionDelta> versionRepository,
            IRepository<SystemItemCustomDetail> customDetailRepository,
            IUserRepository userRepository)
        {
            _nextVersionDeltaService = nextVersionDeltaService;
            _systemItemMappingService = systemItemMappingService;
            _systemItemCustomDetailService = systemItemCustomDetailService;
            _systemItemRepository = systemItemRepository;
            _versionRepository = versionRepository;
            _customDetailRepository = customDetailRepository;
            _userRepository = userRepository;
        }

        public NewElementViewModel Get(Guid systemItemId)
        {
            return Get(Guid.Empty, systemItemId);
        }

        public NewElementViewModel Get(Guid mappingProjectId, Guid systemItemId)
        {
            var element = _systemItemRepository.GetAllItems().SingleOrDefault(si => si.SystemItemId == systemItemId);
            if (null == element)
                throw new Exception("Element not found with id: " + systemItemId);
            if (!Principal.Current.IsAdministrator && !element.MappedSystem.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var viewModel = new NewElementViewModel
            {
                SystemItemId = element.SystemItemId,
                ElementDetails = new ElementDetailsViewModel
                {
                    SystemItemId = element.SystemItemId,
                    ItemName = element.ItemName,
                    Definition = element.Definition,
                    TechnicalName = element.TechnicalName,
                    DataTypeSource = element.DataTypeSource,
                    FieldLength = element.FieldLength,
                    ItemTypeId = element.ItemTypeId,
                    ItemType = (ItemType) element.ItemTypeId,
                    ItemDataTypeId = element.ItemDataTypeId,
                    ItemDataType = (ItemDataType) element.ItemDataTypeId,
                    EnumerationSystemItemId = element.EnumerationSystemItemId,
                    EnumerationSystemItemName =
                        null == element.EnumerationSystemItem ? null : element.EnumerationSystemItem.ItemName,
                    EnumerationSystemItemDefinition =
                        null == element.EnumerationSystemItem ? null : element.EnumerationSystemItem.Definition,
                    ItemUrl = element.ItemUrl,
                    FullItemPath = Utility.GetFullItemPath(element),
                    DomainItemPath = Utility.GetDomainItemPath(element),
                    PathSegments = Utility.GetAllItemSegments(element, false).ToArray()
                },
                SystemItemMappings = mappingProjectId.Equals(Guid.Empty)
                    ? _systemItemMappingService.GetSourceMappings(systemItemId)
                    : _systemItemMappingService.GetItemMappingsByProject(systemItemId, mappingProjectId),
                PreviousVersions = element.NewSystemItemVersionDeltas.Select(pv => new PreviousVersionViewModel
                {
                    PreviousVersionId = pv.SystemItemVersionDeltaId,
                    ItemChangeType = (ItemChangeType) pv.ItemChangeTypeId,
                    OldSystemItemId = pv.OldSystemItemId,
                    PreviousVersionItems = null == pv.OldSystemItem
                        ? new[] {new ElementListViewModel.ElementPathViewModel.PathSegment {Name = "None"}}
                        : Utility.GetAllItemSegments(pv.OldSystemItem).ToArray(),
                    Description = string.IsNullOrEmpty(pv.Description) ? "None" : pv.Description
                }).ToArray(),
                NextVersions = element.OldSystemItemVersionDeltas.Where(nv => nv.NewSystemItem != null && nv.NewSystemItem.MappedSystem.IsActive).Select(nv => new NextVersionViewModel
                {
                    NextVersionId = nv.SystemItemVersionDeltaId,
                    ItemChangeType = (ItemChangeType) nv.ItemChangeTypeId,
                    NewSystemItemId = nv.NewSystemItemId,
                    NextVersionItems = null == nv.NewSystemItem
                        ? new[] {new ElementListViewModel.ElementPathViewModel.PathSegment {Name = "None"}}
                        : Utility.GetAllItemSegments(nv.NewSystemItem).ToArray(),
                    Description = string.IsNullOrEmpty(nv.Description) ? "None" : nv.Description
                }).ToArray(),
                Notes = element.Notes.Select(n => new NoteViewModel
                {
                    NoteId = n.NoteId,
                    Title = n.Title,
                    Notes = n.Notes,
                    IsEdited = n.CreateDate != n.UpdateDate,
                    CreateDate = n.CreateDate,
                    CreateBy = n.CreateById.HasValue ? GetUserName(n.CreateById) : null
                }).ToArray(),
                SystemItemCustomDetailsContainer =
                    new SystemItemCustomDetailsGroupViewModel(_systemItemCustomDetailService.Get(systemItemId),
                        systemItemId)
            };

            return viewModel;
        }

        public void Delete(Guid systemItemId)
        {
            if(!Principal.Current.IsAdministrator && !_systemItemRepository.Get(systemItemId).MappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User doesn't have delete access on this data standard");

            CheckIfElementIsSystemItemMapTarget(systemItemId);

            var systemItemMappings = _systemItemMappingService.GetSourceMappings(systemItemId, true);
            foreach (var systemItemMapping in systemItemMappings)
            {
                _systemItemMappingService.Delete(systemItemMapping.SourceSystemItemId, systemItemMapping.SystemItemMapId);
            }

            var nextVersionDeltas = _nextVersionDeltaService.Get(systemItemId);
            foreach (var nextDelta in nextVersionDeltas)
            {
                _versionRepository.Delete(nextDelta.SystemItemVersionDeltaId);
            }

            var itemCustomDetails = _systemItemCustomDetailService.Get(systemItemId);
            foreach (var itemCustomDetail in itemCustomDetails)
            {
                _customDetailRepository.Delete(itemCustomDetail.SystemItemCustomDetailId);
            }
            
            //Loops through and deletes all child items
            var childSystemItems = _systemItemRepository.Get(systemItemId).ChildSystemItems.Select(x => x.SystemItemId).ToList();
            foreach (var item in childSystemItems)
            {
                Delete(item);
            }

            _systemItemRepository.Delete(systemItemId);
            _systemItemRepository.SaveChanges();
        }

        public void CheckIfElementIsSystemItemMapTarget(Guid elementSystemItemId)
        {
            var systemItem = _systemItemRepository.Get(elementSystemItemId);
            if (systemItem.TargetSystemItemMaps.Count > 0)
            {
                throw new BusinessException(
                    string.Format(
                        "Cannot delete this item because it or a child element is mapped by {0}.",
                        GetMappingSourceSystemItemNames(systemItem.TargetSystemItemMaps)));
            }
        }

        public IEnumerable<ElementDetailsViewModel> GetChildren(Guid itemId)
        {
            var systemItem = _systemItemRepository.Get(itemId);

            var childern = null == systemItem ?
                _systemItemRepository.GetAllItems().Where(x => x.MappedSystemId == itemId && x.ParentSystemItem == null && !x.MappedSystemExtensionId.HasValue).ToList()
                : systemItem.ChildSystemItems.Where(x => !x.MappedSystemExtensionId.HasValue).ToList();
            
            if(!Principal.Current.IsAdministrator && childern.Any() && !childern.First().MappedSystem.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            return childern.Select(element => new ElementDetailsViewModel
            {
                SystemItemId = element.SystemItemId,
                ItemName = element.ItemName,
                IsExtended = element.IsExtended,
                Definition = element.Definition,
                TechnicalName = element.TechnicalName,
                DataTypeSource = element.DataTypeSource,
                FieldLength = element.FieldLength,
                ItemTypeId = element.ItemTypeId,
                ItemType = (ItemType)element.ItemTypeId,
                ItemDataTypeId = element.ItemDataTypeId,
                ItemDataType = (ItemDataType)element.ItemDataTypeId,
                ParentSystemItemId = element.ParentSystemItemId,
                EnumerationSystemItemName = null == element.EnumerationSystemItem ? null : element.EnumerationSystemItem.ItemName,
                EnumerationSystemItemDefinition = null == element.EnumerationSystemItem ? null : element.EnumerationSystemItem.Definition,
                
                ItemUrl = element.ItemUrl
            }).OrderBy(x => x.ItemTypeId).ThenBy(x => x.ItemName);

        }

        private string GetMappingSourceSystemItemNames(IEnumerable<SystemItemMap> systemItemMaps)
        {
            return systemItemMaps.Aggregate(
                string.Empty, (current, systemItemMap) => current +
                                                          string.Format("{0}[{1}]",
                                                              current.Length > 0 ? " and " : string.Empty,
                                                              systemItemMap.SourceSystemItem.MappedSystem.SystemName +
                                                              "." +
                                                              BuildSourceSystemItemName(systemItemMap.SourceSystemItem)));
        }

        private string GetUserName(Guid? createdById = null)
        {
            if (!createdById.HasValue) return null;

            var user = _userRepository.GetAllUsers().FirstOrDefault(x => x.Id == createdById.Value.ToString());
            if (user != null) return user.FirstName + " " + user.LastName;

            return null;
        }

        private string BuildSourceSystemItemName(SystemItem systemItem)
        {
            var sourceSystemItemName = string.Empty;
            var isNotNullParent = null != systemItem.ParentSystemItem;
            if (isNotNullParent)
            {
                sourceSystemItemName += BuildSourceSystemItemName(systemItem.ParentSystemItem);
            }

            sourceSystemItemName += string.Format("{0}{1}", isNotNullParent ? "." : string.Empty, systemItem.ItemName);
            return sourceSystemItemName;
        }
    }
}
