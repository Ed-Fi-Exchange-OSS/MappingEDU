// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.Configuration;
using MappingEdu.Service.Model.Admin;

namespace MappingEdu.Service.Admin.ConfigurationCheck.Database
{
    public class ApplicationUserHasCorrectDatabaseRole : IConfigurationCheck
    {
        private readonly IMappingEduConfiguration _configuration;
        private readonly IExecuteBooleanScalar _executeBooleanScalar;

        public ApplicationUserHasCorrectDatabaseRole(IMappingEduConfiguration configuration, IExecuteBooleanScalar executeBooleanScalar)
        {
            _configuration = configuration;
            _executeBooleanScalar = executeBooleanScalar;
        }

        public ConfigurationStatus.ConfigurationCheck Check()
        {
            var sql = "SELECT IS_ROLEMEMBER('MappingEdu_User')";
            var result = _executeBooleanScalar.Execute(_configuration.ProdConnectionString, sql);

            return new ConfigurationStatus.ConfigurationCheck
            {
                ConfigurationCheckType = ConfigurationStatus.ConfigurationCheckType.Database,
                Success = result,
                Title = "Application user has correct database role"
            };
        }
    }
}