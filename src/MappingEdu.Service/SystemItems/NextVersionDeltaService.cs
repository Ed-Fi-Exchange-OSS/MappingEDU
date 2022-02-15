// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.NextVersionDelta;

namespace MappingEdu.Service.SystemItems
{
    public interface INextVersionDeltaService
    {
        NextVersionDeltaViewModel[] Get(Guid systemItemId);

        NextVersionDeltaViewModel Get(Guid systemItemId, Guid versionDeltaId);

        NextVersionDeltaViewModel Post(Guid systemItemId, NextVersionDeltaCreateModel nextVersionDelta);

        NextVersionDeltaViewModel Put(Guid systemItemId, Guid versionDeltaId, NextVersionDeltaEditModel nextVersionDelta);

        void Delete(Guid systemItemId, Guid versionDeltaId);
    }

    public class NextVersionDeltaService : INextVersionDeltaService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItem> _systemItemRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;

        public NextVersionDeltaService(IRepository<SystemItemVersionDelta> versionRepository,
            IRepository<SystemItem> systemItemRepository,
            IMapper mapper)
        {
            _versionRepository = versionRepository;
            _systemItemRepository = systemItemRepository;
            _mapper = mapper;
        }

        public NextVersionDeltaViewModel[] Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);
            var viewModels = _mapper.Map<NextVersionDeltaViewModel[]>(systemItem.NextSystemItemVersionDeltas);
            var nextMappedSystem = systemItem.MappedSystem.NextVersionMappedSystems.FirstOrDefault();
            if (nextMappedSystem != null)
                viewModels.Do(x => x.NewMappedSystemId = nextMappedSystem.MappedSystemId);
            return viewModels;
        }

        public NextVersionDeltaViewModel Get(Guid systemItemId, Guid versionDeltaId)
        {
            var versionDelta = GetSpecificVersionDelta(versionDeltaId, systemItemId);
            var viewModel = _mapper.Map<NextVersionDeltaViewModel>(versionDelta);
            var nextMappedSystem = versionDelta.OldSystemItem.MappedSystem.NextVersionMappedSystems.FirstOrDefault();
            if (nextMappedSystem != null)
                viewModel.NewMappedSystemId = nextMappedSystem.MappedSystemId;
            return viewModel;
        }

        public NextVersionDeltaViewModel Post(Guid systemItemId, NextVersionDeltaCreateModel nextVersionDelta)
        {
            var systemItem = GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            var version = new SystemItemVersionDelta
            {
                NewSystemItemId = nextVersionDelta.NewSystemItemId,
                OldSystemItemId = systemItemId,
                ItemChangeTypeId = nextVersionDelta.ItemChangeTypeId,
                Description = nextVersionDelta.Description
            };

            if (nextVersionDelta.NewSystemItemId.HasValue)
            {
                var newSystemItem = GetSystemItem(nextVersionDelta.NewSystemItemId.Value);
                newSystemItem.PreviousSystemItemVersionDeltas.Add(version);
            }
            systemItem.NextSystemItemVersionDeltas.Add(version);
            _versionRepository.SaveChanges();
            return Get(systemItemId, version.SystemItemVersionDeltaId);
        }

        public NextVersionDeltaViewModel Put(Guid systemItemId, Guid versionDeltaId, NextVersionDeltaEditModel nextVersionDelta)
        {
            var versionDelta = GetSpecificVersionDelta(versionDeltaId, systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);
            versionDelta.ItemChangeTypeId = nextVersionDelta.ItemChangeTypeId;
            versionDelta.Description = nextVersionDelta.Description;

            if (nextVersionDelta.NewSystemItemId.HasValue && versionDelta.NewSystemItemId != nextVersionDelta.NewSystemItemId)
            {
                var nextSystemItem = GetSystemItem(nextVersionDelta.NewSystemItemId.Value);
                nextSystemItem.PreviousSystemItemVersionDeltas.Add(versionDelta);
            }
            versionDelta.NewSystemItemId = nextVersionDelta.NewSystemItemId;

            _versionRepository.SaveChanges();
            return Get(systemItemId, versionDelta.SystemItemVersionDeltaId);
        }

        public void Delete(Guid systemItemId, Guid versionId)
        {
            if (!Principal.Current.IsAdministrator)
                GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            _versionRepository.Delete(versionId);
            _versionRepository.SaveChanges();
        }

        private SystemItemVersionDelta GetSpecificVersionDelta(Guid versionId, Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var version = _versionRepository.Get(versionId);
            if (version == null)
                throw new Exception(string.Format("The version record with id '{0}' does not exist.", versionId));
            if (!Principal.Current.IsAdministrator && !version.OldSystemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            if (version.OldSystemItemId != systemItemId)
                throw new Exception(string.Format("The version record with id '{0}' does not have system item '{1}' as its old version.", versionId, systemItemId));

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