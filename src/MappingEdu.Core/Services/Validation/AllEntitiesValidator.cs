// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.Services.Validation
{
    public abstract class AllEntitiesValidator<TEntity, TValidationFailure> : Validator<TValidationFailure>
        where TValidationFailure : IValidationFailure
        where TEntity : IEntity
    {
        private readonly IEntityValidator<TEntity, TValidationFailure> _entityValidator;
        private readonly IRepository<TEntity> _repository;

        protected AllEntitiesValidator(IRepository<TEntity> repository,
            IEntityValidator<TEntity, TValidationFailure> entityValidator)
        {
            _repository = repository;
            _entityValidator = entityValidator;
        }

        public override TValidationFailure[] ValidateCore()
        {
            var validationFailures = _repository.GetForValidation()
                .SelectMany(entity => _entityValidator.Validate(entity));

            return validationFailures.ToArray();
        }

        public override ValidationRuleDescription[] GetAllRuleDescriptions()
        {
            return _entityValidator.GetAllRuleDescriptions();
        }
    }
}