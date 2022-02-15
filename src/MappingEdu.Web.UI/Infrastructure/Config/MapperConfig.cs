// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using MappingEdu.Core.Domain.Mappings;
using MappingEdu.Service.Mappings;

namespace MappingEdu.Web.UI.Infrastructure.Config
{
    /// <summary>
    ///     Configures auto mapper
    /// </summary>
    public class MapperConfig
    {
        private static bool _isStarting;

        /// <summary>
        ///     Initializes the configuration
        /// </summary>
        public static void Initialize()
        {
            if (_isStarting) return;

            _isStarting = true;

            Mapper.AddProfile(new DomainMappingProfile());
            Mapper.AddProfile(new ServiceModelProfile());

            Mapper.AssertConfigurationIsValid();
        }
    }
}