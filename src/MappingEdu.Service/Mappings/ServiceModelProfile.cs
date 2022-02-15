// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using AutoMapper;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Service.Model.BriefElement;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.EnumerationItem;
using MappingEdu.Service.Model.EnumerationItemMapping;
using MappingEdu.Service.Model.MapNote;
using MappingEdu.Service.Model.MappedSystem;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Model.Membership;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.Model.SystemItemDefinition;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.Model.SystemItemName;
using MappingEdu.Service.Model.SystemItemSelector;
using MappingEdu.Service.Model.SystemItemTree;
using MappingEdu.Service.Util;
using DomainViewModel = MappingEdu.Service.Model.Domain.DomainViewModel;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;
using MappedSystemViewModel = MappingEdu.Service.Model.SystemItemSelector.MappedSystemViewModel;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.Mappings
{
    public class ServiceModelProfile : Profile
    {
        private void AddBlankTargetEnumerationItem(SystemItemMappingViewModel viewModel)
        {
            if (!viewModel.TargetEnumerationItems.Any())
                return;
            var targetEnums = new TargetEnumerationItemViewModel[viewModel.TargetEnumerationItems.Length + 1];
            targetEnums[0] = new TargetEnumerationItemViewModel();
            Array.Copy(viewModel.TargetEnumerationItems, 0, targetEnums, 1, viewModel.TargetEnumerationItems.Length);
            viewModel.TargetEnumerationItems = targetEnums;
        }

        protected override void Configure()
        {
            base.Configure();

            CreateMap<ApplicationUser, OrganizationUserModel>();
            //CreateMap<ApplicationUser, UserModel>();

            CreateMap<CustomDetailMetadata, CustomDetailMetadataViewModel>();

            CreateMap<ItemType, Core.Domain.ItemType>()
                .ForMember(vm => vm.MappingProjectQueueFilters, opt => opt.Ignore());

            CreateMap<WorkflowStatusType, Core.Domain.WorkflowStatusType>()
                .ForMember(vm => vm.MappingProjectQueueFilters, opt => opt.Ignore());

            CreateMap<MappingMethodType, Core.Domain.MappingMethodType>()
                .ForMember(vm => vm.MappingProjectQueueFilters, opt => opt.Ignore());


            // mapped system

            CreateMap<MappedSystem, DataStandardViewModel>()
                .ForMember(vm => vm.DataStandardId, opt => opt.MapFrom(x => x.MappedSystemId))
                .ForMember(vm => vm.PreviousDataStandardId, opt => opt.MapFrom(x => x.PreviousMappedSystemId))
                .ForMember(vm => vm.PreviousDataStandard, opt => opt.MapFrom(x => x.PreviousVersionMappedSystem))
                .ForMember(vm => vm.NextDataStandard, opt => opt.UseValue(null))
                .ForMember(vm => vm.NextDataStandardId, opt => opt.UseValue(null))
                .ForMember(vm => vm.ClonedFromDataStandardId, opt => opt.MapFrom(x => x.ClonedFromMappedSystemId))
                .ForMember(vm => vm.ClonedFromDataStandard, opt => opt.MapFrom(x => x.ClonedFromMappedSystem))
                .ForMember(vm => vm.Clones, opt => opt.MapFrom(x => x.Clones.Where(c => c.IsActive)))
                .ForMember(vm => vm.Flagged, opt => opt.MapFrom(x => x.FlaggedBy.Select(u => u.Id).Contains((Principal.Current != null) ? Principal.Current.UserId : "0")))
                .ForMember(vm => vm.UpdateDate, opt => opt.MapFrom(x =>
                    (x.UpdateDate > x.UserUpdates.Select(u => u.UpdateDate).OrderByDescending(u => u).FirstOrDefault()
                    ? x.UpdateDate
                    : x.UserUpdates.Select(u => u.UpdateDate).OrderByDescending(u => u).FirstOrDefault())))
                .ForMember(vm => vm.UserUpdateDate, opt => opt.MapFrom(x => x.UserUpdates.SingleOrDefault(u => u.UserId == Principal.Current.UserId).UpdateDate))
                .ForMember(vm => vm.Role, opt => opt.MapFrom(x => x.Users.SingleOrDefault(u => u.UserId == Principal.Current.UserId).Role));

            CreateMap<MappedSystem, DataStandardCloningViewModel>()
                .ForMember(vm => vm.DataStandardId, opt => opt.MapFrom(x => x.MappedSystemId))
                .ForMember(vm => vm.PreviousDataStandardId, opt => opt.MapFrom(x => x.PreviousMappedSystemId))
                .ForMember(vm => vm.PreviousDataStandard, opt => opt.MapFrom(x => x.PreviousVersionMappedSystem))
                .ForMember(vm => vm.NextDataStandard, opt => opt.UseValue(null))
                .ForMember(vm => vm.NextDataStandardId, opt => opt.UseValue(null))
                .ForMember(vm => vm.ClonedFromDataStandardId, opt => opt.MapFrom(x => x.ClonedFromMappedSystemId))
                .ForMember(vm => vm.ClonedFromDataStandard, opt => opt.MapFrom(x => x.ClonedFromMappedSystem))
                .ForMember(vm => vm.Clones, opt => opt.MapFrom(x => x.Clones.Where(c => c.IsActive)))
                .ForMember(vm => vm.Flagged, opt => opt.MapFrom(x => x.FlaggedBy.Select(u => u.Id).Contains((Principal.Current != null) ? Principal.Current.UserId : "0")))
                .ForMember(vm => vm.UpdateDate, opt => opt.MapFrom(x =>
                    (x.UpdateDate > x.UserUpdates.Select(u => u.UpdateDate).OrderByDescending(u => u).FirstOrDefault()
                    ? x.UpdateDate
                    : x.UserUpdates.Select(u => u.UpdateDate).OrderByDescending(u => u).FirstOrDefault())))
                .ForMember(vm => vm.UserUpdateDate, opt => opt.MapFrom(x => x.UserUpdates.SingleOrDefault(u => u.UserId == Principal.Current.UserId).UpdateDate))
                .ForMember(vm => vm.Role, opt => opt.MapFrom(x => x.Users.SingleOrDefault(u => u.UserId == Principal.Current.UserId).Role))
                .ForMember(vm => vm.SimilarVersioning, opt => opt.UseValue(false));

            CreateMap<MappedSystem, MappedSystemViewModel>()
                .ForMember(vm => vm.Domains, opt => opt.MapFrom(x => x.SystemItems.Where(y => y.ItemType == ItemType.Domain && y.IsActive)));

            CreateMap<MappedSystem, DataStandardCreateModel>()
                .ForMember(vm => vm.PreviousDataStandardId, opt => opt.MapFrom(x => x.PreviousMappedSystemId));

            CreateMap<MappedSystemUser, MappedSystemUserModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(x => x.User.Id))
                .ForMember(vm => vm.Email, opt => opt.MapFrom(x => x.User.Email))
                .ForMember(vm => vm.FirstName, opt => opt.MapFrom(x => x.User.FirstName))
                .ForMember(vm => vm.LastName, opt => opt.MapFrom(x => x.User.LastName));

            // mapping project

            CreateMap<MappingProject, DataStandardMappingProjectsViewModel>();

            CreateMap<MappingProject, MappingProjectViewModel>()
                .ForMember(vm => vm.SourceDataStandardId, opt => opt.MapFrom(x => x.SourceDataStandardMappedSystemId))
                .ForMember(vm => vm.TargetDataStandardId, opt => opt.MapFrom(x => x.TargetDataStandardMappedSystemId))
                .ForMember(vm => vm.SourceDataStandard, opt => opt.MapFrom(x => x.SourceDataStandard))
                .ForMember(vm => vm.TargetDataStandard, opt => opt.MapFrom(x => x.TargetDataStandard))
                .ForMember(vm => vm.ProjectStatusTypeName, opt => opt.MapFrom(x => x.ProjectStatusType.Name))
                .ForMember(vm => vm.Flagged, opt => opt.MapFrom(x => x.FlaggedBy.Select(u => u.Id).Contains((Principal.Current != null) ? Principal.Current.UserId : "0")))
                .ForMember(vm => vm.UpdateDate, opt => opt.MapFrom(x =>
                    (x.UpdateDate > x.UserUpdates.Select(u => u.UpdateDate).OrderByDescending(u => u).FirstOrDefault()
                    ? x.UpdateDate
                    : x.UserUpdates.Select(u => u.UpdateDate).OrderByDescending(u => u).FirstOrDefault())))
                .ForMember(vm => vm.UserUpdateDate, opt => opt.MapFrom(x => x.UserUpdates.SingleOrDefault(u => u.UserId == Principal.Current.UserId).UpdateDate))
                .ForMember(vm => vm.Role, opt => opt.MapFrom(x => x.Users.SingleOrDefault(u => u.UserId == Principal.Current.UserId).Role))
                .ForMember(vm => vm.CreateBy, opt => opt.MapFrom(x => x.CreateBy));

            CreateMap<MappingProjectUser, MappingProjectUserModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(x => x.User.Id))
                .ForMember(vm => vm.Email, opt => opt.MapFrom(x => x.User.Email))
                .ForMember(vm => vm.FirstName, opt => opt.MapFrom(x => x.User.FirstName))
                .ForMember(vm => vm.LastName, opt => opt.MapFrom(x => x.User.LastName));

            // note

            CreateMap<MapNote, MapNoteViewModel>()
                .ForMember(vm => vm.IsEdited, opt => opt.MapFrom(x => x.CreateDate != x.UpdateDate));

            CreateMap<Note, NoteViewModel>()
                .ForMember(vm => vm.IsEdited, opt => opt.MapFrom(x => x.CreateDate != x.UpdateDate));

            // organization

            CreateMap<Organization, OrganizationModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(x => x.OrganizationId))
                .ForMember(vm => vm.Domains, opt => opt.MapFrom(x => x.Domains.Split(Configuration.Membership.OrganizationDomainDelimiter)));

            CreateMap<Organization, UserOrganizationModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(x => x.OrganizationId))
                .ForMember(vm => vm.Domains, opt => opt.MapFrom(x => x.Domains.Split(Configuration.Membership.OrganizationDomainDelimiter)));

            // user

            CreateMap<MappingProjectUser, UserMappingProjectModel>()
                .ForMember(vm => vm.Name, opt => opt.MapFrom(x => x.MappingProject.ProjectName))
                .ForMember(vm => vm.Description, opt => opt.MapFrom(x => x.MappingProject.Description))
                .ForMember(vm => vm.Role, opt => opt.MapFrom(x => x.Role))
                .ForMember(vm => vm.Id, opt => opt.MapFrom(x => x.MappingProjectId));

            CreateMap<MappedSystemUser, UserMappedSystemModel>()
                .ForMember(vm => vm.Name, opt => opt.MapFrom(x => x.MappedSystem.SystemName))
                .ForMember(vm => vm.Role, opt => opt.MapFrom(x => x.Role))
                .ForMember(vm => vm.Version, opt => opt.MapFrom(x => x.MappedSystem.SystemVersion))
                .ForMember(vm => vm.Id, opt => opt.MapFrom(x => x.MappedSystemId));

            // system item

            CreateMap<SystemEnumerationItem, TargetEnumerationItemViewModel>();

            CreateMap<SystemItem, DomainViewModel>();

            CreateMap<SystemItem, SystemItemTreeViewModel>()
                .ForMember(vm => vm.Children, opt => opt.MapFrom(x => x.ChildSystemItems))
                .ForMember(vm => vm.ItemTypeName, opt => opt.MapFrom(x => x.ItemType.Name));

            CreateMap<SystemItem, SystemItemNameViewModel>();

            CreateMap<SystemItem, SystemItemDefinitionViewModel>();

            CreateMap<SystemItem, ElementDetailsViewModel>()
                .ForMember(vm => vm.EnumerationSystemItemName, opt => opt.MapFrom(x => x.EnumerationSystemItem == null ? null : x.EnumerationSystemItem.ItemName))
                .ForMember(vm => vm.DomainItemPath, opt => opt.MapFrom(x => Utility.GetDomainItemPath(x)))
                .ForMember(vm => vm.FullItemPath, opt => opt.MapFrom(x => Utility.GetFullItemPath(x)))
                .ForMember(vm => vm.PathSegments, opt => opt.MapFrom(x => Utility.GetAllItemSegments(x, true)));

            CreateMap<SystemItem, ElementDetailsSearchViewModel>()
                .ForMember(vm => vm.EnumerationSystemItemName, opt => opt.MapFrom(x => x.EnumerationSystemItem == null ? null : x.EnumerationSystemItem.ItemName))
                .ForMember(vm => vm.DomainId, opt => opt.MapFrom(x => Utility.GetDomain(x).SystemItemId))
                .ForMember(vm => vm.DomainName, opt => opt.MapFrom(x => Utility.GetDomain(x).ItemName))
                .ForMember(vm => vm.FullItemPath, opt => opt.MapFrom(x => Utility.GetFullItemPath(x)))
                .ForMember(vm => vm.PathSegments, opt => opt.MapFrom(x => Utility.GetAllItemSegments(x, true)))
                .ForMember(vm => vm.DomainItemPath, opt => opt.MapFrom(x => Utility.GetDomainItemPath(x)))
                .ForMember(vm => vm.SpacedItemPath, opt => opt.MapFrom(x => Utility.GetItemPathSpaced(x)))
                .ForMember(vm => vm.ShortItemPath, opt => opt.MapFrom(x => Utility.GetShortItemPath(x)));

            CreateMap<SystemEnumerationItem, EnumerationItemViewModel>();

            CreateMap<SystemItem, Model.SystemItem.SystemItemViewModel>()
                .ForMember(vm => vm.DataStandardId, opt => opt.MapFrom(x => x.MappedSystemId))
                .ForMember(vm => vm.ExtensionShortName, opt => opt.MapFrom(x => x.MappedSystemExtensionId.HasValue ? x.MappedSystemExtension.ShortName : ""))
                .ForMember(vm => vm.PathSegments, opt => opt.Ignore())
                .ForMember(vm => vm.SystemItemCustomDetails, opt => opt.Ignore());


            CreateMap<SystemItem, BriefElementViewModel>()
                .ForMember(vm => vm.ElementDetails, opt => opt.MapFrom(x => x))
                .ForMember(vm => vm.SystemItemMappings, opt => opt.MapFrom(x => x.SourceSystemItemMaps));

            CreateMap<SystemItemVersionDelta, NextVersionDeltaViewModel>()
                .ForMember(vm => vm.NewDataStandard, opt => opt.MapFrom(x => x.NewSystemItem == null ? null : x.NewSystemItem.MappedSystem))
                .ForMember(vm => vm.NewMappedSystemId, opt => opt.Ignore());

            CreateMap<SystemItemVersionDelta, PreviousVersionDeltaViewModel>()
                .ForMember(vm => vm.OldDataStandard, opt => opt.MapFrom(x => x.OldSystemItem == null ? null : x.OldSystemItem.MappedSystem))
                .ForMember(vm => vm.OldMappedSystemId, opt => opt.Ignore());

            CreateMap<SystemItem, PreviousVersionDeltaViewModel.SystemItemNode>()
                .ForMember(vm => vm.ItemTypeName, opt => opt.MapFrom(x => x.ItemType.Name));

            CreateMap<SystemItem, NextVersionDeltaViewModel.SystemItemNode>()
                .ForMember(vm => vm.ItemTypeName, opt => opt.MapFrom(x => x.ItemType.Name));

            CreateMap<SystemItem, TargetEnumerationItemViewModel.SystemItemNode>();

            CreateMap<SystemItemMap, SystemItemMappingViewModel>()
                .ForMember(vm => vm.EnumerationItemMappings, opt => opt.MapFrom(m => m.SystemEnumerationItemMaps))
                .ForMember(vm => vm.TargetEnumerationItems, opt => opt.MapFrom(m => m.TargetSystemItems.SelectMany(x => x.SystemEnumerationItems)))
                .AfterMap((m, vm) => AddBlankTargetEnumerationItem(vm));

            CreateMap<SystemItemMap, SystemItemMappingBriefViewModel>();

            CreateMap<SystemItemCustomDetail, SystemItemCustomDetailViewModel>();

            CreateMap<SystemEnumerationItemMap, EnumerationItemMappingViewModel>()
                .ForMember(vm => vm.SourceCodeValue, opt => opt.MapFrom(x => x.SourceSystemEnumerationItem.CodeValue))
                .ForMember(vm => vm.TargetCodeValue, opt => opt.MapFrom(x => x.TargetSystemEnumerationItem == null ? null : x.TargetSystemEnumerationItem.CodeValue))
                .ForMember(vm => vm.TargetSystemItem, opt => opt.MapFrom(x => x.TargetSystemEnumerationItem == null ? null : x.TargetSystemEnumerationItem.SystemItem));
            CreateMap<SystemItem, EnumerationItemMappingViewModel.SystemItemNode>();

            CreateMap<SystemItem, Model.SystemItemSelector.DomainViewModel>()
                .ForMember(vm => vm.ItemTypeName, opt => opt.MapFrom(x => x.ItemType.Name))
                .ForMember(vm => vm.Entities, opt => opt.MapFrom(x => x.ChildSystemItems.Where(y => y.ItemType == ItemType.Entity && y.IsActive)))
                .ForMember(vm => vm.Enumerations, opt => opt.MapFrom(x => x.ChildSystemItems.Where(y => y.ItemType == ItemType.Enumeration && y.IsActive)));

            CreateMap<SystemItem, SystemItemViewModel>()
                .ForMember(vm => vm.ItemTypeName, opt => opt.MapFrom(x => x.ItemType.Name));

            Mapper.AssertConfigurationIsValid();
        }
    }
}