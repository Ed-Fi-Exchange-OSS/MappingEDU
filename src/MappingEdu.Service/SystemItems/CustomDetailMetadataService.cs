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
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemCustomDetail;

namespace MappingEdu.Service.SystemItems
{
    public interface ICustomDetailMetadataService
    {
        CustomDetailMetadataViewModel[] Get(Guid mappedSystemId);

        CustomDetailMetadataViewModel Get(Guid mappedSystemId, Guid customDetailMetadataId);

        CustomDetailMetadataViewModel Post(Guid mappedSystemId, CustomDetailMetadataCreateModel model);

        CustomDetailMetadataViewModel Put(Guid mappedSystemId, Guid customDetailMetadataId, CustomDetailMetadataEditModel model);

        void Delete(Guid mappedSystemId, Guid customDetailMetadataId);
    }

    public class CustomDetailMetadataService : ICustomDetailMetadataService
    {
        private readonly IRepository<CustomDetailMetadata> _customDetailMetadataRepository;
        private readonly IRepository<SystemItemCustomDetail> _systemItemCustomDetailRepository;
        private readonly IRepository<MappedSystem> _mappedSystemRepository;
        private readonly IMapper _mapper;

        public CustomDetailMetadataService(
            IRepository<CustomDetailMetadata> customDetailMetadataRepository, IRepository<MappedSystem> mappedSystemRepository, IMapper mapper, IRepository<SystemItemCustomDetail> systemItemCustomDetailRepository)
        {
            _customDetailMetadataRepository = customDetailMetadataRepository;
            _systemItemCustomDetailRepository = systemItemCustomDetailRepository;
            _mappedSystemRepository = mappedSystemRepository;
            _mapper = mapper;
        }

        public CustomDetailMetadataViewModel[] Get(Guid mappedSystemId)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId);
            var customDetailMetadataModel = _mapper.Map<CustomDetailMetadataViewModel[]>(mappedSystem.CustomDetailMetadata);
            return customDetailMetadataModel;
        }

        public CustomDetailMetadataViewModel Get(Guid mappedSystemId, Guid customDetailMetadataId)
        {
            var customDetailMetadata = GetCustomDetailMetadata(mappedSystemId, customDetailMetadataId);
            var customDetailMetadataModel = _mapper.Map<CustomDetailMetadataViewModel>(customDetailMetadata);
            return customDetailMetadataModel;
        }

        public CustomDetailMetadataViewModel Post(Guid mappedSystemId, CustomDetailMetadataCreateModel model)
        {
            var mappedSystem = GetMappedSystem(mappedSystemId, MappedSystemUser.MappedSystemUserRole.Edit);
            var customDetailMetadata = new CustomDetailMetadata
            {
                DisplayName = model.DisplayName,
                IsBoolean = model.IsBoolean,
                IsCoreDetail = model.IsCoreDetail,
                MappedSystemId = mappedSystemId,
                MappedSystem = mappedSystem
            };

            _customDetailMetadataRepository.Add(customDetailMetadata);
            _customDetailMetadataRepository.SaveChanges();

            return Get(mappedSystemId, customDetailMetadata.CustomDetailMetadataId);
        }

        public CustomDetailMetadataViewModel Put(Guid mappedSystemId, Guid customDetailMetadataId, CustomDetailMetadataEditModel model)
        {
            var customDetailMetadata = GetCustomDetailMetadata(mappedSystemId, customDetailMetadataId, MappedSystemUser.MappedSystemUserRole.Edit);
            customDetailMetadata.DisplayName = model.DisplayName;
            customDetailMetadata.IsBoolean = model.IsBoolean;
            customDetailMetadata.IsCoreDetail = model.IsCoreDetail;

            _customDetailMetadataRepository.SaveChanges();

            return Get(mappedSystemId, customDetailMetadataId);
        }

        public void Delete(Guid mappedSystemId, Guid customDetailMetadataId)
        {
            var customDetailMetadata = GetCustomDetailMetadata(mappedSystemId, customDetailMetadataId, MappedSystemUser.MappedSystemUserRole.Edit);

            var systemItemCustomDetails = customDetailMetadata.SystemItemCustomDetails.ToList();
            foreach (var systemItemCustomDetail in systemItemCustomDetails)
                _systemItemCustomDetailRepository.Delete(systemItemCustomDetail);

            _customDetailMetadataRepository.Delete(customDetailMetadata);
            _customDetailMetadataRepository.SaveChanges();
        }

        private CustomDetailMetadata GetCustomDetailMetadata(Guid mappedSystemId, Guid customDetailMetadataId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var customDetailMetadata = _customDetailMetadataRepository.Get(customDetailMetadataId);

            if (null == customDetailMetadata)
                throw new Exception(string.Format("The custom detail metadata with id '{0}' does not exist.", customDetailMetadataId));
            if (customDetailMetadata.MappedSystemId != mappedSystemId)
                throw new Exception(
                    string.Format("The custom detail metadata with id '{0}' does not have a mapped system id of '{1}'.", customDetailMetadataId, mappedSystemId));
            if(!Principal.Current.IsAdministrator && !customDetailMetadata.MappedSystem.HasAccess(role))
                throw new SecurityException("User does not have access to custom meta details");

            return customDetailMetadata;
        }

        private MappedSystem GetMappedSystem(Guid mappedSystemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);
            if (null == mappedSystem)
                throw new Exception(string.Format("The mapped system with id '{0}' does not exist.", mappedSystemId));

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return mappedSystem;
        }
    }
}