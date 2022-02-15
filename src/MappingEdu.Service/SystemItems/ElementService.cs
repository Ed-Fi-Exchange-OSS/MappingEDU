// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Element;
using MappingEdu.Service.Model.SystemItemCustomDetail;

namespace MappingEdu.Service.SystemItems
{
    public interface IElementService
    {
        ElementViewModel Get(Guid systemItemId);

        ElementViewModel Get(Guid mappingProjectId, Guid systemItemId);

        void Delete(Guid systemItemId);

        void CheckIfElementIsSystemItemMapTarget(Guid elementSystemItemId);
    }

    public class ElementService : IElementService
    {
        private readonly IRepository<SystemItemCustomDetail> _customDetailRepository;
        private readonly IElementDetailsService _elementDetailsService;
        private readonly IRepository<SystemItemMap> _mappingRepository;
        private readonly ISystemItemNameService _nameService;
        private readonly INextVersionDeltaService _nextVersionDeltaService;
        private readonly INoteService _noteService;
        private readonly IPreviousVersionDeltaService _previousVersionDeltaService;
        private readonly ISystemItemCustomDetailService _systemItemCustomDetailService;
        private readonly ISystemItemMappingService _systemItemMappingService;
        private readonly IRepository<SystemItem> _systemItemRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;

        public ElementService(ISystemItemNameService nameService,
            INoteService noteService,
            INextVersionDeltaService nextVersionDeltaService,
            IPreviousVersionDeltaService previousVersionDeltaService,
            IElementDetailsService elementDetailsService,
            ISystemItemMappingService systemItemMappingService,
            ISystemItemCustomDetailService systemItemCustomDetailService,
            IRepository<SystemItem> systemItemRepository,
            IRepository<SystemItemVersionDelta> versionRepository,
            IRepository<SystemItemCustomDetail> customDetailRepository,
            IRepository<SystemItemMap> mappingRepository)
        {
            _nameService = nameService;
            _noteService = noteService;
            _nextVersionDeltaService = nextVersionDeltaService;
            _previousVersionDeltaService = previousVersionDeltaService;
            _elementDetailsService = elementDetailsService;
            _systemItemMappingService = systemItemMappingService;
            _systemItemCustomDetailService = systemItemCustomDetailService;
            _systemItemRepository = systemItemRepository;
            _versionRepository = versionRepository;
            _customDetailRepository = customDetailRepository;
            _mappingRepository = mappingRepository;
        }

        public ElementViewModel Get(Guid systemItemId)
        {
            var viewModel = new ElementViewModel
            {
                SystemItemId = systemItemId,
                SystemItemName = _nameService.Get(systemItemId),
                ElementDetails = _elementDetailsService.Get(systemItemId),
                SystemItemMappings = _systemItemMappingService.GetSourceMappings(systemItemId),
                Notes = _noteService.Get(systemItemId),
                NextVersionDeltas = _nextVersionDeltaService.Get(systemItemId),
                PreviousVersionDeltas = _previousVersionDeltaService.Get(systemItemId),
                SystemItemCustomDetailsContainer = new SystemItemCustomDetailsGroupViewModel(_systemItemCustomDetailService.Get(systemItemId), systemItemId)
            };

            return viewModel;
        }

        public ElementViewModel Get(Guid mappingProjectId, Guid systemItemId)
        {
            var viewModel = new ElementViewModel
            {
                SystemItemId = systemItemId,
                SystemItemName = _nameService.Get(systemItemId),
                ElementDetails = _elementDetailsService.Get(systemItemId),
                SystemItemMappings =
                    _systemItemMappingService.GetItemMappingsByProject(systemItemId, mappingProjectId),
                Notes = _noteService.Get(systemItemId),
                NextVersionDeltas = _nextVersionDeltaService.Get(systemItemId),
                PreviousVersionDeltas = _previousVersionDeltaService.Get(systemItemId),
                SystemItemCustomDetailsContainer =
                    new SystemItemCustomDetailsGroupViewModel(
                        _systemItemCustomDetailService.Get(systemItemId), systemItemId)
            };

            return viewModel;
        }

        public void Delete(Guid systemItemId)
        {
            if(!Principal.Current.IsAdministrator && !_systemItemRepository.Get(systemItemId).MappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to peform this action");

            CheckIfElementIsSystemItemMapTarget(systemItemId);

            var systemItemMappings = _systemItemMappingService.GetSourceMappings(systemItemId, true);

            foreach (var systemItemMapping in systemItemMappings)
            {
                _mappingRepository.Delete(systemItemMapping.SystemItemMapId);
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

            _systemItemRepository.Delete(systemItemId);
            _systemItemRepository.SaveChanges();
        }

        public void CheckIfElementIsSystemItemMapTarget(Guid elementSystemItemId)
        {
            var systemItem = _systemItemRepository.Get(elementSystemItemId);
            if (systemItem.TargetSystemItemMaps.Count > 0)
            {
                throw new Exception(
                    string.Format(
                        "Cannot delete this item because it or a child element is mapped by {0}.", GetMappingSourceSystemItemNames(systemItem.TargetSystemItemMaps)));
            }
        }

        private string GetMappingSourceSystemItemNames(IEnumerable<SystemItemMap> systemItemMaps)
        {
            return systemItemMaps.Aggregate(
                string.Empty, (current, systemItemMap) => current +
                                                          string.Format("{0}[{1}]", current.Length > 0 ? " and " : string.Empty,
                                                              systemItemMap.SourceSystemItem.MappedSystem.SystemName + "." + BuildSourceSystemItemName(systemItemMap.SourceSystemItem)));
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