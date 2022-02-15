// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using System.Web.Routing;
using MappingEdu.Core.Domain.Services;
using MappingEdu.Service.Providers;
using MappingEdu.Web.UI.Infrastructure;
using MappingEdu.Web.UI.Infrastructure.Config;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace MappingEdu.Web.UI.Infrastructure
{
    /// <summary>
    ///     Application (OWIN) Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Configures the application
        /// </summary>
        /// <param name="app">The app builder</param>
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ApiRouteConfig.Register(config);
            ContainerConfig.Register(app, config);
            DocumentationConfig.Register(config);
            FormattersConfig.Register(config);
            LoggingConfig.Initialize();
            MapperConfig.Initialize();

            MvcRouteConfig.RegisterRoutes(RouteTable.Routes);

            SystemClock.Now = () => DateTime.Now;

            DataProtectionProvider.ConfigureAuth(app);

            // authorize all requests
            config.Filters.Add(new AuthorizeAttribute());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
        }

        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; set; }
    }
}
