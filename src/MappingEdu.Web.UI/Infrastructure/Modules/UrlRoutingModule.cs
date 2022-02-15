// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web;
using System.Web.Routing;
using MappingEdu.Web.UI.Infrastructure.Extensions;

namespace MappingEdu.Web.UI.Infrastructure.Modules
{
    public class UrlRoutingModule : System.Web.Routing.UrlRoutingModule
    {
        public override void PostResolveRequestCache(HttpContextBase context)
        {
            var routeData = RouteCollection.GetRouteData(context);

            var redirecturi = GetRedirectUri(routeData, context.Request.Url);

            if (redirecturi != null && redirecturi.AbsoluteUri != context.Request.Url.AbsoluteUri)
            {
                context.Response.RedirectPermanent(redirecturi.ToString());
                return;
            }

            base.PostResolveRequestCache(context);
        }

        private static Uri GetRedirectUri(RouteData routeData, Uri requestedUri)
        {
            if (routeData == null)
                return null;

            var route = routeData.Route as Route;
            return route == null ? null : requestedUri.MatchSegmentsAndSlashes(route.Url);
        }
    }
}