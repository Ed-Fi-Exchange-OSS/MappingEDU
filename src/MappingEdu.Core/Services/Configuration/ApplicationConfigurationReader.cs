// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;

namespace MappingEdu.Core.Services.Configuration
{
    public class ApplicationConfigurationReader : IApplicationConfigurationReader
    {
        public string GetSetting(string key)
        {
            if (!HasSetting(key))
                throw new Exception(string.Format("Could not find setting '{0}'", key));

            return ConfigurationManager.AppSettings[key];
        }

        public string GetConnectionString(string key)
        {
            if (!HasConnectionString(key))
                throw new Exception(string.Format("Could not find connection string '{0}'", key));

            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        public bool HasSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            return !string.IsNullOrEmpty(value);
        }

        public bool HasConnectionString(string key)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[key];
            if (connectionStringSettings == null)
                return false;

            var value = connectionStringSettings.ConnectionString;
            var hasConnectionString = !string.IsNullOrEmpty(value);
            return hasConnectionString;
        }

        public OptionalSetting<string> GetOptionalSetting(string key)
        {
            return HasSetting(key) ? new OptionalSetting<string>(GetSetting(key)) : OptionalSetting<string>.Empty();
        }
    }
}