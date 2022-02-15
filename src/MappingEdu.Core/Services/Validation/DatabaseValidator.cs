// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;

namespace MappingEdu.Core.Services.Validation
{
    public class DatabaseValidator : IDatabaseValidator
    {
        private readonly IValidatorProvider _provider;

        public DatabaseValidator(IValidatorProvider provider)
        {
            _provider = provider;
        }

        public ValidationResult Validate()
        {
            var failures = _provider.GetAllValidators().SelectMany(x => x.Validate());
            var result = new ValidationResult(failures.ToArray(), GetAllRuleDescriptions());
            result.ValidationRuleDescriptions = GetAllRuleDescriptions();
            return result;
        }

        private ValidationRuleDescription[] GetAllRuleDescriptions()
        {
            return _provider.GetAllValidators().SelectMany(x => x.GetAllRuleDescriptions()).ToArray();
        }
    }
}