// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
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
    public interface ISystemItemCustomDetailService
    {
        SystemItemCustomDetailViewModel[] Get(Guid systemItemId);

        SystemItemCustomDetailViewModel Get(Guid systemItemId, Guid systemItemCustomDetailId);

        SystemItemCustomDetailViewModel Post(Guid systemItemId, SystemItemCustomDetailCreateModel model);

        SystemItemCustomDetailViewModel Put(Guid systemItemId, SystemItemCustomDetailEditModel model);

        void Delete(Guid systemItemId, Guid systemItemCustomDetailId);
    }

    public class SystemItemCustomDetailService : ISystemItemCustomDetailService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItemCustomDetail> _systemItemCustomDetailRepository;
        private readonly IRepository<CustomDetailMetadata> _customDetailMetadataRepository;
        private readonly IRepository<SystemItem> _systemItemRepository;

        public SystemItemCustomDetailService(
            IRepository<SystemItemCustomDetail> systemItemCustomDetailRepository, IRepository<SystemItem> systemItemRepository, IMapper mapper, IRepository<CustomDetailMetadata> customDetailMetadataRepository )
        {
            _systemItemCustomDetailRepository = systemItemCustomDetailRepository;
            _systemItemRepository = systemItemRepository;
            _customDetailMetadataRepository = customDetailMetadataRepository;
            _mapper = mapper;
        }

        public SystemItemCustomDetailViewModel[] Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);

            var systemItemCustomDetails = new List<SystemItemCustomDetailViewModel>();
            foreach (var metadata in systemItem.MappedSystem.CustomDetailMetadata)
            {
                var systemItemCustomDetail = systemItem.SystemItemCustomDetails.FirstOrDefault(x => x.CustomDetailMetadataId == metadata.CustomDetailMetadataId);
                var customDetailModel = new SystemItemCustomDetailViewModel
                {
                    CustomDetailMetadataId = metadata.CustomDetailMetadataId,
                    CustomDetailMetadata = new CustomDetailMetadataViewModel()
                    {
                        IsBoolean = metadata.IsBoolean,
                        IsCoreDetail = metadata.IsCoreDetail,
                        CustomDetailMetadataId = metadata.CustomDetailMetadataId,
                        DisplayName = metadata.DisplayName,
                        MappedSystemId = metadata.MappedSystemId
                    },
                    SystemItemId = systemItemId,
                };

                if (systemItemCustomDetail != null)
                {
                    customDetailModel.Value = systemItemCustomDetail.Value;
                    customDetailModel.SystemItemCustomDetailId = systemItemCustomDetail.SystemItemCustomDetailId;
                }

                systemItemCustomDetails.Add(customDetailModel);
            }

            return systemItemCustomDetails.ToArray();

        }

        public SystemItemCustomDetailViewModel Get(Guid systemItemId, Guid systemItemCustomDetailId)
        {
            var systemItemCustomDetail = GetSystemItemCustomDetail(systemItemId, systemItemCustomDetailId);
            var systemItemCustomDetailViewModel = _mapper.Map<SystemItemCustomDetailViewModel>(systemItemCustomDetail);
            return systemItemCustomDetailViewModel;
        }

        public SystemItemCustomDetailViewModel Post(Guid systemItemId, SystemItemCustomDetailCreateModel model)
        {
            var systemItem = GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);
            var customDetailMetadata = GetCustomDetailMetadata(model.CustomDetailMetadataId);
            var systemItemCustomDetail = new SystemItemCustomDetail
            {
                CustomDetailMetadata = customDetailMetadata,
                CustomDetailMetadataId = model.CustomDetailMetadataId,
                SystemItemId = systemItemId,
                SystemItem = systemItem,
                Value = model.Value
            };

            _systemItemCustomDetailRepository.Add(systemItemCustomDetail);
            _systemItemCustomDetailRepository.SaveChanges();

            return Get(systemItemId, systemItemCustomDetail.SystemItemCustomDetailId);
        }

        public SystemItemCustomDetailViewModel Put(Guid systemItemId, SystemItemCustomDetailEditModel model)
        {
            var systemItemCustomDetail = GetSystemItemCustomDetail(systemItemId, model.SystemItemCustomDetailId, MappedSystemUser.MappedSystemUserRole.Edit);
            systemItemCustomDetail.Value = string.IsNullOrWhiteSpace(model.Value) ? null : model.Value;

            _systemItemCustomDetailRepository.SaveChanges();

            return Get(systemItemId, model.SystemItemCustomDetailId);
        }

        public void Delete(Guid systemItemId, Guid systemItemCustomDetailId)
        {
            var systemItemCustomDetail = GetSystemItemCustomDetail(systemItemId, systemItemCustomDetailId, MappedSystemUser.MappedSystemUserRole.Edit);
            _systemItemCustomDetailRepository.Delete(systemItemCustomDetail);
            _systemItemCustomDetailRepository.SaveChanges();
        }

        private SystemItemCustomDetail GetSystemItemCustomDetail(Guid systemItemId, Guid systemItemCustomDetailId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItemCustomDetail = _systemItemCustomDetailRepository.Get(systemItemCustomDetailId);

            if (null == systemItemCustomDetail)
                throw new Exception(string.Format("The system item custom detail with id '{0}' does not exist.", systemItemCustomDetailId));
            if (systemItemCustomDetail.SystemItemId != systemItemId)
                throw new Exception(
                    string.Format("The system item custom detail with id '{0}' does not have a system item id of '{1}'.", systemItemCustomDetailId, systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItemCustomDetail.SystemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return systemItemCustomDetail;
        }

        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);
            if (null == systemItem)
                throw new Exception(string.Format("The system item with id '{0}' does not exist.", systemItemId));
            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return systemItem;
        }


        private CustomDetailMetadata GetCustomDetailMetadata(Guid customDetailMetadataId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var customDetailMetadata = _customDetailMetadataRepository.Get(customDetailMetadataId);
            if (null == customDetailMetadata)
                throw new Exception(string.Format("The custom detail metadata with id '{0}' does not exist.", customDetailMetadataId));
            if (!Principal.Current.IsAdministrator && !customDetailMetadata.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return customDetailMetadata;
        }
    }
}