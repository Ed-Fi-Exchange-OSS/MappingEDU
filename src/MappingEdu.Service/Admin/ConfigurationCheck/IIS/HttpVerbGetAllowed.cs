// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Service.Model.Admin;

namespace MappingEdu.Service.Admin.ConfigurationCheck.IIS
{
    public class HttpVerbGetAllowed : IConfigurationCheck
    {
        public ConfigurationStatus.ConfigurationCheck Check()
        {
            return new ConfigurationStatus.ConfigurationCheck
            {
                ConfigurationCheckType = ConfigurationStatus.ConfigurationCheckType.IIS,
                Title = "HTTP verb GET allowed",
                DataId = "IISGETAllowed"
            };
        }
    }
}