// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
using EFCache;

namespace MappingEdu.Core.DataAccess.Entities.Cache
{
    internal class CachePolicy : CachingPolicy
    {
        protected override void GetExpirationTimeout(ReadOnlyCollection<EntitySetBase> affectedEntitySets, out TimeSpan slidingExpiration, out DateTimeOffset absoluteExpiration)
        {
            slidingExpiration = TimeSpan.FromMinutes(Configuration.Cache.SlidingExpiration);
            absoluteExpiration = DateTimeOffset.Now.AddMinutes(Configuration.Cache.AbsoluteExpirationMinutes);
        }
    }
}