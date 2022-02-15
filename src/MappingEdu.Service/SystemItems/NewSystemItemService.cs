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
using MappingEdu.Service.Model.NewSystemItem;
using MappingEdu.Service.Util;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.SystemItems
{
    public interface INewSystemItemService
    {
        NewSystemItemViewModel Post(NewSystemItemCreateModel model);
    }

    public class NewSystemItemService : INewSystemItemService
    {
        private readonly IRepository<SystemItem> _systemItemRepository;

        public NewSystemItemService(IRepository<SystemItem> systemItemRepository)
        {
            _systemItemRepository = systemItemRepository;
        }

        public NewSystemItemViewModel Post(NewSystemItemCreateModel model)
        {
            var parentSystemItem = GetSystemItem(model.ParentSystemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            var systemItem = new SystemItem
            {
                MappedSystemId = parentSystemItem.MappedSystemId,
                ParentSystemItemId = model.ParentSystemItemId,
                ElementGroupSystemItemId = Utility.GetElementGroupSystemItemId(parentSystemItem),
                ItemName = model.ItemName,
                ItemType = ItemType.GetValues().Single(x => x.Name == model.ItemTypeName),
                Definition = model.Definition,
                IsActive = true
            };

            parentSystemItem.ChildSystemItems.Add(systemItem);
            _systemItemRepository.SaveChanges();

            return new NewSystemItemViewModel
            {
                ParentSystemItemId = systemItem.ParentSystemItemId.Value,
                SystemItemId = systemItem.SystemItemId,
                ItemName = systemItem.ItemName,
                ItemTypeId = systemItem.ItemTypeId,
                Definition = systemItem.Definition
            };
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