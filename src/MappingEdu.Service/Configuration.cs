// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using CuttingEdge.Conditions;
using MappingEdu.Common.Configuration;

namespace MappingEdu.Service
{
    /// <summary>
    ///     Configuration for the services layer.
    /// </summary>
    internal sealed class Configuration
    {
        //Application
        private const string ApplicationAppNameKey = "MappingEdu.Service.Application.AppName";

        //Email
        private const string EmailDisableSSLCertificateCheckKey = "MappingEdu.Service.Email.DisableSSLCertificateCheck";
        private const string EmailEnableSSLKey = "MappingEdu.Service.Email.EnableSSL";
        private const string EmailFromKey = "MappingEdu.Service.Email.From";
        private const string EmailPasswordKey = "MappingEdu.Service.Email.Password";
        private const string EmailPortKey = "MappingEdu.Service.Email.Port";
        private const string EmailServerAddressKey = "MappingEdu.Service.Email.ServerAddress";
        private const string EmailUsernameKey = "MappingEdu.Service.Email.Username";

        //Membership
        private const string MembershipOrganizationDomainDelimiterKey = "MappingEdu.Service.Membership.OrganizationDomainDelimiter";
        private static readonly IConfigurationStore _configurationStore;

        static Configuration()
        {
            var factory = new ConfigurationStoreFactory();
            _configurationStore = factory.GetStore();
        }

        public static class Application
        {
            public static string AppName
            {
                get { return _configurationStore.GetSetting(ApplicationAppNameKey, "MappingEDU"); }
            }
        }

        public static class Membership
        {
            public static char OrganizationDomainDelimiter
            {
                get { return _configurationStore.GetSetting(MembershipOrganizationDomainDelimiterKey, ','); }
            }
        }

        public static class Email
        {
            public static bool DisableSSLCertificateCheck
            {
                get { return _configurationStore.GetSetting(EmailDisableSSLCertificateCheckKey, false); }
            }

            public static bool EnableSSL
            {
                get { return _configurationStore.GetSetting(EmailEnableSSLKey, true); }
            }

            public static string From
            {
                get { return _configurationStore.GetSetting(EmailFromKey, Username); }
            }

            public static string Password
            {
                get
                {
                    var password = _configurationStore.GetSetting(EmailPasswordKey, string.Empty);
                    Condition.Ensures(password).IsNotNullOrWhiteSpace(string.Format("Application setting '{0}' has not been set", EmailPasswordKey));
                    return password;
                }
            }

            public static int Port
            {
                get { return _configurationStore.GetSetting(EmailPortKey, EnableSSL ? 587 : 25); }
            }

            public static string ServerAddress
            {
                get
                {
                    var address = _configurationStore.GetSetting(EmailServerAddressKey, string.Empty);
                    Condition.Ensures(address).IsNotNullOrWhiteSpace(string.Format("Application setting '{0}' has not been set", EmailServerAddressKey));
                    return address;
                }
            }

            public static string Username
            {
                get
                {
                    var username = _configurationStore.GetSetting(EmailUsernameKey, string.Empty);
                    Condition.Ensures(username).IsNotNullOrWhiteSpace(string.Format("Application setting '{0}' has not been set", EmailUsernameKey));
                    return username;
                }
            }
        }
    }
}