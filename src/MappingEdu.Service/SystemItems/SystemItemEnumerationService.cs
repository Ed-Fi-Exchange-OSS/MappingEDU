// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemEnumeration;

namespace MappingEdu.Service.SystemItems
{
    public interface ISystemItemEnumerationService
    {
        SystemItemEnumerationViewModel[] Get(Guid dataStandardId);
    }

    public class SystemItemEnumerationService : ISystemItemEnumerationService
    {
        private readonly ISystemItemRepository _systemItemRepository;

        public SystemItemEnumerationService(ISystemItemRepository systemItemRepository)
        {
            _systemItemRepository = systemItemRepository;
        }

        public SystemItemEnumerationViewModel[] Get(Guid dataStandardId)
        {
            var enumerationItems = _systemItemRepository.GetAllItems()
                .Where(si => si.MappedSystemId == dataStandardId && si.ItemTypeId == ItemType.Enumeration.Id);

            if(!Principal.Current.IsAdministrator && enumerationItems.Any() && !enumerationItems.First().MappedSystem.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            return enumerationItems.Select(si => new SystemItemEnumerationViewModel
                    {
                        SystemItemId = si.SystemItemId,
                        ItemName = si.ItemName,
                        Definition = si.Definition,
                    }).ToArray();
        }
    }
}