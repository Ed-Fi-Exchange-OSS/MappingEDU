// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        public EntityContext _databaseContext; //TODO: FIx me

        public Repository(EntityContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        /// <summary>
        ///     Gets all objects of a particular type from a data context.
        /// </summary>
        /// <returns>An array of objects of the requested type.</returns>
        public virtual TEntity[] GetAll()
        {
            var entities = _databaseContext.Set<TEntity>();
            return entities.ToArray();
        }

        public IQueryable<TEntity> GetAllQueryable()
        {
            return _databaseContext.Set<TEntity>();
        }

        /// <summary>
        ///     Gets data for validation purposes.
        /// </summary>
        /// <returns></returns>
        public TEntity[] GetForValidation()
        {
            _databaseContext.LoadValidationEntities();

            var entities = from data in _databaseContext.Set<TEntity>().Local
                select data;

            return entities.ToArray();
        }

        /// <summary>
        ///     Gets a specific object of a particular type from a data context.
        /// </summary>
        /// <returns>The requested entity.</returns>
        public TEntity Get(Guid id)
        {
            return _databaseContext.Set<TEntity>().Find(id);
        }

        /// <summary>
        ///     Adds the specified entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            _databaseContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        ///     Adds the specified entities to the database context.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            _databaseContext.Set<TEntity>().AddRange(entities);
        }

        /// <summary>
        ///     Deletes the specified entity within the database context.
        /// </summary>
        /// <param name="id">Id of the entity to delete.</param>
        public virtual void Delete(Guid id)
        {
            var entity = Get(id);
            Delete(entity);
        }

        /// <summary>
        ///     Deletes the specified entity within the database context.
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity != null)
                _databaseContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        ///     Deletes the specified entities within the database context.
        /// </summary>
        /// <param name="entities">Entities to delete</param>
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _databaseContext.Set<TEntity>().RemoveRange(entities);
        }


        /// <summary>
        ///     Saves any updated entities in the context.
        /// </summary>
        public void SaveChanges()
        {
            _databaseContext.SaveChanges();
        }

        public IQueryable<TEntity> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> querySharper)
        {
            var query = querySharper(_databaseContext.Set<TEntity>());
            return query;
        }
    }
}