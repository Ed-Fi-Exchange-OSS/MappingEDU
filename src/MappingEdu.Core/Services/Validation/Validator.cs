// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;

namespace MappingEdu.Core.Services.Validation
{
    public abstract class Validator<TFailure> : IValidator where TFailure : IValidationFailure
    {
        public IValidationFailure[] Validate()
        {
            return ValidateCore().Cast<IValidationFailure>().ToArray();
        }

        public abstract ValidationRuleDescription[] GetAllRuleDescriptions();

        public abstract TFailure[] ValidateCore();
    }
}