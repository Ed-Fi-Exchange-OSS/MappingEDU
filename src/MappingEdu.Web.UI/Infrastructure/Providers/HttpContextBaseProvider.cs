// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web;

namespace MappingEdu.Web.UI.Infrastructure.Providers
{
    public class HttpContextBaseProvider
    {
        public static Func<HttpContextBase> GetHttpContextBase =
            () => { throw new Exception(string.Format("{0} is not initialized", typeof (HttpContextBaseProvider))); };

        public static void AlwaysUseCurrentContext()
        {
            GetHttpContextBase = () => new HttpContextWrapper(HttpContext.Current);
        }
    }
}