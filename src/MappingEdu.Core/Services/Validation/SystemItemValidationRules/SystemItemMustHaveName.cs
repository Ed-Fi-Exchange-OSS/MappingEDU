// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemMustHaveName : IValidationRule<SystemItem>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemItem systemItem)
        {
            return !string.IsNullOrWhiteSpace(systemItem.ItemName);
        }

        public string GetFailureMessage(SystemItem systemItem)
        {
            return string.Format("{0} Item Name is not set.", systemItem.ItemType.Name);
        }

        public string GetPropertyName(SystemItem systemItem)
        {
            return "ItemName";
        }
    }
}