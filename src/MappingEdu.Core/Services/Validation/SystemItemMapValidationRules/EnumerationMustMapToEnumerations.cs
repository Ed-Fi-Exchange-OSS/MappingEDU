// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemMapValidationRules
{
    public class EnumerationMustMapToEnumerations : IValidationRule<SystemItemMap>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemItemMap entity)
        {
            if (entity.SourceSystemItem.ItemType != ItemType.Enumeration)
                return true;

            return !entity.TargetSystemItems.Any(x => x.ItemType != ItemType.Enumeration);
        }

        public string GetFailureMessage(SystemItemMap entity)
        {
            return "Enumerations can only map to other enumerations.";
        }

        public string GetPropertyName(SystemItemMap entity)
        {
            return null;
        }
    }
}