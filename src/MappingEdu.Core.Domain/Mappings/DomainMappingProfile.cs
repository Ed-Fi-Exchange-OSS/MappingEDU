// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;

namespace MappingEdu.Core.Domain.Mappings
{
    public class DomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            //base.Configure();

            CreateMap<Enumerations.CompleteStatusType, CompleteStatusType>()
                .ForMember(dest => dest.CompleteStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.CompleteStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());

            CreateMap<Enumerations.EnumerationMappingStatusType, EnumerationMappingStatusType>()
                .ForMember(dest => dest.EnumerationMappingStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.EnumerationMappingStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemEnumerationItemMaps, opt => opt.Ignore());

            CreateMap<Enumerations.EnumerationMappingStatusReasonType, EnumerationMappingStatusReasonType>()
                .ForMember(dest => dest.EnumerationMappingStatusReasonTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.EnumerationMappingStatusReasonTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemEnumerationItemMaps, opt => opt.Ignore());

            CreateMap<Enumerations.ItemChangeType, ItemChangeType>()
                .ForMember(dest => dest.ItemChangeTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemChangeTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemVersionDeltas, opt => opt.Ignore());

            CreateMap<Enumerations.ItemDataType, ItemDataType>()
                .ForMember(dest => dest.ItemDataTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemDataTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItems, opt => opt.Ignore());

            CreateMap<Enumerations.ItemType, ItemType>()
                .ForMember(dest => dest.ItemTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItems, opt => opt.Ignore())
                .ForMember(dest => dest.MappingProjectQueueFilters, opt => opt.Ignore());

            CreateMap<Enumerations.MappingStatusType, MappingStatusType>()
                .ForMember(dest => dest.MappingStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.MappingStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());

            CreateMap<Enumerations.MappingStatusReasonType, MappingStatusReasonType>()
                .ForMember(dest => dest.MappingStatusReasonTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.MappingStatusReasonTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());

            CreateMap<Enumerations.ProjectStatusType, ProjectStatusType>()
                .ForMember(dest => dest.ProjectStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ProjectStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.MappingProjects, opt => opt.Ignore());

            CreateMap<Enumerations.WorkflowStatusType, WorkflowStatusType>()
                .ForMember(dest => dest.WorkflowStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.WorkflowStatusTypeName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore())
                .ForMember(dest => dest.MappingProjectQueueFilters, opt => opt.Ignore());

            CreateMap<Enumerations.MappingMethodType, MappingMethodType>()
                .ForMember(dest => dest.MappingMethodTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.MappingMethodTypeName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore())
                .ForMember(dest => dest.MappingProjectQueueFilters, opt => opt.Ignore());

            CreateMap<Enumerations.AutoMappingReasonType, AutoMappingReasonType>()
                .ForMember(dest => dest.AutoMappingReasonTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.AutoMappingReasonTypeName, opt => opt.MapFrom(source => source.Name));

            CreateMap<Enumerations.SystemConstantType, SystemConstantType>()
                .ForMember(dest => dest.SystemConstantTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.SystemConstantTypeName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.SystemConstants, opt => opt.Ignore());
        }
    }
}