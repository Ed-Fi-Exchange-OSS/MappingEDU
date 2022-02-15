// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using MappingEdu.Core.Domain.Enumerations;

namespace MappingEdu.Core.DataAccess.Services
{
    public interface IEnumerationEntityCreator
    {
        TEntity[] CreateEntityFromEnum<TEnum, TEntity>(IFindEntity findEntity) where TEnum : IDatabaseEnumeration;
    }

    public class EnumerationEntityCreator : IEnumerationEntityCreator
    {
        public TEntity[] CreateEntityFromEnum<TEnum, TEntity>(IFindEntity findEntity) where TEnum : IDatabaseEnumeration
        {
            var method = typeof (TEnum).GetMethod("GetDatabaseValues",
                BindingFlags.Public | BindingFlags.Static |
                BindingFlags.FlattenHierarchy);
            var enumerationValues = (dynamic) method.Invoke(null, null);

            var entities = new List<TEntity>();
            foreach (var enumerationValue in enumerationValues)
            {
                dynamic entity = Mapper.Map(enumerationValue, typeof (TEnum), typeof (TEntity));

                dynamic existingEntity = findEntity.FindExisitingEntity(entity);
                if (existingEntity != null)
                    entity = Mapper.Map(enumerationValue, existingEntity, typeof (TEnum), typeof (TEntity));

                entities.Add(entity);
            }

            return entities.ToArray();
        }
    }
}