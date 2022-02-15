// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Services.Configuration
{
    public class MappingEduConfiguration : IMappingEduConfiguration
    {
        public const string ProdConnectionStringNameKey = "MappingEdu";
        public const string ProdAdminConnectionStringNameKey = "DbAdmin";

        private const string GoogleAnalyticsIdKey = "GoogleAnalyticsId";
        private const string GoogleAnalyticsWebSiteKey = "GoogleAnalyticsWebSite";
        private readonly IApplicationConfigurationReader _applicationConfigurationReader;

        public MappingEduConfiguration(IApplicationConfigurationReader applicationConfigurationReader)
        {
            _applicationConfigurationReader = applicationConfigurationReader;
        }

        public string ProdConnectionString
        {
            get { return _applicationConfigurationReader.GetConnectionString(ProdConnectionStringNameKey); }
        }

        public string AdminConnectionString
        {
            get { return _applicationConfigurationReader.GetConnectionString(ProdAdminConnectionStringNameKey); }
        }

        public string GoogleAnalyticsId
        {
            get { return _applicationConfigurationReader.GetOptionalSetting(GoogleAnalyticsIdKey).Value; }
        }

        public string GoogleAnalyticsWebSite
        {
            get { return _applicationConfigurationReader.GetOptionalSetting(GoogleAnalyticsWebSiteKey).Value; }
        }
    }
}