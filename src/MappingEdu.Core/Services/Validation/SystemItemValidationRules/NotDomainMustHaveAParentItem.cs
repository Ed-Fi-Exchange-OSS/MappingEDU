// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Models;
using ItemType = MappingEdu.Core.Models.Enumerations.ItemType;

namespace MappingEdu.Core.Services.Validation.SystemItemValidationRules
{
    public class NotDomainMustHaveAParentItem : IValidationRule<SystemItem>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemItem entity)
        {
            return entity.ItemType.Id == ItemType.Domain.Id ||
                (null != entity.ParentSystemItemId && null != entity.ParentSystemItem);
        }

        public string GetFailureMessage(SystemItem entity)
        {
            return string.Format("{0} System Items must have a Parent.", entity.ItemType.Name);
        }

        public string GetPropertyName(SystemItem entity)
        {
            return "ParentSystemItemId";
        }
    }
}
