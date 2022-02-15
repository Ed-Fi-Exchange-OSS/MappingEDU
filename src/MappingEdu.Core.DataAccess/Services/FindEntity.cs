// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Infrastructure;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;

namespace MappingEdu.Core.DataAccess.Services
{
    public interface IFindEntity
    {
        T FindExisitingEntity<T>(T entity) where T : class;
    }

    public class FindEntity : IFindEntity
    {
        private readonly EntityContext _context;

        public FindEntity(EntityContext context)
        {
            _context = context;
        }

        public T FindExisitingEntity<T>(T entity) where T : class
        {
            var keyValues = GetKeys(entity);
            return _context.Set<T>().Find(keyValues);
        }

        private string[] GetKeyNames<T>() where T : class
        {
            var objectSet = ((IObjectContextAdapter) _context).ObjectContext.CreateObjectSet<T>();
            var keyNames = objectSet.EntitySet.ElementType.KeyMembers.Select(k => k.Name).ToArray();
            return keyNames;
        }

        private object[] GetKeys<T>(T entity) where T : class
        {
            var keyNames = GetKeyNames<T>();
            var type = typeof (T);

            var keys = new object[keyNames.Length];
            for (var i = 0; i < keyNames.Length; i++)
            {
                keys[i] = type.GetProperty(keyNames[i]).GetValue(entity, null);
            }
            return keys;
        }
    }
}