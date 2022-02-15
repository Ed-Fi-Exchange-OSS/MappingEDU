// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Services.CustomMigration;
using MappingEdu.Service.Model.Admin;

namespace MappingEdu.Service.Admin.ConfigurationCheck.Database
{
    public class CustomMigrationsApplied : IConfigurationCheck
    {
        private readonly ICustomMigrationService _customMigrationService;

        public CustomMigrationsApplied(ICustomMigrationService customMigrationService)
        {
            _customMigrationService = customMigrationService;
        }

        public ConfigurationStatus.ConfigurationCheck Check()
        {
            return new ConfigurationStatus.ConfigurationCheck
            {
                ConfigurationCheckType = ConfigurationStatus.ConfigurationCheckType.Database,
                Success = _customMigrationService.PendingMigrations.Length == 0,
                Title = "All custom migrations are applied"
            };
        }
    }
}