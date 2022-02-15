// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using Should;

namespace MappingEdu.Tests.DataAccess.Utilities
{
    public interface ITestDatabaseCleaner
    {
        void CleanEntityTables();
    }

    public class TestDatabaseCleaner : ITestDatabaseCleaner
    {
        public static readonly List<Type> EntitiesToClean = new List<Type>
        {
            typeof (CustomMigration),
            typeof (MappedSystemUser),
            typeof (MappingProjectUser),
            typeof (MappedSystem),
            typeof (SystemEnumerationItem),
            typeof (SystemEnumerationItemMap),
            typeof (SystemItem),
            typeof (SystemItemMap),
            typeof (SystemItemVersionDelta),
            typeof (SystemItemCustomDetail),
            typeof (MappingProject),
            typeof (Log)
        };

        public static readonly List<Type> EntitiesThatShouldCascade = new List<Type>
        {
            typeof (Note),
            typeof (CustomDetailMetadata),
            typeof (MapNote)
        };

        public static readonly List<Type> EntitiesToIgnore = new List<Type>
        {
            typeof (AutoMappingReasonType),
            typeof (BuildVersion),
            typeof (CompleteStatusType),
            typeof (EnumerationMappingStatusType),
            typeof (EnumerationMappingStatusReasonType),
            typeof (ItemChangeType),
            typeof (ItemDataType),
            typeof (ItemType),
            typeof (MappingStatusType),
            typeof (MappingStatusReasonType),
            typeof (Organization),
            typeof (ProjectStatusType),
            typeof (MappingMethodType),
            typeof (WorkflowStatusType)
        };

        private readonly EntityContext _databaseContext;

        public TestDatabaseCleaner(EntityContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void CleanEntityTables()
        {
            var cleanTableMethod = GetType().GetMethod("CleanTable", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var type in (IEnumerable<Type>) EntitiesToClean)
            {
                cleanTableMethod.MakeGenericMethod(type).Invoke(this, new[] {_databaseContext});
            }

            _databaseContext.SaveChanges();

            var ensureCascadeMethod = GetType().GetMethod("EnsureCascade", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var type in EntitiesThatShouldCascade)
            {
                ensureCascadeMethod.MakeGenericMethod(type).Invoke(this, new[] {_databaseContext});
            }
        }

        protected void CleanTable<TEntity>(EntityContext databaseContext) where TEntity : class
        {
            foreach (var entity in databaseContext.Set<TEntity>().ToList())
                databaseContext.Set<TEntity>().Remove(entity);
        }

        protected void EnsureCascade<TEntity>(EntityContext databaseContext) where TEntity : class
        {
            databaseContext.Set<TEntity>().Any().ShouldBeFalse();
        }
    }
}