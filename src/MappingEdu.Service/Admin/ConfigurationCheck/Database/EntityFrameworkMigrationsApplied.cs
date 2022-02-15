// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Services;
using MappingEdu.Service.Model.Admin;

namespace MappingEdu.Service.Admin.ConfigurationCheck.Database
{
    public class EntityFrameworkMigrationsApplied : IConfigurationCheck
    {
        private readonly IDatabaseMigrator _migrator;

        public EntityFrameworkMigrationsApplied(IDatabaseMigrator migrator)
        {
            _migrator = migrator;
        }

        public ConfigurationStatus.ConfigurationCheck Check()
        {
            return new ConfigurationStatus.ConfigurationCheck
            {
                ConfigurationCheckType = ConfigurationStatus.ConfigurationCheckType.Database,
                Success = _migrator.PendingMigrations.Length == 0,
                Title = "All entity framework migrations are applied"
            };
        }
    }
}