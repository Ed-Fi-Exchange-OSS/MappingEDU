// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemTree;

namespace MappingEdu.Service.SystemItemTree
{
    public interface ISystemItemTreeService
    {
        /// <summary>
        ///     Gets all available entities and enumerations to be listed in the
        ///     tree view.
        /// </summary>
        /// <param name="domainSystemItemId">The domain system item identifier.</param>
        /// <returns>All top-level entities and enumerations within the system.</returns>
        SystemItemTypeViewModel[] Get(Guid domainSystemItemId);

        /// <summary>
        ///     Gets the specified system item and sub items by its identifier.
        /// </summary>
        /// <param name="domainSystemItemId">The domain system item identifier.</param>
        /// <param name="systemItemId">The system item identifier.</param>
        /// <returns>
        ///     The requested entity or enumeration.
        /// </returns>
        SystemItemTreeViewModel Get(Guid domainSystemItemId, Guid systemItemId);
    }

    public class SystemItemTreeService : ISystemItemTreeService
    {
        private readonly IMapper _mapper;
        private readonly ISystemItemRepository _systemItemRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SystemItemTreeService" /> class.
        /// </summary>
        /// <param name="systemItemRepository">The system item repository.</param>
        public SystemItemTreeService(ISystemItemRepository systemItemRepository, IMapper mapper)
        {
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        /// <summary>
        ///     Gets all top-level entities and enumerations to be listed in the
        ///     tree view.
        /// </summary>
        /// <param name="domainSystemItemId">The domain system item identifier.</param>
        /// <returns>
        ///     All top-level entities and enumerations within the system.
        /// </returns>
        public SystemItemTypeViewModel[] Get(Guid domainSystemItemId)
        {
            var domainSystemItem = _systemItemRepository.Get(domainSystemItemId);
            domainSystemItem = _systemItemRepository.GetWithTreeLoaded(domainSystemItem.MappedSystemId, domainSystemItemId);
            var topLevelSystemItems = domainSystemItem.ChildSystemItems.ToArray();

            var entityItems = topLevelSystemItems.Where(data => data.ItemType == ItemType.Entity);
            var entities = new SystemItemTypeViewModel
            {
                ItemTypeId = ItemType.Entity.Id,
                ItemTypeName = "Entities",
                Children = _mapper.Map<SystemItemTreeViewModel[]>(entityItems)
            };

            var enumerationItems = topLevelSystemItems.Where(data => data.ItemType == ItemType.Enumeration);
            var enumerations = new SystemItemTypeViewModel
            {
                ItemTypeId = ItemType.Enumeration.Id,
                ItemTypeName = "Enumerations",
                Children = _mapper.Map<SystemItemTreeViewModel[]>(enumerationItems)
            };

            return new[]
            {
                entities,
                enumerations
            };
        }

        /// <summary>
        ///     Gets the specified system item and sub items by its identifier.
        /// </summary>
        /// <param name="domainSystemItemId">The domain system item identifier.</param>
        /// <param name="systemItemId">The system item identifier.</param>
        /// <returns>
        ///     The requested entity or enumeration.
        /// </returns>
        /// <exception cref="Exception">Thrown if the systemItemId does not exist in the data context.</exception>
        public SystemItemTreeViewModel Get(Guid domainSystemItemId, Guid systemItemId)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);

            // if the parent item in null then throw an error
            if (systemItem == null)
                throw new Exception(string.Format("The system item id '{0}' was not found.", systemItemId));

            // map the parent system item to the view model
            var viewModel = _mapper.Map<SystemItemTreeViewModel>(systemItem);

            return viewModel;
        }
    }
}