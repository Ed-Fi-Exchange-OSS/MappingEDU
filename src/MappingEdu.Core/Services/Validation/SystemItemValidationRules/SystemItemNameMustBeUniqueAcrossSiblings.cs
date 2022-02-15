// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemNameMustBeUniqueAcrossSiblings : IValidationRule<SystemItem>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemItem entity)
        {
            if (entity.ParentSystemItem == null)
                return true;

            return entity.ParentSystemItem.ChildSystemItems.Count(x => x.ItemName == entity.ItemName) == 1;
        }

        public string GetFailureMessage(SystemItem entity)
        {
            return string.Format("Item with name '{0}' already exists for parent item '{1}'.", entity.ItemName, entity.ParentSystemItem.ItemName);
        }

        public string GetPropertyName(SystemItem entity)
        {
            return "ItemName";
        }
    }
}