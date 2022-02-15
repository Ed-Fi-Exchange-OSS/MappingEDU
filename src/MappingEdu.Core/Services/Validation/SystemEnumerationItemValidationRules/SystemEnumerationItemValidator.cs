// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    public class SystemEnumerationItemValidator : EntityValidator<SystemEnumerationItem, SystemEnumerationItemValidationFailure>
    {
        public SystemEnumerationItemValidator(IRuleProvider<IValidationRule<SystemEnumerationItem>, SystemEnumerationItem> provider)
            : base(provider)
        {
        }

        protected override SystemEnumerationItemValidationFailure BuildFailure(SystemEnumerationItem entity, FailureInfo failureInfo)
        {
            return new SystemEnumerationItemValidationFailure
            {
                FailingRule = failureInfo.RuleType,
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                Id = entity.SystemEnumerationItemId,
                SystemItemId = entity.SystemItemId,
                SystemItemName = null != entity.SystemItem ? entity.SystemItem.ItemName : string.Empty,
                CodeValue = entity.CodeValue,
                Description = entity.Description,
                ShortDescription = entity.ShortDescription
            };
        }
    }
}