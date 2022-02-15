// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using EFCache;
using EFCache.Redis;
using MappingEdu.Core.DataAccess.Entities.Cache;

namespace MappingEdu.Core.DataAccess.Entities
{
    /// <summary>
    ///     Entity database context configuration.
    /// </summary>
    /// <remarks>
    ///     This class will automatically be located by the entity framework
    ///     at runtime. It provides level-2 cache support (in-memory and redis)
    /// </remarks>
    public class EntityConfiguration : DbConfiguration
    {
        private static readonly Lazy<ICache> _instance = new Lazy<ICache>(() =>
        {
          switch (Configuration.Cache.Mode)
            {
                case CacheMode.Redis:
                    return new RedisCache(Configuration.Cache.Redis.ConnectionSting);
                case CacheMode.InMemory:
                    return new InMemoryCache();
                default:
                    throw new Exception("Unsupported cache mode.");
            }
        });

        public EntityConfiguration()
        {
            if (!Configuration.Cache.Enabled)
                return;

            var transactionHandler = new CacheTransactionHandler(_instance.Value);

            AddInterceptor(transactionHandler);

            Loaded += (sender, args) => args.ReplaceService<DbProviderServices>(
                (s, _) => new CachingProviderServices(s, transactionHandler));
        }
    }
}