// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemVersionDeltaValidationRules
{
    public class DeletedSystemItemDeltaMustNotHaveNewSystemItem : IValidationRule<SystemItemVersionDelta>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemItemVersionDelta entity)
        {
            if (entity.ItemChangeType != ItemChangeType.DeletedElement &&
                entity.ItemChangeType != ItemChangeType.DeletedEntity)
                return true;

            return entity.NewSystemItemId == null;
        }

        public string GetFailureMessage(SystemItemVersionDelta entity)
        {
            return "An deleted version delta cannot have a next version.";
        }

        public string GetPropertyName(SystemItemVersionDelta entity)
        {
            return null;
        }
    }
}