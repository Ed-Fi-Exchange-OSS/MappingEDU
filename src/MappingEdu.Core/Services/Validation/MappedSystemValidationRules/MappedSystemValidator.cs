// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation.MappedSystemValidationRules
{
    public class MappedSystemValidator : EntityValidator<MappedSystem, MappedSystemValidationFailure>
    {
        public MappedSystemValidator(IRuleProvider<IValidationRule<MappedSystem>, MappedSystem> provider)
            : base(provider)
        {
        }

        protected override MappedSystemValidationFailure BuildFailure(MappedSystem mappedSystem, FailureInfo failureInfo)
        {
            var failure = new MappedSystemValidationFailure
            {
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                FailingRule = failureInfo.RuleType,
                Id = mappedSystem.MappedSystemId,
                MappedSystemName = mappedSystem.SystemName,
                MappedSystemVersion = mappedSystem.SystemVersion,
                PreviousMappedSystemId = mappedSystem.PreviousMappedSystemId,
                IsActive = mappedSystem.IsActive
            };

            return failure;
        }
    }
}