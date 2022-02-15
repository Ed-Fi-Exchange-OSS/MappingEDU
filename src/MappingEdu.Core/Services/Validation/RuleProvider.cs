// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;

namespace MappingEdu.Core.Services.Validation
{
    public class RuleProvider<TValidationRule, TEntity> : IRuleProvider<TValidationRule, TEntity>
        where TValidationRule : IValidationRule<TEntity>
    {
        private readonly TValidationRule[] _validationRules;

        public RuleProvider(TValidationRule[] validationRules)
        {
            _validationRules = validationRules;
        }

        public TValidationRule[] GetAll()
        {
            return _validationRules;
        }

        public TValidationRule[] GetAddRules()
        {
            return _validationRules.Where(x => x is IAddValidationRule).ToArray();
        }

        public TValidationRule[] GetUpdateRules()
        {
            return _validationRules.Where(x => x is IUpdateValidationRule).ToArray();
        }
    }
}