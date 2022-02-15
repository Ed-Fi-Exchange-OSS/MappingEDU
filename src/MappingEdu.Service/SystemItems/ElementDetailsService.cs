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
using MappingEdu.Service.Model.ElementDetails;
using ItemDataType = MappingEdu.Core.Domain.Enumerations.ItemDataType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.SystemItems
{
    public interface IElementDetailsService
    {
        ElementDetailsViewModel Get(Guid systemItemId);

        ElementDetailsViewModel Post(ElementDetailsCreateModel model);

        ElementDetailsViewModel Put(Guid systemItemId, ElementDetailsEditModel model);
    }

    public class ElementDetailsService : IElementDetailsService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItem> _systemItemRepository;

        public ElementDetailsService(IRepository<SystemItem> systemItemRepository, IMapper mapper)
        {
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        public ElementDetailsViewModel Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);
            var viewModel = _mapper.Map<ElementDetailsViewModel>(systemItem);
            return viewModel;
        }

        public ElementDetailsViewModel Post(ElementDetailsCreateModel model)
        {
            var parentSystemItem = GetSystemItem(model.ParentSystemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            var systemItem = new SystemItem
            {
                MappedSystemId = parentSystemItem.MappedSystemId,
                ParentSystemItemId = model.ParentSystemItemId,
                ItemName = model.ItemName,
                TechnicalName = model.TechnicalName,
                Definition = model.Definition,
                FieldLength = model.FieldLength,
                ItemDataType = ItemDataType.GetByIdOrDefault(model.ItemDataTypeId),
                DataTypeSource = model.DataTypeSource,
                ItemUrl = model.ItemUrl,
                EnumerationSystemItemId =
                    model.ItemDataTypeId == ItemDataType.Enumeration.Id ? model.EnumerationSystemItemId : null,
                ItemType = ItemType.Element,
                IsActive = true
            };

            parentSystemItem.ChildSystemItems.Add(systemItem);
            _systemItemRepository.SaveChanges();

            return Get(systemItem.SystemItemId);
        }

        public ElementDetailsViewModel Put(Guid systemItemId, ElementDetailsEditModel model)
        {
            var element = GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            element.ItemName = model.ItemName;
            element.TechnicalName = model.TechnicalName;
            element.Definition = model.Definition;
            element.FieldLength = model.FieldLength;
            element.ItemDataType = ItemDataType.GetByIdOrDefault(model.ItemDataTypeId);
            element.DataTypeSource = model.DataTypeSource;
            element.ItemUrl = model.ItemUrl;
            element.EnumerationSystemItemId =
                model.ItemDataTypeId == ItemDataType.Enumeration.Id ? model.EnumerationSystemItemId : null;

            _systemItemRepository.SaveChanges();

            return Get(element.SystemItemId);
        }

        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);
            if (systemItem == null)
                throw new Exception(string.Format("System Item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return systemItem;
        }
    }
}