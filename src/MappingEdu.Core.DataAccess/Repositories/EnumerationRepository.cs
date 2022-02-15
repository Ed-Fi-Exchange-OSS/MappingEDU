// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class EnumerationRepository : IEnumerationRepository
    {
        private readonly EntityContext _databaseContext;

        public EnumerationRepository(EntityContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public DbEnumeration Load(string enumerationTypeName)
        {
            var entityFrameworkCollectionName = enumerationTypeName + "s";
            if (enumerationTypeName.EndsWith("y"))
                entityFrameworkCollectionName = enumerationTypeName.Substring(0, enumerationTypeName.Length - 1) + "ies";

            var databaseContextType = _databaseContext.GetType();
            var collectionProperty = databaseContextType.GetProperty(entityFrameworkCollectionName,
                BindingFlags.Public | BindingFlags.Instance);

            var databaseValues = (IEnumerable) collectionProperty.GetValue(_databaseContext);

            var markerType = typeof (DomainModule);
            var entityType = markerType.Assembly.GetType(string.Format("{0}.{1}", markerType.Namespace, enumerationTypeName));
            var idPropertyName = enumerationTypeName + "Id";
            var namePropertyName = enumerationTypeName + "Name";
            var idProperty = GetProperty(entityType, idPropertyName);
            var nameProperty = GetProperty(entityType, namePropertyName);

            var dbValues = databaseValues
                .Cast<object>()
                .Select(x => new DbEnumerationValue(idProperty.GetValue(x), (string) nameProperty.GetValue(x)));
            return new DbEnumeration(enumerationTypeName, dbValues);
        }

        private static PropertyInfo GetProperty(Type entityType, string propertyName)
        {
            var property = entityType.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new Exception(string.Format("Could not load property '{0}' on type '{1}'", propertyName,
                    entityType));
            }
            return property;
        }
    }
}