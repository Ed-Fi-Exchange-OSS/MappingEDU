// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Http;

namespace MappingEdu.Web.UI.Infrastructure.Config
{
    /// <summary>
    ///     Configures API routes
    /// </summary>
    public class ApiRouteConfig
    {
        /// <summary>
        ///     Registers the configuration
        /// </summary>
        public static void Register(HttpConfiguration configuration)
        {
            configuration.MapHttpAttributeRoutes();
        }
    }
}