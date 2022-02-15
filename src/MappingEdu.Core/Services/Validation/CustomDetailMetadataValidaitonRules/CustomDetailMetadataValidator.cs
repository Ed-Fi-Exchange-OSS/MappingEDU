// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation.CustomDetailMetadataValidaitonRules
{
    public class CustomDetailMetadataValidator : EntityValidator<CustomDetailMetadata, CustomDetailMetadtaValidationFailure>
    {
        public CustomDetailMetadataValidator(IRuleProvider<IValidationRule<CustomDetailMetadata>, CustomDetailMetadata> provider)
            : base(provider)
        {
        }

        protected override CustomDetailMetadtaValidationFailure BuildFailure(CustomDetailMetadata customDetailMetadata, FailureInfo failureInfo)
        {
            return new CustomDetailMetadtaValidationFailure
            {
                FailingRule = failureInfo.RuleType,
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                Id = customDetailMetadata.CustomDetailMetadataId,
                MappedSystemId = customDetailMetadata.MappedSystemId,
                MappedSystemName = null != customDetailMetadata.MappedSystem ? customDetailMetadata.MappedSystem.SystemName : string.Empty,
                CustomDetailDisplayName = customDetailMetadata.DisplayName
            };
        }
    }
}