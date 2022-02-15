// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Reflection;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.DataAccess.Services;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Repositories;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;

namespace MappingEdu.Tests.DataAccess.Core.Enumerations
{
    [TestFixture]
    public class AllDatabaseEnumerationsTests : EmptyDatabaseTestBase
    {
        private void LoadType(Type t)
        {
            var reflector = GetInstance<IEnumerationRepository>();
            try
            {
                reflector.Load(t.Name);
            }
            catch (Exception e)
            {
                throw new Exception(
                    string.Format("Could not load database enumeration {0} --> ({1})", t.Name, e.Message), e);
            }
        }

        private void CreateEntities<TEnum, TEntity>() where TEnum : IDatabaseEnumeration
        {
            var reflector = GetInstance<IEnumerationEntityCreator>();
            try
            {
                var doNotFindEntity = new DoNotFindEntity();
                var entities = reflector.CreateEntityFromEnum<TEnum, TEntity>(doNotFindEntity);
            }
            catch (Exception e)
            {
                throw new Exception(
                    string.Format("Could not create entities from database enumeration {0} --> ({1})",
                        typeof (TEnum).Name, e.Message), e);
            }
        }

        public class DoNotFindEntity : IFindEntity
        {
            public T FindExisitingEntity<T>(T entity) where T : class
            {
                return null;
            }
        }

        [Test]
        public void For_each_declared_db_enumeration_there_should_be_an_enumeration_in_the_database()
        {
            var types = typeof (IDatabaseEnumeration)
                .Assembly
                .GetTypes()
                .Where(x => x.CanBeCastTo<IDatabaseEnumeration>())
                .Where(x => x.IsConcrete())
                .ToArray();

            types.Do(LoadType);
        }

        //This functionality used for migrations
        [Test]
        public void Should_be_able_to_create_database_enumeration_values_by_reflection()
        {
            var types = typeof (IDatabaseEnumeration)
                .Assembly
                .GetTypes()
                .Where(x => x.CanBeCastTo<IDatabaseEnumeration>())
                .Where(x => x.IsConcrete())
                .ToArray();

            var addOrUpdateDatabaseEnumerationMethod = GetType()
                .GetMethod("CreateEntities", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var type in types)
            {
                var markerType = typeof (DomainModule);
                var entityType = markerType.Assembly.GetType(string.Format("{0}.{1}", markerType.Namespace, type.Name));
                addOrUpdateDatabaseEnumerationMethod.MakeGenericMethod(type, entityType).Invoke(this, null);
            }
        }
    }
}