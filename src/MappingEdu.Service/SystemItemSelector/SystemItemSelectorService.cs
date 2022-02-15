// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappedSystem;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.SystemItemSelector
{
    public interface ISystemItemSelectorService
    {
        MappedSystemViewModel[] Get();

        MappedSystemViewModel Get(Guid mappedSystemId);
    }

    public class SystemItemSelectorService : ISystemItemSelectorService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<MappedSystem> _repository;
        private readonly ISystemItemRepository _systemItemRepository;

        public SystemItemSelectorService(IRepository<MappedSystem> repository, ISystemItemRepository systemItemRepository, IMapper mapper)
        {
            _repository = repository;
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        public MappedSystemViewModel[] Get()
        {
            var mappedSystems = _repository.GetAll().Where(x => x.IsActive);
            foreach (var mappedSystem in mappedSystems)
                foreach (var domain in mappedSystem.SystemItems.Where(x => x.ItemType == ItemType.Domain && x.IsActive))
                    _systemItemRepository.GetWithTreeLoaded(mappedSystem.MappedSystemId, domain.SystemItemId);
            return _mapper.Map<MappedSystemViewModel[]>(mappedSystems);
        }

        public MappedSystemViewModel Get(Guid mappedSystemId)
        {
            var mappedSystem = _repository.Get(mappedSystemId);

            foreach (var domain in mappedSystem.SystemItems.Where(x => x.ItemType == ItemType.Domain && x.IsActive))
                _systemItemRepository.GetWithTreeLoaded(mappedSystemId, domain.SystemItemId);

            return _mapper.Map<MappedSystemViewModel>(mappedSystem);
        }
    }
}