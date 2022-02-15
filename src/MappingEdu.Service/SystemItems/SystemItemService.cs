// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.SystemItem;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using ItemChangeType = MappingEdu.Core.Domain.Enumerations.ItemChangeType;
using PreviousVersionViewModel = MappingEdu.Service.Model.SystemItem.PreviousVersionViewModel;
using SystemItemViewModel = MappingEdu.Service.Model.SystemItem.SystemItemViewModel;

namespace MappingEdu.Service.SystemItems
{
    public interface ISystemItemService
    {
        SystemItemViewModel Get(Guid systemItemId);

        SystemItemDetailViewModel GetDetail(Guid systemItemId, Guid? mappingProjectId = null, Guid? mappedSystemExtensionId = null);

        SystemItemDetailViewModel[] GetUsage(Guid systemItemId);

        SystemItemViewModel Post(SystemItemCreateModel model);

        SystemItemViewModel Put(Guid systemItemId, SystemItemEditModel model);

        void Delete(Guid systemItemId);

    }

    public class SystemItemService : ISystemItemService
    {
        private readonly IRepository<SystemItemCustomDetail> _customDetailRepository;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<SystemItemVersionDelta> _versionRepository;
        private readonly IRepository<SystemEnumerationItem> _systemEnumerationItemRepository;
        private readonly ISystemItemRepository _systemItemRepository;



        public SystemItemService(IRepository<SystemItemCustomDetail>  customDetailRepository, IMappedSystemRepository mappedSystemRepository, IMappingProjectRepository mappingProjectRepository, ISystemItemRepository systemItemRepository, IMapper mapper,
            IUserRepository userRepository, IRepository<SystemItemVersionDelta> versionRepository, IRepository<SystemEnumerationItem> systemEnumerationItemRepository)
        {
            _customDetailRepository = customDetailRepository;
            _mappedSystemRepository = mappedSystemRepository;
            _mappingProjectRepository = mappingProjectRepository;
            _mapper = mapper;
            _systemEnumerationItemRepository = systemEnumerationItemRepository;
            _systemItemRepository = systemItemRepository;
            _userRepository = userRepository;
            _versionRepository = versionRepository;
        }


        public SystemItemViewModel Get(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId);
            var model = new SystemItemViewModel
            {
                DataStandardId = systemItem.MappedSystemId,
                DataTypeSource = systemItem.DataTypeSource,
                Definition = systemItem.Definition,
                ExtensionShortName = (systemItem.MappedSystemExtensionId.HasValue) ? systemItem.MappedSystemExtension.ShortName : "",
                FieldLength = systemItem.FieldLength,
                ItemDataTypeId = systemItem.ItemDataTypeId,
                IsExtended = systemItem.IsExtended,
                ItemName = systemItem.ItemName,
                ItemTypeId = systemItem.ItemTypeId,
                ItemUrl = systemItem.ItemUrl,
                ParentSystemItemId = systemItem.ParentSystemItemId,
                PathSegments = GetPathSegments(systemItem),
                SystemItemCustomDetails = new List<SystemItemCustomDetailViewModel>(),
                SystemItemId = systemItem.SystemItemId,
                TechnicalName = systemItem.TechnicalName
            };
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
                    SystemItemId = systemItemId
                };

                if (systemItemCustomDetail != null)
                {
                    customDetailModel.Value = systemItemCustomDetail.Value;
                    customDetailModel.SystemItemCustomDetailId = systemItemCustomDetail.SystemItemCustomDetailId;
                }

                model.SystemItemCustomDetails.Add(customDetailModel);
            }
            return model;

        }

        public SystemItemDetailViewModel GetDetail(Guid systemItemId, Guid? mappingProjectId = null, Guid? mappedSystemExtensionId = null)
        {
            var systemItem = GetSystemItem(systemItemId);
            var mappedSystem = GetMappedSystem(systemItem.MappedSystemId);

            var systemItemDetail = new SystemItemDetailViewModel
            {
                DataStandardId = systemItem.MappedSystemId,
                DataTypeSource = systemItem.DataTypeSource,
                Definition = systemItem.Definition,
                ExtensionShortName = (systemItem.MappedSystemExtensionId.HasValue) ? systemItem.MappedSystemExtension.ShortName : "",
                FieldLength = systemItem.FieldLength,
                ItemDataTypeId = systemItem.ItemDataTypeId,
                IsExtended = systemItem.IsExtended,
                ItemName = systemItem.ItemName,
                ItemTypeId = systemItem.ItemTypeId,
                ItemUrl = systemItem.ItemUrl,
                ParentSystemItemId = systemItem.ParentSystemItemId,
                SystemItemId = systemItem.SystemItemId,
                TechnicalName = systemItem.TechnicalName,

                ChildSystemItems = systemItem.ChildSystemItems.Where(x => systemItem.MappedSystemExtensionId.HasValue || (!x.MappedSystemExtensionId.HasValue && x.MappedSystemExtensionId == mappedSystemExtensionId)).Select(x => new SystemItemViewWithSummaryModel
                {
                    DataStandardId = x.MappedSystemId,
                    DataTypeSource = x.DataTypeSource,
                    Definition = x.Definition,
                    ExtensionShortName = (x.MappedSystemExtensionId.HasValue) ? x.MappedSystemExtension.ShortName : "",
                    FieldLength = x.FieldLength,
                    ItemDataTypeId = x.ItemDataTypeId,
                    IsExtended = x.IsExtended,
                    ItemName = x.ItemName,
                    ItemTypeId = x.ItemTypeId,
                    ItemUrl = x.ItemUrl,
                    ParentSystemItemId = x.ParentSystemItemId,
                    SystemItemId = x.SystemItemId,
                    TechnicalName = x.TechnicalName,
                }).ToList(),

                EnumerationSystemItemId = systemItem.EnumerationSystemItemId,
                EnumerationSystemItem = systemItem.EnumerationSystemItemId.HasValue ? new SystemItemViewWithSummaryModel
                {
                    Definition = systemItem.EnumerationSystemItem.Definition,
                    ItemName = systemItem.EnumerationSystemItem.ItemName,
                    SystemItemId = systemItem.EnumerationSystemItemId.Value
                } : null,

                PathSegments = GetPathSegments(systemItem),

                SystemItemCustomDetails = new List<SystemItemCustomDetailViewModel>()
            };

            foreach (var metadata in mappedSystem.CustomDetailMetadata)
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
                    SystemItemId = systemItemId
                };

                if (systemItemCustomDetail != null)
                {
                    customDetailModel.Value = systemItemCustomDetail.Value;
                    customDetailModel.SystemItemCustomDetailId = systemItemCustomDetail.SystemItemCustomDetailId;
                }

                systemItemDetail.SystemItemCustomDetails.Add(customDetailModel);
            }

            if (mappingProjectId == null && systemItem.MappedSystem.HasAccess(MappedSystemUser.MappedSystemUserRole.View))
            {
                systemItemDetail.NextVersions = systemItem.NextSystemItemVersionDeltas.Select(nv => new NextVersionViewModel()
                {
                    NextVersionId = nv.SystemItemVersionDeltaId,
                    ItemChangeType = (ItemChangeType) nv.ItemChangeTypeId,
                    NewSystemItemId = nv.NewSystemItemId,
                    NewSystemItemPathSegments = (nv.NewSystemItem == null) ? new List<PathSegment> {new PathSegment {ItemName = "None"}} : GetPathSegments(nv.NewSystemItem),
                    Description = nv.Description,
                }).ToList();

                systemItemDetail.Notes = systemItem.Notes.Select(x => new NoteViewModel
                {
                    NoteId = x.NoteId,
                    Title = x.Title,
                    Notes = x.Notes,
                    IsEdited = x.CreateDate != x.UpdateDate,
                    CreateDate = x.CreateDate,
                    CreateBy = x.CreateById.HasValue ? GetUserName(x.CreateById) : null,
                    CreateById = x.CreateById
                }).ToList();

                systemItemDetail.PreviousVersions = systemItem.PreviousSystemItemVersionDeltas.Select(pv => new PreviousVersionViewModel
                {
                    PreviousVersionId = pv.SystemItemVersionDeltaId,
                    ItemChangeType = (ItemChangeType) pv.ItemChangeTypeId,
                    OldSystemItemId = pv.OldSystemItemId,
                    OldSystemItemPathSegments = (pv.OldSystemItem == null) ? new List<PathSegment> {new PathSegment {ItemName = "None"}} : GetPathSegments(pv.OldSystemItem),
                    Description = pv.Description,
                }).ToList();
            }
            else if(mappingProjectId.HasValue)
            {
                GetMappingProject(mappingProjectId.Value, MappingProjectUser.MappingProjectUserRole.View);
                var summaries = _mappingProjectRepository.GetSummary(mappingProjectId.Value, null, systemItemId);
                foreach (var summary in summaries)
                    systemItemDetail.ChildSystemItems.First(x => x.SystemItemId == summary.SystemItemId).Summary = summary;
            }

            return systemItemDetail;
        }

        public SystemItemDetailViewModel[] GetUsage(Guid systemItemId)
        {
            return _systemItemRepository.GetAllItems().Where(s => s.EnumerationSystemItemId == systemItemId).ToList()
                .Select(systemItem => new SystemItemDetailViewModel
                {
                    DataStandardId = systemItem.MappedSystemId,
                    DataTypeSource = systemItem.DataTypeSource,
                    Definition = systemItem.Definition,
                    FieldLength = systemItem.FieldLength,
                    ItemDataTypeId = systemItem.ItemDataTypeId,
                    IsExtended = systemItem.IsExtended,
                    ItemName = systemItem.ItemName,
                    ItemTypeId = systemItem.ItemTypeId,
                    ItemUrl = systemItem.ItemUrl,
                    ParentSystemItemId = systemItem.ParentSystemItemId,
                    SystemItemId = systemItem.SystemItemId,
                    TechnicalName = systemItem.TechnicalName,

                    PathSegments = GetPathSegments(systemItem)
                }).ToArray();
        }

        public SystemItemViewModel Post(SystemItemCreateModel model)
        {
            GetMappedSystem(model.DataStandardId, MappedSystemUser.MappedSystemUserRole.Edit);

            var systemItem = new SystemItem()
            {
                DataTypeSource = model.DataTypeSource,
                Definition = model.Definition,
                EnumerationSystemItemId = model.EnumerationSystemItemId,
                FieldLength = model.FieldLength,
                ItemDataTypeId = model.ItemDataTypeId,
                IsActive = true,
                IsExtended = model.IsExtended,
                ItemName = model.ItemName,
                ItemTypeId = model.ItemTypeId,
                ItemUrl = model.ItemUrl,
                MappedSystemId = model.DataStandardId,
                ParentSystemItemId = model.ParentSystemItemId,
                TechnicalName = model.TechnicalName
            };

            _systemItemRepository.Add(systemItem);
            _systemItemRepository.SaveChanges();

            return _mapper.Map<SystemItemViewModel>(systemItem);
        }

        public SystemItemViewModel Put(Guid systemItemId, SystemItemEditModel model)
        {
            var systemItem = GetSystemItem(model.SystemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            if (model.DataStandardId != systemItem.MappedSystemId)
                throw new SecurityException("Data Standard Id do not match");

            if (systemItemId != model.SystemItemId)
                throw new SecurityException("System Item Ids do not match");

            if (model.ParentSystemItemId != systemItem.ParentSystemItemId)
                throw new SecurityException("Parent System Item Ids do not match");

            if (model.ItemTypeId != systemItem.ItemTypeId)
                throw new SecurityException("Item Types do not match");

            systemItem.DataTypeSource = model.DataTypeSource;
            systemItem.Definition = model.Definition;
            systemItem.EnumerationSystemItemId = model.EnumerationSystemItemId;
            systemItem.FieldLength = model.FieldLength;
            systemItem.ItemDataTypeId = model.ItemDataTypeId;
            systemItem.IsExtended = model.IsExtended;
            systemItem.ItemName = model.ItemName;
            systemItem.ItemUrl = model.ItemUrl;
            systemItem.TechnicalName = model.TechnicalName;

            if(systemItem.IsExtended)
                MarkChildrenExtended(systemItem);

            _systemItemRepository.SaveChanges();
            return _mapper.Map<SystemItemViewModel>(systemItem);
        }

        public void Delete(Guid systemItemId)
        {
            DeleteChildren(systemItemId);
            _systemItemRepository.SaveChanges();
        }

        private void DeleteChildren(Guid systemItemId)
        {
            var systemItem = GetSystemItem(systemItemId, MappedSystemUser.MappedSystemUserRole.Edit);

            if (systemItem.EnumerationUsages.Count > 0)
                throw new BusinessException("Cannot delete this system item because it is currently used within the system.");
            if (systemItem.TargetSystemItemMaps.Count > 0 || systemItem.SourceSystemItemMaps.Count > 0)
                throw new BusinessException("Cannot delete this system item because it currently has mappings.");

            var copies = systemItem.SystemItemCopies.ToArray();
            foreach (var copy in copies) {
                copy.CopiedFromSystemItem = null;
                copy.CopiedFromSystemItemId = null;
            }

            var nextVersionDeltas = systemItem.NextSystemItemVersionDeltas.ToArray();
            foreach (var nextDelta in nextVersionDeltas)
                _versionRepository.Delete(nextDelta);

            var enumerations = systemItem.SystemEnumerationItems.ToArray();
            foreach (var enumeration in enumerations)
                _systemEnumerationItemRepository.Delete(enumeration);

            var itemCustomDetails = systemItem.SystemItemCustomDetails.ToArray();
            foreach (var itemCustomDetail in itemCustomDetails)
                _customDetailRepository.Delete(itemCustomDetail.SystemItemCustomDetailId);

            var childSystemItems = systemItem.ChildSystemItems.ToArray();
            foreach (var item in childSystemItems)
                Delete(item.SystemItemId);

            _systemItemRepository.Delete(systemItemId);
        }

        private MappedSystem GetMappedSystem(Guid mappedSystemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var mappedSystem = _mappedSystemRepository.Get(mappedSystemId);

            if (mappedSystem == null)
                throw new Exception(string.Format("Mapped System with id '{0}' does not exist.", mappedSystemId));

            if (!Principal.Current.IsAdministrator && !mappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            if (!mappedSystem.IsActive)
                throw new Exception(string.Format("Mapped System with id '{0}' is marked as deleted.", mappedSystemId));

            return mappedSystem;
        }


        private MappingProject GetMappingProject(Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);

            if (mappingProject == null)
                throw new Exception(string.Format("Mapped Project with id '{0}' does not exist.", mappingProjectId));

            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            if (!mappingProject.IsActive)
                throw new Exception(string.Format("Mapping Project with id '{0}' is marked as deleted.", mappingProjectId));

            return mappingProject;
        }

        private List<PathSegment> GetPathSegments(SystemItem item, bool excludeItem = false)
        {
            var parent = excludeItem ? item.ParentSystemItem : item;
            var segments = new List<PathSegment>();

            while (parent != null)
            {
                var segment = new PathSegment
                {
                    Definition = parent.Definition,
                    IsExtended = parent.IsExtended,
                    ItemName = parent.ItemName,
                    ItemTypeId = parent.ItemTypeId,
                    SystemItemId = parent.SystemItemId
                };
                if (parent.MappedSystemExtensionId.HasValue)
                {
                    segment.Extension = new MappedSystemExtensionModel
                    {
                        MappedSystemExtensionId = parent.MappedSystemExtensionId.Value,
                        ShortName = parent.MappedSystemExtension.ShortName
                    };
                }

                segments.Add(segment);

                parent = parent.ParentSystemItem;
            }

            segments.Reverse();
            return segments;
        }

        private SystemItem GetSystemItem(Guid systemItemId, MappedSystemUser.MappedSystemUserRole role = MappedSystemUser.MappedSystemUserRole.Guest)
        {
            var systemItem = _systemItemRepository.Get(systemItemId);

            if (systemItem == null)
                throw new Exception(string.Format("SystemItem with id '{0}' does not exist.", systemItemId));

            if (!Principal.Current.IsAdministrator && !systemItem.MappedSystem.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            if (!systemItem.IsActive)
                throw new Exception(string.Format("System Item with id '{0}' is marked as deleted.", systemItem));

            return systemItem;
        }

        private string GetUserName(Guid? createdById = null)
        {
            if (!createdById.HasValue) return null;

            var user = _userRepository.GetAllUsers().FirstOrDefault(x => x.Id == createdById.Value.ToString());
            if (user != null) return user.FirstName + " " + user.LastName;

            return null;
        }

        private void MarkChildrenExtended(SystemItem item) {
            foreach (var child in item.ChildSystemItems)
            {
                child.IsExtended = true;
                MarkChildrenExtended(child);
            }
        }
    }
}
