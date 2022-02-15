// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Entity;

namespace MappingEdu.Service.SystemItems
{
    public interface IEntityService
    {
        /// <summary>
        ///     Gets the specified entity and all related view data.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns>An <see cref="EntityViewModel" /> loaded with all related data.</returns>
        EntityViewModel Get(Guid entityId);

        void Delete(Guid entityId);
    }

    public class EntityService : IEntityService
    {
        private readonly IBriefElementService _briefElementService;
        private readonly IRepository<SystemItemCustomDetail> _customDetailRepository;
        private readonly ISystemItemDefinitionService _descriptionService;
        private readonly IElementService _elementService;
        private readonly IRepository<SystemItemMap> _mappingRepository;
        private readonly ISystemItemNameService _nameService;
        private readonly INextVersionDeltaService _nextVersionDeltaService;
        private readonly INoteService _noteService;
        private readonly IPreviousVersionDeltaService _previousVersionDeltaService;
        private readonly ISystemItemCustomDetailService _systemItemCustomDetailService;

        private readonly IRepository<SystemItem> _systemItemRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;

        public EntityService(ISystemItemNameService nameService,
            ISystemItemDefinitionService descriptionService,
            IBriefElementService briefElementService,
            IElementService elementService,
            INoteService noteService,
            INextVersionDeltaService nextVersionDeltaService,
            IPreviousVersionDeltaService previousVersionDeltaService,
            ISystemItemCustomDetailService systemItemCustomDetailService,
            IRepository<SystemItem> systemItemRepository,
            IRepository<SystemItemVersionDelta> versionRepository,
            IRepository<SystemItemCustomDetail> customDetailRepository,
            IRepository<SystemItemMap> mappingRepository)
        {
            _nameService = nameService;
            _descriptionService = descriptionService;
            _briefElementService = briefElementService;
            _elementService = elementService;
            _noteService = noteService;
            _nextVersionDeltaService = nextVersionDeltaService;
            _previousVersionDeltaService = previousVersionDeltaService;
            _systemItemCustomDetailService = systemItemCustomDetailService;
            _systemItemRepository = systemItemRepository;
            _versionRepository = versionRepository;
            _customDetailRepository = customDetailRepository;
            _mappingRepository = mappingRepository;
        }

        public EntityViewModel Get(Guid entityId)
        {
            var viewModel = new EntityViewModel
            {
                SystemItemId = entityId,
                SystemItemName = _nameService.Get(entityId),
                SystemItemDefinition = _descriptionService.Get(entityId),
                BriefElements = _briefElementService.Get(entityId),
                Notes = _noteService.Get(entityId),
                NextVersionDeltas = _nextVersionDeltaService.Get(entityId),
                PreviousVersionDeltas = _previousVersionDeltaService.Get(entityId)
            };

            return viewModel;
        }

        public void Delete(Guid entityId)
        {
            if(!Principal.Current.IsAdministrator && !_systemItemRepository.Get(entityId).MappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.Edit))
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