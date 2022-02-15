// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Security;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.PreviousVersionDelta;

namespace MappingEdu.Service.SystemItems
{
    public interface IPreviousVersionDeltaService
    {
        PreviousVersionDeltaViewModel[] Get(Guid systemItemId);

        PreviousVersionDeltaViewModel Get(Guid systemItemId, Guid versionDeltaId);

        PreviousVersionDeltaViewModel Post(Guid systemItemId, PreviousVersionDeltaCreateModel previousVersionDelta);

        PreviousVersionDeltaViewModel Put(Guid systemItemId, Guid versionDeltaId, PreviousVersionDeltaEditModel previousVersionDelta);

        void Delete(Guid systemItemId, Guid versionDeltaId);
    }

    public class PreviousVersionDeltaService : IPreviousVersionDeltaService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItem> _systemItemRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;

        public PreviousVersionDeltaService(IRepository<SystemItemVersionDelta> versionRepository,
            IRepository<SystemItem> systemItemRepository,
            IMapper mapper)
        {
            _versionRepository = versionRepository;
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        public PreviousVersionDeltaViewModel[] Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);
            var viewModels = _mapper.Map<PreviousVersionDeltaViewModel[]>(systemItem.PreviousSystemItemVersionDeltas);
            viewModels.Do(x => x.OldMappedSystemId = systemItem.MappedSystem.PreviousMappedSystemId);
            return viewModels;
        }

        public PreviousVersionDeltaViewModel Get(Guid systemItemId, Guid versionDeltaId)
        {
            var versionDelta = GetSpecificVersionDelta(versionDeltaId, systemItemId);
            var viewModel = _mapper.Map<PreviousVersionDeltaViewModel>(versionDelta);
            viewModel.OldMappedSystemId = versionDelta.NewSystemItem.MappedSystem.PreviousMappedSystemId;
            return viewModel;
        }

        public PreviousVersionDeltaViewModel Post(Guid systemItemId, PreviousVersionDeltaCreateModel previousVersionDelta)
        {
            var systemItem = GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            var version = new SystemItemVersionDelta
            {
                OldSystemItemId = previousVersionDelta.OldSystemItemId,
                ItemChangeTypeId = previousVersionDelta.ItemChangeTypeId,
                Description = previousVersionDelta.Description
            };

            if (previousVersionDelta.OldSystemItemId.HasValue)
            {
                var oldSystemItem = GetSystemItem(previousVersionDelta.OldSystemItemId.Value);
                oldSystemItem.NextSystemItemVersionDeltas.Add(version);
            }

            systemItem.PreviousSystemItemVersionDeltas.Add(version);
            _versionRepository.SaveChanges();
            return Get(systemItemId, version.SystemItemVersionDeltaId);
        }

        public PreviousVersionDeltaViewModel Put(Guid systemItemId, Guid versionDeltaId, PreviousVersionDeltaEditModel previousVersionDelta)
        {
            var versionDelta = GetSpecificVersionDelta(versionDeltaId, systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);
            versionDelta.ItemChangeTypeId = previousVersionDelta.ItemChangeTypeId;
            versionDelta.Description = previousVersionDelta.Description;

            if (previousVersionDelta.OldSystemItemId.HasValue && versionDelta.OldSystemItemId != previousVersionDelta.OldSystemItemId)
            {
                var oldSystemItem = GetSystemItem(previousVersionDelta.OldSystemItemId.Value);
                oldSystemItem.NextSystemItemVersionDeltas.Add(versionDelta);
            }
            versionDelta.OldSystemItemId = previousVersionDelta.OldSystemItemId;

            _versionRepository.SaveChanges();
            return Get(systemItemId, versionDelta.SystemItemVersionDeltaId);
        }

        public void Delete(Guid systemItemId, Guid versionDeltaId)
        {
            if (!Principal.Current.IsAdministrator)
                GetSpecificVersionDelta(versionDeltaId, systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            _versionRepository.Delete(versionDeltaId);
            _versionRepository.SaveChanges();
        }

        private SystemItemVersionDelta GetSpecificVersionDelta(Guid versionId, Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var version = _versionRepository.Get(versionId);
            if (version == null)
                throw new Exception(string.Format("The version record with id '{0}' does not exist.", versionId));
            if (!Principal.Current.IsAdministrator && !version.NewSystemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            if (version.NewSystemItemId != systemItemId)
                throw new Exception(string.Format("The version record with id '{0}' does not have system item '{1}' as its new version.", versionId, systemItemId));

            return version;
        }

        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);
            if (systemItem == null)
                throw new Exception(string.Format("The system item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return systemItem;
        }
    }
}