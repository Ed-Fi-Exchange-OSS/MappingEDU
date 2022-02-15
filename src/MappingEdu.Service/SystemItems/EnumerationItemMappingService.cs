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
using MappingEdu.Service.Model.EnumerationItemMapping;

namespace MappingEdu.Service.SystemItems
{
    public interface IEnumerationItemMappingService
    {
        EnumerationItemMappingViewModel[] Get(Guid systemItemMapId);

        EnumerationItemMappingViewModel Get(Guid systemItemMapId, Guid enumerationItemMappingId);

        EnumerationItemMappingViewModel Post(Guid systemItemMapId, EnumerationItemMappingCreateModel model);

        EnumerationItemMappingViewModel Put(Guid systemItemMapId, Guid enumerationItemMappingId, EnumerationItemMappingEditModel model);

        void Delete(Guid systemItemMapId, Guid enumerationItemMappingId);
    }

    public class EnumerationItemMappingService : IEnumerationItemMappingService
    {
        private readonly IRepository<SystemEnumerationItemMap> _enumerationItemMappingRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<SystemEnumerationItem> _systemEnumerationItemRepository;
        private readonly IRepository<SystemItemMap> _systemItemMapRepository;

        public EnumerationItemMappingService(IRepository<SystemEnumerationItemMap> enumerationItemMappingRepository,
            IRepository<SystemEnumerationItem> systemEnumerationItemRepository,
            IRepository<SystemItemMap> systemItemMapRepository,
            IMapper mapper)
        {
            _enumerationItemMappingRepository = enumerationItemMappingRepository;
            _systemEnumerationItemRepository = systemEnumerationItemRepository;
            _systemItemMapRepository = systemItemMapRepository;
            _mapper = mapper;
        }

        public EnumerationItemMappingViewModel[] Get(Guid systemItemMapId)
        {
            var systemItemMap = GetSystemItemMap(systemItemMapId);
            var model = _mapper.Map<EnumerationItemMappingViewModel[]>(systemItemMap.SystemEnumerationItemMaps);
            return model;
        }

        public EnumerationItemMappingViewModel Get(Guid systemItemMapId, Guid enumerationItemMappingId)
        {
            var enumerationItemMap = GetEnumerationItemMap(systemItemMapId, enumerationItemMappingId);
            var model = _mapper.Map<EnumerationItemMappingViewModel>(enumerationItemMap);
            return model;
        }

        public EnumerationItemMappingViewModel Post(Guid systemItemMapId, EnumerationItemMappingCreateModel model)
        {
            var systemItemMap = GetSystemItemMap(systemItemMapId, MappingProjectUser.MappingProjectUserRole.Edit);
            var sourceSystemEnumerationItem = GetSystemEnumerationItem(model.SourceSystemEnumerationItemId);
            SystemEnumerationItem targetSystemEnumerationItem = null;

            if (model.TargetSystemEnumerationItemId.HasValue && Guid.Empty != model.TargetSystemEnumerationItemId.Value)
                targetSystemEnumerationItem = GetSystemEnumerationItem(model.TargetSystemEnumerationItemId.Value);

            var enumerationItemMap = new SystemEnumerationItemMap
            {
                SystemItemMapId = systemItemMapId,
                SystemItemMap = systemItemMap,
                SourceSystemEnumerationItemId = sourceSystemEnumerationItem.SystemEnumerationItemId,
                SourceSystemEnumerationItem = sourceSystemEnumerationItem,
                TargetSystemEnumerationItemId = targetSystemEnumerationItem != null ? model.TargetSystemEnumerationItemId : null,
                TargetSystemEnumerationItem = targetSystemEnumerationItem,
                DeferredMapping = model.DeferredMapping,
                EnumerationMappingStatusTypeId = model.EnumerationMappingStatusTypeId,
                EnumerationMappingStatusReasonTypeId = model.EnumerationMappingStatusReasonTypeId
            };
            systemItemMap.SystemEnumerationItemMaps.Add(enumerationItemMap);

            _enumerationItemMappingRepository.Add(enumerationItemMap);
            _enumerationItemMappingRepository.SaveChanges();

            return Get(systemItemMapId, enumerationItemMap.SystemEnumerationItemMapId);
        }

        public EnumerationItemMappingViewModel Put(Guid systemItemMapId, Guid enumerationItemMappingId, EnumerationItemMappingEditModel model)
        {
            var enumerationItemMap = GetEnumerationItemMap(systemItemMapId, enumerationItemMappingId, MappingProjectUser.MappingProjectUserRole.Edit);

            if (model.TargetSystemEnumerationItemId.HasValue && Guid.Empty != model.TargetSystemEnumerationItemId.Value)
            {
                enumerationItemMap.TargetSystemEnumerationItemId = model.TargetSystemEnumerationItemId.Value;
                enumerationItemMap.TargetSystemEnumerationItem = GetSystemEnumerationItem(model.TargetSystemEnumerationItemId.Value);
            }
            else
            {
                enumerationItemMap.TargetSystemEnumerationItemId = null;
                enumerationItemMap.TargetSystemEnumerationItem = null;
            }

            enumerationItemMap.DeferredMapping = model.DeferredMapping;
            enumerationItemMap.EnumerationMappingStatusTypeId = model.EnumerationMappingStatusTypeId;
            enumerationItemMap.EnumerationMappingStatusReasonTypeId = model.EnumerationMappingStatusReasonTypeId;

            _enumerationItemMappingRepository.SaveChanges();

            return Get(systemItemMapId, enumerationItemMap.SystemEnumerationItemMapId);
        }

        public void Delete(Guid systemItemMapId, Guid enumerationItemMappingId)
        {
            var enumerationItemMap = GetEnumerationItemMap(systemItemMapId, enumerationItemMappingId, MappingProjectUser.MappingProjectUserRole.Edit);
            _enumerationItemMappingRepository.Delete(enumerationItemMap);
            _enumerationItemMappingRepository.SaveChanges();
        }

        private SystemEnumerationItem GetSystemEnumerationItem(Guid sourceSystemEnumerationItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.View)
        {
            var systemEnumerationItem = _systemEnumerationItemRepository.Get(sourceSystemEnumerationItemId);
            if (null == systemEnumerationItem)
                throw new Exception(string.Format("System Enumeration Item with id '{0}' does not exist.", sourceSystemEnumerationItemId));
            if(!Principal.Current.IsAdministrator && !systemEnumerationItem.SystemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return systemEnumerationItem;
        }

        private SystemItemMap GetSystemItemMap(Guid systemItemMapId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var systemItemMap = _systemItemMapRepository.Get(systemItemMapId);
            if (null == systemItemMap)
                throw new Exception(string.Format("System item map with id '{0}' does not exist.", systemItemMapId));
            if (!Principal.Current.IsAdministrator && !systemItemMap.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return systemItemMap;
        }

        private SystemEnumerationItemMap GetEnumerationItemMap(Guid systemItemMapId, Guid enumerationItemMappingId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var enumerationItemMap = _enumerationItemMappingRepository.Get(enumerationItemMappingId);
            if (null == enumerationItemMap)
                throw new Exception(string.Format("Enumeration Item Map with id '{0}' does not exist.", enumerationItemMappingId));
            if (!Principal.Current.IsAdministrator && !enumerationItemMap.SystemItemMap.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            if (enumerationItemMap.SystemItemMapId != systemItemMapId)
                throw new Exception(
                    string.Format(
                        "Enumeration Item Map with id '{0}' does not have System Item Map id of '{1}'.", enumerationItemMappingId, systemItemMapId));
            return enumerationItemMap;
        }
    }
}