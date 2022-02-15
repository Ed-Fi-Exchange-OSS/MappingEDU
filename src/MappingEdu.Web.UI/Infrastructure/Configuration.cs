// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#region

using System;
using MappingEdu.Common.Configuration;

#endregion

namespace MappingEdu.Web.UI.Infrastructure
{
    /// <summary>
    ///     Configuration for the web application.
    /// </summary>
    internal sealed class Configuration
    {
        private const string OAuthAccessTokenExpireTimeSpanKey = "MappingEdu.Web.OAuth.AccessTokenExpireAfterDays";
        private const string OAuthAllowInsecureHttpKey = "MappingEdu.Web.OAuth.AllowInsecureHttp";
        private const string SupportEmailAddressKey = "MappingEdu.Web.Support.EmailAddress";
        private const string SupportUrlKey = "MappingEdu.Web.Support.Url";
        private const string LicenseUrlKey = "MappingEdu.Web.Support.LicenseUrl";
        private const string NotAvailable = "not available";

        private static readonly IConfigurationStore _configurationStore;

        static Configuration()
        {
            var factory = new ConfigurationStoreFactory();
            _configurationStore = factory.GetStore();
        }

        public static class AccessToken
        {
            public static bool AllowInsecureHttp => bool.Parse(_configurationStore.GetSetting(OAuthAllowInsecureHttpKey, "false"));

            public static TimeSpan ExpireTimeSpan => TimeSpan.FromDays(_configurationStore.GetSetting(OAuthAccessTokenExpireTimeSpanKey, 365));
        }

        public static class Support
        {
            public static string SupportEmailAddress => _configurationStore.GetSetting(SupportEmailAddressKey, NotAvailable);

            public static string SupportUrl => _configurationStore.GetSetting(SupportUrlKey, NotAvailable);

            public static string LicenseUrl => _configurationStore.GetSetting(LicenseUrlKey, NotAvailable);
        }
    }
}
