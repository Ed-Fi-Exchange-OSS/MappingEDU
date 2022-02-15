// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.DataStandard;

namespace MappingEdu.Service.MappedSystems
{
    public interface INextDataStandardService
    {
        DataStandardViewModel Get(Guid mappedSystemId);
    }

    public class NextDataStandardService : INextDataStandardService
    {
        private readonly IRepository<MappedSystem> _mappedSystemRepository;
        private readonly IMapper _mapper;

        public NextDataStandardService(IRepository<MappedSystem> mappedSystemRepository, IMapper mapper)
        {
            _mappedSystemRepository = mappedSystemRepository;
            _mapper = mapper;
        }

        public DataStandardViewModel Get(Guid mappedSystemId)
        {
            var mappedSystem = GetNextMappedSystem(mappedSystemId);
            var viewModel = _mapper.Map<DataStandardViewModel>(mappedSystem);
            return viewModel;
        }

        private MappedSystem GetNextMappedSystem(Guid mappedSystemId)
        {
            var mappedSystem = _mappedSystemRepository.GetAll()
                .FirstOrDefault(m => m.PreviousMappedSystemId != null && m.IsActive && m.PreviousMappedSystemId.Equals(mappedSystemId));

            return mappedSystem;
        }
    }
}