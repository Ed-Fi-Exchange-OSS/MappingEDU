// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    public class SystemEnumerationItemMustHaveCodeValue : IValidationRule<SystemEnumerationItem>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemEnumerationItem entity)
        {
            return !string.IsNullOrWhiteSpace(entity.CodeValue);
        }

        public string GetFailureMessage(SystemEnumerationItem entity)
        {
            return "Enumeration Code Value is not set.";
        }

        public string GetPropertyName(SystemEnumerationItem entity)
        {
            return "CodeValue";
        }
    }
}