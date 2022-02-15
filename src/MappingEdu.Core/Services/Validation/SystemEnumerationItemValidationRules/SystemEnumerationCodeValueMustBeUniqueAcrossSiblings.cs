// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    public class SystemEnumerationCodeValueMustBeUniqueAcrossSiblings : IValidationRule<SystemEnumerationItem>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemEnumerationItem entity)
        {
            return entity.SystemItem.SystemEnumerationItems.Count(x => x.CodeValue == entity.CodeValue) == 1;
        }

        public string GetFailureMessage(SystemEnumerationItem entity)
        {
            return string.Format("Code Value '{0}' already exists for enumeration '{1}'.", entity.CodeValue, entity.SystemItem.ItemName);
        }

        public string GetPropertyName(SystemEnumerationItem entity)
        {
            return "CodeValue";
        }
    }
}