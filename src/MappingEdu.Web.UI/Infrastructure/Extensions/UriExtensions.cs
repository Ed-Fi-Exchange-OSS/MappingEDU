// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Web.UI.Infrastructure.Extensions
{
    public static class UriExtensions
    {
        private const string SLASH = "/";

        public static Uri Append(this Uri uri, string value)
        {
            return new Uri(GetAbsoluteUriWithoutQuery(uri) + value + uri.Query);
        }

        public static Uri MatchSegmentsAndSlashes(this Uri uri, string routeUrl)
        {
            var slashRequired = routeUrl.EndsWith(SLASH);

            if (slashRequired && !uri.AbsolutePath.EndsWith(SLASH))
            {
                return uri.Append(SLASH);
            }

            if (!slashRequired && uri.AbsolutePath.EndsWith(SLASH))
                return uri.TrimPathEnd(SLASH[0]);

            return uri;
        }

        private static Uri TrimPathEnd(this Uri uri, params char[] trimChars)
        {
            return new Uri(GetAbsoluteUriWithoutQuery(uri).TrimEnd(trimChars) + uri.Query);
        }

        private static string GetAbsoluteUriWithoutQuery(Uri uri)
        {
            var ret = uri.AbsoluteUri;
            if (uri.Query.Length > 0) ret = ret.Substring(0, ret.Length - uri.Query.Length);
            return ret;
        }
    }
}