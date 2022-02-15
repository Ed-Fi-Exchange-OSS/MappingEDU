// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemMustHaveItemType : IValidationRule<SystemItem>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemItem entity)
        {
            return entity.ItemType.Id != ItemType.Unknown.Id;
        }

        public string GetFailureMessage(SystemItem entity)
        {
            return "System Item, Item Type is not set.";
        }

        public string GetPropertyName(SystemItem entity)
        {
            return "ItemType";
        }
    }
}