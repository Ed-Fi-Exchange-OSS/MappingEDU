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
using MappingEdu.Service.Model.Domain;
using MappingEdu.Service.Util;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.Domains
{
    public interface IDomainService
    {
        DomainViewModel[] Get(Guid mappedSystemId);

        DomainViewModel Get(Guid mappedSystemId, Guid systemItemId);

        DomainViewModel Post(DomainCreateModel model);

        DomainViewModel Put(Guid mappedSystemId, DomainEditModel model);

        void Delete(Guid mappedSystemId, Guid systemItemId);
    }

    public class DomainService : IDomainService
    {
        private readonly IRepository<MappedSystem> _mappedSystemRepository;
        private readonly IMapper _mapper;
        private readonly ISystemItemRepository _repository;

        public DomainService(ISystemItemRepository repository, IRepository<MappedSystem> mappedSystemRepository, IMapper mapper)
        {
            _repository = repository;
            _mappedSystemRepository = mappedSystemRepository;
            _mapper = mapper;
        }

        public DomainViewModel[] Get(Guid mappedSystemId)
        {
            if(!Principal.Current.IsAdministrator) GetMappedSystem(mappedSystemId);

            var systemItems = _repository.GetWhere(mappedSystemId, null);
            var viewModels = _mapper.Map<DomainViewModel[]>(systemItems);
            return viewModels;
        }

        public DomainViewModel Get(Guid mappedSystemId, Guid systemItemId)
        {
            var systemItem = GetSystemItem(mappedSystemId, systemItemId);
            var viewModel = _mapper.Map<DomainViewModel>(systemItem);
            return viewModel;
        }

        public DomainViewModel Post(DomainCreateModel model)
        {
            var mappedSystem = GetMappedSystem(model.DataStandardId, MappedSystemUser.MappedSystemUserRole.Edit);

            var systemItem = new SystemItem
            {
                MappedSystem = mappedSystem,
                ItemName = model.ItemName,
                Definition = string.IsNullOrWhiteSpace(model.Definition) ? null : model.Definition,
                ItemUrl = model.ItemUrl,
                ItemType = ItemType.Domain,
                IsActive = true
            };

            _repository.Add(systemItem);
            _repository.SaveChanges();

            return Get(model.DataStandardId, systemItem.SystemItemId);
        }

        public DomainViewModel Put(Guid mappedSystemId, DomainEditModel model)
        {
            var systemItem = GetSystemItem(mappedSystemId, model.SystemItemId, MappedSystemUser.MappedSystemUserRole.Edit);
            systemItem.ItemName = model.ItemName;
            systemItem.Definition = string.IsNullOrWhiteSpace(model.Definition) ? null : model.Definition;
            systemItem.ItemUrl = model.ItemUrl;

            _repository.SaveChanges();

            return Get(mappedSystemId, model.SystemItemId);
        }

        public void Delete(Guid mappedSystemId, Guid systemItemId)
        {
            var systemItem = GetSystemItem(mappedSystemId, systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            // Do a fake delete by setting is active to false
            systemItem.IsActive = false;
            foreach (var child in systemItem.ElementGroupChildItems)
                child.IsActive = false;

            _repository.SaveChanges();
        }

        private SystemItem GetSystemItem(Guid mappedSystemId, Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _repository.Get(systemItemId);

            if (systemItem == null)
                throw new Exception(string.Format("The System Item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0}-level access to peform this action", role));
            if (systemItem.ItemType.Id != ItemType.Domain.Id)
                throw new Exception(string.Format("The System Item with id '{0}' is not a Domain System Item.", systemItemId));
            if (systemItem.MappedSystemId != mappedSystemId)
                throw new Exception(string.Format("Domain System Item with id '{0}' does not have mapped system id of '{1}'.", systemItemId, mappedSystemId));
            if (!systemItem.IsActive)
                throw new Exception(string.Format("Domain System Item with id '{0}' is marked as deleted.", systemItemId));

            return systemItem;
        }

        private MappedSystem GetMappedSystem(Guid mappedSystemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            if (mappedSystem == null)
                throw new Exception(string.Format("Mapped System with id '{0}' does not exist.", mappedSystemId));
            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            if (!mappedSystem.IsActive)
                throw new Exception(string.Format("Mapped System '{0}' is marked as deleted.", mappedSystem.SystemName));

            return mappedSystem;
        }
    }
}