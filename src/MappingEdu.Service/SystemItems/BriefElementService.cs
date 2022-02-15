// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Security;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.BriefElement;

namespace MappingEdu.Service.SystemItems
{
    public interface IBriefElementService
    {
        BriefElementViewModel[] Get(Guid entitySystemItemId);

        BriefElementViewModel Get(Guid entitySystemItemId, Guid elementSystemItemId);
    }

    public class BriefElementService : IBriefElementService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItem> _systemItemRepository;

        public BriefElementService(IRepository<SystemItem> systemItemRepository, IMapper mapper)
        {
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        public BriefElementViewModel[] Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId, null);

            return _mapper.Map<BriefElementViewModel[]>(systemItem.ChildSystemItems);
        }

        public BriefElementViewModel Get(Guid systemItemId, Guid elementSystemItemId)
        {
            var systemItem = GetSystemItem(elementSystemItemId, systemItemId);

            return _mapper.Map<BriefElementViewModel>(systemItem);
        }

        private SystemItem GetSystemItem(Guid systemItemId, Guid? parentSystemItemId)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);
            if (systemItem == null)
                throw new Exception(string.Format("The system item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");
            if (parentSystemItemId.HasValue && systemItem.ParentSystemItemId != parentSystemItemId)
                throw new Exception(string.Format("The system item with id '{0}' does not have a parent system item with id '{1}'.", systemItemId, parentSystemItemId));

            return systemItem;
        }
    }
}