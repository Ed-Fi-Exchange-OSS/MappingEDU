// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.Enumeration;
using MappingEdu.Service.Util;
using ItemDataType = MappingEdu.Core.Domain.Enumerations.ItemDataType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.SystemItems
{
    public interface IEnumerationService
    {
        EnumerationViewModel Get(Guid elementId);

        void Delete(Guid elementId);
    }

    public class EnumerationService : IEnumerationService
    {
        private readonly IRepository<SystemItemCustomDetail> _customDetailRepository;
        private readonly ISystemItemDefinitionService _descriptionService;
        private readonly IEnumerationItemService _enumerationItemService;
        private readonly IRepository<SystemItemMap> _mappingRepository;
        private readonly ISystemItemNameService _nameService;
        private readonly INextVersionDeltaService _nextVersionDeltaService;
        private readonly INoteService _noteService;
        private readonly IPreviousVersionDeltaService _previousVersionDeltaService;
        private readonly ISystemItemMappingService _systemItemMappingService;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;

        public EnumerationService(ISystemItemNameService nameService,
            IRepository<SystemItemCustomDetail> customDetailRepository,
            ISystemItemDefinitionService descriptionService,
            IEnumerationItemService enumerationItemService,
            INoteService noteService,
            INextVersionDeltaService nextVersionDeltaService,
            IPreviousVersionDeltaService previousVersionDeltaService,
            ISystemItemMappingService systemItemMappingService,
            ISystemItemRepository systemItemRepository,
            IRepository<SystemItemVersionDelta> versionRepository,
            IRepository<SystemItemMap> mappingRepository)
        {
            _nameService = nameService;
            _customDetailRepository = customDetailRepository;
            _descriptionService = descriptionService;
            _enumerationItemService = enumerationItemService;
            _noteService = noteService;
            _nextVersionDeltaService = nextVersionDeltaService;
            _previousVersionDeltaService = previousVersionDeltaService;
            _systemItemMappingService = systemItemMappingService;
            _systemItemRepository = systemItemRepository;
            _versionRepository = versionRepository;
            _mappingRepository = mappingRepository;
        }

        public EnumerationViewModel Get(Guid elementId)
        {
            var viewModel = new EnumerationViewModel
            {
                SystemItemId = elementId,
                SystemItemName = _nameService.Get(elementId),
                SystemItemDefinition = _descriptionService.Get(elementId),
                EnumerationItems = _enumerationItemService.Get(elementId),
                Notes = _noteService.Get(elementId),
                NextVersionDeltas = _nextVersionDeltaService.Get(elementId),
                PreviousVersionDeltas = _previousVersionDeltaService.Get(elementId),
                SystemItemMappings = _systemItemMappingService.GetSourceMappings(elementId)
            };

            var elementOfThisType =
                _systemItemRepository.GetAllItems().Where(s => s.EnumerationSystemItemId == elementId).ToArray();

            viewModel.ElementsOfEnumerationType =
                elementOfThisType.Select(element => new ElementDetailsViewModel
                {
                    SystemItemId = element.SystemItemId,
                    ItemName = element.ItemName,
                    Definition = element.Definition,
                    TechnicalName = element.TechnicalName,
                    DataTypeSource = element.DataTypeSource,
                    FieldLength = element.FieldLength,
                    ItemType = (ItemType) element.ItemTypeId,
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
                }).ToArray();

            return viewModel;
        }

        public void Delete(Guid elementId)
        {
            var enumeration = _systemItemRepository.Get(elementId);
            if (!Principal.Current.IsAdministrator && !enumeration.MappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to peform this action");
            if (enumeration.TargetSystemItemMaps.Count > 0)
                throw new Exception("Cannot delete this enumeration because it is mapped to.");
            if (enumeration.EnumerationUsages.Count > 0)
                throw new Exception("Cannot delete this enumeration because it currently used within the system.");

            var nextVersionDeltas = enumeration.NextSystemItemVersionDeltas.ToArray();
            foreach (var nextDelta in nextVersionDeltas)
                _versionRepository.Delete(nextDelta);

            var systemItemMappings = enumeration.SourceSystemItemMaps.ToArray();
            foreach (var systemItemMapping in systemItemMappings)
                _mappingRepository.Delete(systemItemMapping);

            var itemCustomDetails = enumeration.SystemItemCustomDetails.ToArray();
            foreach (var itemCustomDetail in itemCustomDetails)
            {
                _customDetailRepository.Delete(itemCustomDetail.SystemItemCustomDetailId);
            }

            _systemItemRepository.Delete(elementId);
            _systemItemRepository.SaveChanges();
        }
    }
}