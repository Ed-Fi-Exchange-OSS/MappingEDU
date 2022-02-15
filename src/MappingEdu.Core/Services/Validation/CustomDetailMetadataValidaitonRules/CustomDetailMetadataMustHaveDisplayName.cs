// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation.CustomDetailMetadataValidaitonRules
{
    public class CustomDetailMetadataMustHaveDisplayName : IValidationRule<CustomDetailMetadata>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(CustomDetailMetadata customDetailMetadata)
        {
            return !string.IsNullOrEmpty(customDetailMetadata.DisplayName);
        }

        public string GetFailureMessage(CustomDetailMetadata customDetailMetadata)
        {
            return "Custom Detail Metadata Display Name is required.";
        }

        public string GetPropertyName(CustomDetailMetadata entity)
        {
            return "DisplayName";
        }
    }
}