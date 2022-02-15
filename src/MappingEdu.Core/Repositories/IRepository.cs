// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Repositories
{
    /// <summary>
    ///     This interface defines the methods that will be used to create a simple
    ///     entity repository.
    /// </summary>
    /// <typeparam name="TEntity">The type to modify in the data store.</typeparam>
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        /// <summary>
        ///     Gets all objects of a particular type from a data context.
        /// </summary>
        /// <returns>An array of objects of the requested type.</returns>
        TEntity[] GetAll();

        IQueryable<TEntity> GetAllQueryable();

        IQueryable<TEntity> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> querySharper);

        /// <summary>
        ///     Gets data for validation purposes.
        /// </summary>
        /// <returns></returns>
        TEntity[] GetForValidation();

        /// <summary>
        ///     Gets a specific object of a particular type from a data context.
        /// </summary>
        /// <returns>The requested entity.</returns>
        TEntity Get(Guid id);

        /// <summary>
        ///     Adds the specified entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        ///     Adds the specified entities to the database context.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Deletes the specified entity within the database context.
        /// </summary>
        /// <param name="id">The id of the entity to delete.</param>
        void Delete(Guid id);

        /// <summary>
        ///     Deletes the specified entity within the database context.
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        void Delete(TEntity entity);

        /// <summary>
        ///     Deletes the specified entities within the database context.
        /// </summary>
        /// <param name="entities">Entities to delete</param>
        void RemoveRange(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Saves any updated entities in the context.
        /// </summary>
        void SaveChanges();
    }
}