// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Services.Configuration
{
    public interface IApplicationConfigurationReader
    {
        string GetSetting(string key);

        string GetConnectionString(string key);

        bool HasSetting(string key);

        bool HasConnectionString(string key);

        OptionalSetting<string> GetOptionalSetting(string key);
    }
}