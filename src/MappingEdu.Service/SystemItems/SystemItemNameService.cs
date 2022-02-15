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
using MappingEdu.Service.Model.SystemItemName;

namespace MappingEdu.Service.SystemItems
{
    public interface ISystemItemNameService
    {
        /// <summary>
        ///     Gets the name of a system item.
        /// </summary>
        /// <param name="systemItemId">The system item identifier.</param>
        /// <returns>The name of the system item.</returns>
        SystemItemNameViewModel Get(Guid systemItemId);

        /// <summary>
        ///     Edits the name of an system item.
        /// </summary>
        /// <param name="model">The model containing the data to update.</param>
        /// <returns>
        ///     The newly edited system item view model.
        /// </returns>
        SystemItemNameViewModel Put(SystemItemNameEditModel model);
    }

    public class SystemItemNameService : ISystemItemNameService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItem> _systemItemRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SystemItemNameService" /> class.
        /// </summary>
        /// <param name="systemItemRepository">The system item repository.</param>
        /// <param name="mapper"></param>
        public SystemItemNameService(IRepository<SystemItem> systemItemRepository, IMapper mapper)
        {
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        /// <summary>
        ///     Gets the name of an system item.
        /// </summary>
        /// <param name="systemItemId">The system item identifier.</param>
        /// <returns>
        ///     The name of the system item.
        /// </returns>
        public SystemItemNameViewModel Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);
            var viewModel = _mapper.Map<SystemItemNameViewModel>(systemItem);

            return viewModel;
        }

        /// <summary>
        ///     Edits the name of an system item.
        /// </summary>
        /// <param name="model">The model containing the data to update.</param>
        /// <returns>
        ///     The newly edited system item view model.
        /// </returns>
        public SystemItemNameViewModel Put(SystemItemNameEditModel model)
        {
            var systemItem = GetSystemItem(model.SystemItemId, MappedSystemUser.MappedSystemUserRole.Edit);
            systemItem.ItemName = model.ItemName;

            _systemItemRepository.SaveChanges();

            return Get(model.SystemItemId);
        }

        /// <summary>
        ///     Gets the system item from the repository with a check to make sure it
        ///     exists.
        /// </summary>
        /// <param name="systemItemId">The system item identifier.</param>
        /// <param name="role">The role required to do action</param>
        /// <returns>The requested <see cref="SystemItem" />.</returns>
        /// <exception cref="Exception">Thrown if the requested system item does not exist.</exception>
        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);
            if (systemItem == null)
                throw new Exception(string.Format("The system item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return systemItem;
        }
    }
}