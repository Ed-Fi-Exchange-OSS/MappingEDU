// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation
{
    public abstract class EntityValidator<TEntity, TFailure> : IEntityValidator<TEntity, TFailure>
        where TEntity : IEntity
        where TFailure : IValidationFailure
    {
        private readonly IRuleProvider<IValidationRule<TEntity>, TEntity> _provider;

        protected EntityValidator(IRuleProvider<IValidationRule<TEntity>, TEntity> provider)
        {
            _provider = provider;
        }

        public bool IsValid(TEntity entity)
        {
            return _provider.GetAll().All(x => x.IsValid(entity));
        }

        public TFailure[] Validate(TEntity entity)
        {
            return _provider
                .GetAll()
                .Where(x => !x.IsValid(entity))
                .Select(x => BuildFailureInfo(x, entity))
                .Select(failureInfo => BuildFailure(entity, failureInfo))
                .ToArray();
        }

        public TFailure[] ValidateForAdd(TEntity entity)
        {
            return _provider
                .GetAddRules()
                .Where(x => !x.IsValid(entity))
                .Select(x => BuildFailureInfo(x, entity))
                .Select(failureInfo => BuildFailure(entity, failureInfo))
                .ToArray();
        }

        public TFailure[] ValidateForUpdate(TEntity entity)
        {
            return _provider
                .GetUpdateRules()
                .Where(x => !x.IsValid(entity))
                .Select(x => BuildFailureInfo(x, entity))
                .Select(failureInfo => BuildFailure(entity, failureInfo))
                .ToArray();
        }

        public ValidationRuleDescription[] GetAllRuleDescriptions()
        {
            return _provider.GetAll()
                .Select(
                    rule =>
                        new ValidationRuleDescription {Description = rule.GetType().Name.SpaceCamelHumps()})
                .ToArray();
        }

        private static FailureInfo BuildFailureInfo(IValidationRule<TEntity> rule, TEntity entity)
        {
            return new FailureInfo
            {
                RuleType = rule.GetType(),
                Message = rule.GetFailureMessage(entity),
                PropertyName = rule.GetPropertyName(entity)
            };
        }

        protected abstract TFailure BuildFailure(TEntity entity, FailureInfo failureInfo);

        protected class FailureInfo
        {
            public string Message { get; set; }

            public string PropertyName { get; set; }

            public Type RuleType { get; set; }
        }
    }
}