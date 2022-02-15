// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MappingEdu.Service.Model.Admin
{
    public class ConfigurationStatus
    {
        public enum ConfigurationCheckType
        {
            Unknown,
            Database,
            Images,
            DocumentationSite,
            IIS,
            GoogleAnalytics
        }

        public ConfigurationCheck[] ConfigurationChecks { get; set; }

        public string DatabaseCatalog { get; set; }

        public ConfigurationCheck[] DatabaseConfigurationChecks
        {
            get { return DeploymentChecksOfType(ConfigurationCheckType.Database); }
        }

        public string DatabaseServer { get; set; }

        public ConfigurationCheck[] DocumentationSiteConfigurationChecks
        {
            get { return DeploymentChecksOfType(ConfigurationCheckType.DocumentationSite); }
        }

        public string DocumentationSiteUrl { get; set; }

        public ConfigurationCheck[] GoogleAnalyticsConfigurationChecks
        {
            get { return DeploymentChecksOfType(ConfigurationCheckType.GoogleAnalytics); }
        }

        public string GoogleAnalyticsId { get; set; }

        public string GoogleAnalyticsWebSite { get; set; }

        public ConfigurationCheck[] IISConfigurationChecks
        {
            get { return DeploymentChecksOfType(ConfigurationCheckType.IIS); }
        }

        public string ImageDiskPath { get; set; }

        public ConfigurationCheck[] ImagesConfigurationChecks
        {
            get { return DeploymentChecksOfType(ConfigurationCheckType.Images); }
        }

        public string ImageUrlRoot { get; set; }

        private ConfigurationCheck[] DeploymentChecksOfType(ConfigurationCheckType configurationCheckType)
        {
            if (ConfigurationChecks == null)
                return null;

            return ConfigurationChecks.Where(x => x.ConfigurationCheckType == configurationCheckType).OrderBy(x => x.Title).ToArray();
        }

        public class ConfigurationCheck
        {
            public ConfigurationCheckType ConfigurationCheckType { get; set; }

            public Dictionary<string, string> Data { get; set; }

            public string DataId { get; set; }

            public bool? Success { get; set; }

            public string Title { get; set; }

            public ConfigurationCheck()
            {
                Data = new Dictionary<string, string>();
            }

            public string GetDataAttributes()
            {
                var sb = new StringBuilder();
                sb.AppendFormat("data-id='{0}' ", DataId);
                foreach (var d in Data)
                    sb.AppendFormat("data-{0}='{1}' ", d.Key, d.Value);
                return sb.ToString();
            }
        }
    }
}