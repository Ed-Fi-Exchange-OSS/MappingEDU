// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Common.Configuration;
using System.Configuration;
using System;

namespace MappingEdu.Tests.Business.Bases.Mocks
{
    public class ConfigurationStoreMock : IConfigurationStore
    {
        public T GetSection<T>(string sectionName) where T : ConfigurationSection
        {
            throw new NotImplementedException();
        }

        public T GetSetting<T>(string key, T defaultValue)
        {
            return defaultValue;
        }

        public string GetSetting(string key, string defaultValue)
        {
            return defaultValue;
        }
    }
}