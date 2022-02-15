// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemVersionDeltaValidationRules
{
    public class SystemItemVersionDeltaValidator : EntityValidator<SystemItemVersionDelta, SystemItemVersionDeltaValidationFailure>
    {
        public SystemItemVersionDeltaValidator(IRuleProvider<IValidationRule<SystemItemVersionDelta>, SystemItemVersionDelta> provider) : base(provider)
        {
        }

        protected override SystemItemVersionDeltaValidationFailure BuildFailure(SystemItemVersionDelta entity, FailureInfo failureInfo)
        {
            var failure = new SystemItemVersionDeltaValidationFailure
            {
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                FailingRule = failureInfo.RuleType,
                Id = entity.SystemItemVersionDeltaId
            };

            return failure;
        }
    }
}