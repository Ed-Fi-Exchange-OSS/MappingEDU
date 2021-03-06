// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Services.Configuration;
using MappingEdu.Service.Model.Admin;

namespace MappingEdu.Service.Admin.ConfigurationCheck.Database
{
    public class AllConnectionStringReferenceSameDatabaseCatalog : IConfigurationCheck
    {
        private readonly IMappingEduConfiguration _configuration;

        public AllConnectionStringReferenceSameDatabaseCatalog(IMappingEduConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ConfigurationStatus.ConfigurationCheck Check()
        {
            const string initialCatalog = "initial catalog";
            const char connectionStringSplitter = ';';

            var adminConnection = _configuration.AdminConnectionString.Split(connectionStringSplitter);
            var prodConnection = _configuration.ProdConnectionString.Split(connectionStringSplitter);

            var adminInitialCatalog = adminConnection.FirstOrDefault(x => x.StartsWith(initialCatalog));
            var prodInitialCatalog = prodConnection.FirstOrDefault(x => x.StartsWith(initialCatalog));

            var result = adminInitialCatalog == prodInitialCatalog;

            return new ConfigurationStatus.ConfigurationCheck
            {
                ConfigurationCheckType = ConfigurationStatus.ConfigurationCheckType.Database,
                Success = result,
                Title = "All connection strings reference same database catalog"
            };
        }
    }
}