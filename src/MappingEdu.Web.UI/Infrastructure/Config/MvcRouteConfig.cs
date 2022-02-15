// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace MappingEdu.Web.UI.Infrastructure.Config
{
    /// <summary>
    ///     Configures MVc routes
    /// </summary>
    public class MvcRouteConfig
    {
        /// <summary>
        ///     Registers the configuration
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                 name: "Default",
                 url: "",
                 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
             );

            // Added to insure that we hit the index.html
            routes.Ignore("");
        }
    }
}
