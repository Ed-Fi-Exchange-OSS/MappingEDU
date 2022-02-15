// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Services;
using MappingEdu.Core.Services.Configuration;
using MappingEdu.Service.Admin.ConfigurationCheck;
using MappingEdu.Service.Model.Admin;

namespace MappingEdu.Service.Admin
{
    public interface IConfigurationStatusService
    {
        ConfigurationStatus Get();
    }

    public class ConfigurationStatusService : IConfigurationStatusService
    {
        private readonly IMappingEduConfiguration _configuration;
        private readonly IConfigurationCheck[] _configurationChecks;
        private readonly IDatabaseConnection _connection;

        public ConfigurationStatusService(IMappingEduConfiguration configuration, IDatabaseConnection connection, IConfigurationCheck[] configurationChecks)
        {
            _configuration = configuration;
            _connection = connection;
            _configurationChecks = configurationChecks;
        }

        public ConfigurationStatus Get()
        {
            var model = new ConfigurationStatus
            {
                DatabaseCatalog = _connection.Database,
                DatabaseServer = _connection.DataSource,
                GoogleAnalyticsId = _configuration.GoogleAnalyticsId,
                GoogleAnalyticsWebSite = _configuration.GoogleAnalyticsWebSite
            };

            model.ConfigurationChecks = _configurationChecks.Select(x => x.Check()).ToArray();

            return model;
        }
    }
}