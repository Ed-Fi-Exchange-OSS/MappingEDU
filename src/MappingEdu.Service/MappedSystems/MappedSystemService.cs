// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Security;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Domains;
using MappingEdu.Service.Model.MappedSystem;

namespace MappingEdu.Service.MappedSystems
{
    public interface IMappedSystemService
    {
        MappedSystemViewModel[] Get();

        MappedSystemViewModel Get(Guid id);

        MappedSystemViewModel Post(MappedSystemCreateModel model);

        Stream Export(Guid id);
        
    }

    public class MappedSystemService : IMappedSystemService
    {
        private readonly IDataStandardService _detailsService;
        private readonly IDomainService _domainService;
        private readonly IMappedSystemRepository _repository;

        public MappedSystemService(IDataStandardService detailsService, IDomainService domainService, IMappedSystemRepository repository)
        {
            _detailsService = detailsService;
            _domainService = domainService;
            _repository = repository;
        }

        public MappedSystemViewModel[] Get()
        {
            var mappedSystems = _repository.GetAll();
            return mappedSystems.Select(x => Get(x.MappedSystemId)).ToArray();
        }

        public MappedSystemViewModel Get(Guid id)
        {
            if (!Principal.Current.IsAdministrator && !_repository.Get(id).HasAccess())
                throw new SecurityException("User needs at least Guest Access to perform this action");

            var viewModel = new MappedSystemViewModel
            {
                MappedSystemId = id,
                DataStandard = _detailsService.Get(id),
                Domains = _domainService.Get(id)
            };
            return viewModel;
        }

        public MappedSystemViewModel Post(MappedSystemCreateModel model)
        {
            if(Principal.Current.IsGuest)
                throw new SecurityException("Guests cannot create standards");

            var mappedSystem = new MappedSystem
            {
                SystemName = model.MappedSystemDetails.SystemName,
                SystemVersion = model.MappedSystemDetails.SystemVersion,
                IsActive = true
            };
            _repository.Add(mappedSystem);
            _repository.SaveChanges();

            return Get(mappedSystem.MappedSystemId);
        }

        public Stream Export(Guid id) {
            throw new NotImplementedException();
        }
    }
}