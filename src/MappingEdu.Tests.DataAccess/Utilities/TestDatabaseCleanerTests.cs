// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.DataAccess;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;

namespace MappingEdu.Tests.DataAccess.Utilities
{
    public class TestDatabaseCleanerTests : TestBase
    {
        //[Test]
        //public void Entities_should_be_cleaned_cascaded_or_ignored()
        //{
        //    var entityTypes = GetDeclaredEntityTypes();
        //    foreach (var entityType in entityTypes)
        //    {
        //        var entitiesToClean = TestDatabaseCleaner.EntitiesToClean;
        //        var entitesToCascade = TestDatabaseCleaner.EntitiesThatShouldCascade;
        //        var entitiesToIgnore = TestDatabaseCleaner.EntitiesToIgnore;
        //        var handledEntities = entitiesToClean.Union(entitiesToIgnore);
        //        handledEntities = handledEntities.Union(entitesToCascade);

        //        if (!handledEntities.Contains(entityType))
        //        {
        //            Assert.Fail("Entity {0} is not included in the EntitiesToClean, EntitiesThatShouldCascade or EntitiesToIgnore.", entityType);
        //        }
        //    }
        //}

        //private static IEnumerable<Type> GetDeclaredEntityTypes()
        //{
        //    var entityTypes = typeof (DataAccessModule).Assembly
        //        .GetAllTypesClosing(typeof (EntityTypeConfiguration<>))
        //        .Select(t => t.BaseType.GetGenericArguments()[0])
        //        .ToList()
        //        ;
        //    return entityTypes;
        //}
    }
}