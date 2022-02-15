// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation.MappingProjectValidationRules
{
    public class MappingProjectValidator : EntityValidator<MappingProject, MappingProjectValidationFailure>
    {
        public MappingProjectValidator(IRuleProvider<IValidationRule<MappingProject>, MappingProject> provider) : base(provider)
        {
        }

        protected override MappingProjectValidationFailure BuildFailure(MappingProject entity, FailureInfo failureInfo)
        {
            var failure = new MappingProjectValidationFailure
            {
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                FailingRule = failureInfo.RuleType,
                Id = entity.MappingProjectId,
                ProjectName = entity.ProjectName,
                Description = entity.Description
            };

            return failure;
        }
    }
}