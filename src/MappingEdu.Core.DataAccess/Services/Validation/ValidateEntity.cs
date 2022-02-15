// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Services.Validation;

namespace MappingEdu.Core.DataAccess.Services.Validation
{
    public abstract class ValidateEntity<TEntity, TFailure> : IValidateEntity
        where TFailure : IValidationFailure
        where TEntity : IEntity
    {
        private readonly IEntityValidator<TEntity, TFailure> _entityValidator;

        protected ValidateEntity(IEntityValidator<TEntity, TFailure> entityValidator)
        {
            _entityValidator = entityValidator;
        }

        public DbValidationError[] ValidateForAdd(IEntity entity)
        {
            return ValidateForAdd((TEntity) entity);
        }

        public DbValidationError[] ValidateForUpdate(IEntity entity)
        {
            return ValidateForUpdate((TEntity) entity);
        }

        public bool CanValidate(IEntity entity)
        {
            return entity is TEntity;
        }

        protected virtual DbValidationError[] ValidateForAdd(TEntity entity)
        {
            var failures = _entityValidator.ValidateForAdd(entity);
            return ConvertTFailureToDbValidationErrors(failures);
        }

        protected virtual DbValidationError[] ValidateForUpdate(TEntity entity)
        {
            var failures = _entityValidator.ValidateForUpdate(entity);
            return ConvertTFailureToDbValidationErrors(failures);
        }

        private static DbValidationError[] ConvertTFailureToDbValidationErrors(IEnumerable<TFailure> failures)
        {
            return failures.Select(x => new DbValidationError(x.PropertyName, x.ValidationError)).ToArray();
        }
    }
}