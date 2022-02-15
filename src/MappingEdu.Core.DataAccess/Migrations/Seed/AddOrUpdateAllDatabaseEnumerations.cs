// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Services;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Enumerations;

namespace MappingEdu.Core.DataAccess.Migrations.Seed
{
    public sealed class AddOrUpdateAllDatabaseEnumerations
    {
        public static void Run(EntityContext context)
        {
            var local = new AddOrUpdateAllDatabaseEnumerations();
            local.AddOrUpdate(context);
        }

        private void AddOrUpdate(EntityContext context)
        {
            var types = typeof (IDatabaseEnumeration)
                .Assembly
                .GetTypes()
                .Where(x => x.CanBeCastTo<IDatabaseEnumeration>())
                .Where(x => x.IsConcrete())
                .ToArray();

            var addOrUpdateDatabaseEnumerationMethod = GetType().GetMethod("AddOrUpdateDatabaseEnumeration", BindingFlags.NonPublic | BindingFlags.Instance);
            var coreModelsAssembly = typeof (DomainModule).Assembly;
            foreach (var type in types)
            {
                var entityType = coreModelsAssembly.GetType("MappingEdu.Core.Domain." + type.Name);
                addOrUpdateDatabaseEnumerationMethod.MakeGenericMethod(type, entityType).Invoke(this, new object[] {context});
            }
        }

        private void AddOrUpdateDatabaseEnumeration<TEnum, TEntity>(EntityContext context)
            where TEntity : class
            where TEnum : IDatabaseEnumeration
        {
            var enumerationCreator = new EnumerationEntityCreator();
            try
            {
                var findEntities = new FindEntity(context);
                var databaseEnumerationValues = enumerationCreator.CreateEntityFromEnum<TEnum, TEntity>(findEntities);
                var dbSet = context.Set<TEntity>();
                dbSet.AddOrUpdate(databaseEnumerationValues);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Could not create database enumeration entities {0} --> ({1})", typeof (TEnum).Name, e.Message), e);
            }
        }
    }
}