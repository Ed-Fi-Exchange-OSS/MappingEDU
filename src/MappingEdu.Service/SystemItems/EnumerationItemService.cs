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
using MappingEdu.Service.Model.EnumerationItem;

namespace MappingEdu.Service.SystemItems
{
    public interface IEnumerationItemService
    {
        EnumerationItemViewModel Get(Guid systemItemId, Guid systemEnumerationItemId);

        EnumerationItemViewModel[] Get(Guid systemItemId);

        EnumerationItemViewModel Post(Guid systemItemId, EnumerationItemCreateModel model);

        EnumerationItemViewModel Put(Guid systemItemId, Guid systemEnumerationItemId, EnumerationItemEditModel model);

        void Delete(Guid systemItemId, Guid systemEnumerationItemId);
    }

    public class EnumerationItemService : IEnumerationItemService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemEnumerationItem> _systemEnumerationItemRepository;
        private readonly IRepository<SystemItem> _systemItemRepository;

        public EnumerationItemService(IRepository<SystemEnumerationItem> systemEnumerationItemRepository, IRepository<SystemItem> systemItemRepository, IMapper mapper)
        {
            _systemEnumerationItemRepository = systemEnumerationItemRepository;
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        public EnumerationItemViewModel Get(Guid systemItemId, Guid systemEnumerationItemId)
        {
            var enumeration = GetSystemEnumerationItem(systemItemId, systemEnumerationItemId);
            var model = _mapper.Map<EnumerationItemViewModel>(enumeration);
            return model;
        }

        public EnumerationItemViewModel[] Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);
            var model = _mapper.Map<EnumerationItemViewModel[]>(systemItem.SystemEnumerationItems);
            return model;
        }

        public EnumerationItemViewModel Post(Guid systemItemId, EnumerationItemCreateModel model)
        {
            var systemItem = GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            var systemEnumerationItem = new SystemEnumerationItem
            {
                SystemItemId = systemItemId,
                CodeValue = model.CodeValue,
                Description = model.Description,
                ShortDescription = model.ShortDescription
            };

            _systemEnumerationItemRepository.Add(systemEnumerationItem);
            _systemEnumerationItemRepository.SaveChanges();

            return Get(systemItem.SystemItemId, systemEnumerationItem.SystemEnumerationItemId);
        }

        public EnumerationItemViewModel Put(Guid systemItemId, Guid systemEnumerationItemId, EnumerationItemEditModel model)
        {
            var systemEnumerationItem = GetSystemEnumerationItem(systemItemId, systemEnumerationItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            systemEnumerationItem.CodeValue = model.CodeValue;
            systemEnumerationItem.Description = model.Description;
            systemEnumerationItem.ShortDescription = model.ShortDescription;

            _systemEnumerationItemRepository.SaveChanges();

            return Get(systemItemId, systemEnumerationItem.SystemEnumerationItemId);
        }

        public void Delete(Guid systemItemId, Guid systemEnumerationItemId)
        {
            var enumerationItem = GetSystemEnumerationItem(systemItemId, systemEnumerationItemId, MappedSystemUser.MappedSystemUserRole.Edit);
            if (enumerationItem.TargetSystemEnumerationItemMaps.Count > 0)
                throw new Exception("Cannot delete this enumeration item because it is mapped to.");

            _systemEnumerationItemRepository.Delete(systemEnumerationItemId);
            _systemItemRepository.SaveChanges();
        }

        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);
            if (null == systemItem)
                throw new Exception(string.Format("System Item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException("User doesn't have appropriate access for system items on this Data Standard");
            return systemItem;
        }

        private SystemEnumerationItem GetSystemEnumerationItem(Guid systemItemId, Guid systemEnumerationItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var enumeration = _systemEnumerationItemRepository.Get(systemEnumerationItemId);
            if (null == enumeration)
                throw new Exception(string.Format("System Enumeration Item with id '{0}' does not exist.", systemEnumerationItemId));
            if (enumeration.SystemItemId != systemItemId)
                throw new Exception(string.Format("System Enumeration Item with id '{0}' does not have System Item id of '{1}'.", systemEnumerationItemId, systemItemId));
            if (!Principal.Current.IsAdministrator && !enumeration.SystemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return enumeration;
        }
    }
}