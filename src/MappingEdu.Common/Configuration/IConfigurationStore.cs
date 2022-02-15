// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Configuration;

namespace MappingEdu.Common.Configuration
{
    /// <summary>
    ///     A configuration store.
    /// </summary>
    public interface IConfigurationStore
    {
        /// <summary>
        ///     Returns an application setting casted to defined type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="key">The application settings key</param>
        /// <param name="defaultValue">The default value</param>
        T GetSetting<T>(string key, T defaultValue);

        /// <summary>
        ///     Returns an application setting as a string.
        /// </summary>
        /// <param name="key">The application settings key</param>
        /// <param name="defaultValue">The default value</param>
        string GetSetting(string key, string defaultValue);

        /// <summary>
        ///     Returns a configuration section.
        /// </summary>
        /// <typeparam name="T">The type for the config section</typeparam>
        /// <param name="sectionName">The section name.</param>
        T GetSection<T>(string sectionName) where T : ConfigurationSection;
    }
}