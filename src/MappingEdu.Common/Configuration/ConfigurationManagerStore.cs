// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;

namespace MappingEdu.Common.Configuration
{
    /// <summary>
    ///     Store for accessing settings in the configuration manager.
    /// </summary>
    public class ConfigurationManagerStore : IConfigurationStore
    {
        public T GetSetting<T>(string key, T defaultValue)
        {
            try
            {
                var setting = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrWhiteSpace(setting))
                    return defaultValue;

                var result = (T) Convert.ChangeType(setting, typeof (T));

                if (null == result)
                    throw new Exception();

                return result;
            }
            catch
            {
                return defaultValue;
            }
        }

        public string GetSetting(string key, string defaultValue)
        {
            var setting = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(setting) ? defaultValue : setting;
        }

        public T GetSection<T>(string sectionName) where T : ConfigurationSection
        {
            return ConfigurationManager.GetSection(sectionName) as T;
        }
    }
}