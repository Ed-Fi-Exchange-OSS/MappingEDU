// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Mapping
{
    public class Mapper : IMapper
    {
        public TDestination Map<TDestination>(object source)
        {
            return AutoMapper.Mapper.Map<TDestination>(source);
        }

        public TDestination DynamicMap<TDestination>(object source)
        {
            return AutoMapper.Mapper.DynamicMap<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return AutoMapper.Mapper.Map(source, destination);
        }

        public void CreateMap<TSource, TDestination>()
        {
            AutoMapper.Mapper.CreateMap<TSource, TDestination>();
        }
    }
}