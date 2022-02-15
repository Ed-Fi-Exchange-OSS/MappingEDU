// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Common;
using MappingEdu.Common.Configuration;
using MappingEdu.Core.DataAccess.Entities.Cache;

namespace MappingEdu.Core.DataAccess
{
    /// <summary>
    ///     Configuration for the data access layer.
    /// </summary>
    internal sealed class Configuration
    {
        private const string CacheEnabledKey = "MappingEdu.DataAccess.Cache.Enabled";
        private const string CacheModeKey = "MappingEdu.DataAccess.Cache.Mode";
        private const string CacheModeSlidingAbsoluteMinutesKey = "MappingEdu.DataAccess.Cache.AbsoluteMinutes";
        private const string CacheModeSlidingExpirationKey = "MappingEdu.DataAccess.Cache.SlidingExpiration";
        private const string CacheRedisConnectionStringKey = "MappingEdu.DataAccess.Cache.Redis.ConnectionString";
        private const string DatabasePrefixKey = "MappingEdu.DataAccess.Database.Prefix";
        private const string DatabaseSchemaKey = "MappingEdu.DataAccess.Database.Schema";
        private static readonly IConfigurationStore _configurationStore;

        static Configuration()
        {
            var factory = new ConfigurationStoreFactory();
            _configurationStore = factory.GetStore();
        }

        public static class Cache
        {
            public static int AbsoluteExpirationMinutes
            {
                get { return _configurationStore.GetSetting(CacheModeSlidingAbsoluteMinutesKey, 10); }
            }

            public static bool Enabled
            {
                get { return _configurationStore.GetSetting(CacheEnabledKey, false); }
            }

            public static CacheMode Mode
            {
                get { return _configurationStore.GetSetting(CacheModeKey, CacheMode.InMemory); }
            }

            public static int SlidingExpiration
            {
                get { return _configurationStore.GetSetting(CacheModeSlidingExpirationKey, 0); }
            }

            public static class Redis
            {
                public static string ConnectionSting
                {
                    get { return _configurationStore.GetSetting(CacheRedisConnectionStringKey, string.Empty); }
                }
            }
        }

        public static class Database
        {
            public static string Prefix
            {
                get { return _configurationStore.GetSetting(DatabasePrefixKey, string.Empty); }
            }

            public static string Schema
            {
                get { return _configurationStore.GetSetting(DatabaseSchemaKey, Constants.DataAccess.Schema); }
            }
        }
    }
}