// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation.MappingProjectValidationRules
{
    public class MappingProjectMustHaveName : IValidationRule<MappingProject>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(MappingProject entity)
        {
            return !string.IsNullOrWhiteSpace(entity.ProjectName);
        }

        public string GetFailureMessage(MappingProject entity)
        {
            return "Project Name is a required field.";
        }

        public string GetPropertyName(MappingProject entity)
        {
            return "ProjectName";
        }
    }
}