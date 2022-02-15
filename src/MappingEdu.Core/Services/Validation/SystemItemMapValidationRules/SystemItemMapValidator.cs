// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemMapValidationRules
{
    public class SystemItemMapValidator : EntityValidator<SystemItemMap, SystemItemMapValidationFailure>
    {
        public SystemItemMapValidator(IRuleProvider<IValidationRule<SystemItemMap>, SystemItemMap> provider) : base(provider)
        {
        }

        protected override SystemItemMapValidationFailure BuildFailure(SystemItemMap entity, FailureInfo failureInfo)
        {
            var failure = new SystemItemMapValidationFailure
            {
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                FailingRule = failureInfo.RuleType,
                Id = entity.SystemItemMapId
            };

            return failure;
        }
    }
}