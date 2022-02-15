// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemValidator : EntityValidator<SystemItem, SystemItemValidationFailure>
    {
        public SystemItemValidator(IRuleProvider<IValidationRule<SystemItem>, SystemItem> provider)
            : base(provider)
        {
        }

        protected override SystemItemValidationFailure BuildFailure(SystemItem systemItem, FailureInfo failureInfo)
        {
            return new SystemItemValidationFailure
            {
                FailingRule = failureInfo.RuleType,
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                Id = systemItem.SystemItemId,
                MappedSystemId = systemItem.MappedSystemId,
                MappedSystemName = null != systemItem.MappedSystem ? systemItem.MappedSystem.SystemName : string.Empty,
                SystemItemName = systemItem.ItemName,
                SystemItemType = systemItem.ItemType
            };
        }
    }
}