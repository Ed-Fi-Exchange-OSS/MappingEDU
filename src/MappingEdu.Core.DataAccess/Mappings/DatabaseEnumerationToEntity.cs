// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using MappingEdu.Core.Attributes;
using MappingEdu.Core.Domain.Enumerations;

[assembly: LoadAutoMapperProfiles]

namespace MappingEdu.Core.DataAccess.Mappings
{
    public class DatabaseEnumerationToEntity : Profile
    {
        protected override void Configure()
        {
            base.Configure();

            CreateMap<CompleteStatusType, Domain.CompleteStatusType>()
                .ForMember(dest => dest.CompleteStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.CompleteStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());

            CreateMap<EnumerationMappingStatusType, Domain.EnumerationMappingStatusType>()
                .ForMember(dest => dest.EnumerationMappingStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.EnumerationMappingStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemEnumerationItemMaps, opt => opt.Ignore());

            CreateMap<EnumerationMappingStatusReasonType, Domain.EnumerationMappingStatusReasonType>()
                .ForMember(dest => dest.EnumerationMappingStatusReasonTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.EnumerationMappingStatusReasonTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemEnumerationItemMaps, opt => opt.Ignore());

            CreateMap<ItemChangeType, Domain.ItemChangeType>()
                .ForMember(dest => dest.ItemChangeTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemChangeTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemVersionDeltas, opt => opt.Ignore());

            CreateMap<ItemDataType, Domain.ItemDataType>()
                .ForMember(dest => dest.ItemDataTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemDataTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItems, opt => opt.Ignore());

            CreateMap<ItemType, Domain.ItemType>()
                .ForMember(dest => dest.ItemTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItems, opt => opt.Ignore());

            CreateMap<MappingStatusType, Domain.MappingStatusType>()
                .ForMember(dest => dest.MappingStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.MappingStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());

            CreateMap<MappingStatusReasonType, Domain.MappingStatusReasonType>()
                .ForMember(dest => dest.MappingStatusReasonTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.MappingStatusReasonTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());

            CreateMap<ProjectStatusType, Domain.ProjectStatusType>()
                .ForMember(dest => dest.ProjectStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ProjectStatusTypeName, opt => opt.MapFrom(source => source.DatabaseText))
                .ForMember(dest => dest.MappingProjects, opt => opt.Ignore());

            CreateMap<WorkflowStatusType, Domain.WorkflowStatusType>()
                .ForMember(dest => dest.WorkflowStatusTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.WorkflowStatusTypeName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());

            CreateMap<MappingMethodType, Domain.MappingMethodType>()
                .ForMember(dest => dest.MappingMethodTypeId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.MappingMethodTypeName, opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.SystemItemMaps, opt => opt.Ignore());
        }
    }
}
