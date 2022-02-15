// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemDefinition;

namespace MappingEdu.Service.SystemItems
{
    public interface ISystemItemDefinitionService
    {
        /// <summary>
        ///     Gets the specified entity's description.
        /// </summary>
        /// <param name="systemItemId">The entity identifier.</param>
        /// <returns>The entity description.</returns>
        SystemItemDefinitionViewModel Get(Guid systemItemId);

        /// <summary>
        ///     Puts the specified entity description.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        ///     The modified entity description view model.
        /// </returns>
        SystemItemDefinitionViewModel Put(SystemItemDefinitionEditModel model);
    }

    public class SystemItemDefinitionService : ISystemItemDefinitionService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItem> _systemItemRepository;

        public SystemItemDefinitionService(IRepository<SystemItem> systemItemRepository, IMapper mapper)
        {
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        /// <summary>
        ///     Gets the specified entity's description.
        /// </summary>
        /// <param name="systemItemId">The entity identifier.</param>
        /// <returns>
        ///     The entity description.
        /// </returns>
        public SystemItemDefinitionViewModel Get(Guid systemItemId)
        {
            var entity = GetSystemItem(systemItemId);
            var viewModel = _mapper.Map<SystemItemDefinitionViewModel>(entity);

            return viewModel;
        }

        /// <summary>
        ///     Puts the specified entity description.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        ///     The modified entity description view model.
        /// </returns>
        public SystemItemDefinitionViewModel Put(SystemItemDefinitionEditModel model)
        {
            var entity = GetSystemItem(model.SystemItemId, MappedSystemUser.MappedSystemUserRole.Edit);
            entity.Definition = model.Definition;

            _systemItemRepository.SaveChanges();

            return Get(model.SystemItemId);
        }

        /// <summary>
        ///     Gets the entity from the repository with a check to make sure it
        ///     exists.
        /// </summary>
        /// <param name="systemItemId">The entity identifier.</param>
        /// <returns>The requested <see cref="SystemItem" />.</returns>
        /// <exception cref="Exception">Thrown if the requested entity does not exist.</exception>
        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var entity = _systemItemRepository.Get(systemItemId);

            if (entity == null)
                throw new Exception(string.Format("The system item with id '{0}' does not exist.", systemItemId));
                
            if(!Principal.Current.IsAdministrator && !entity.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return entity;
        }
    }
}