// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Validation.SystemItemVersionDeltaValidationRules
{
    public class AddedSystemItemDeltaMustNotHaveOldSystemItem : IValidationRule<SystemItemVersionDelta>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(SystemItemVersionDelta entity)
        {
            if (entity.ItemChangeType != ItemChangeType.AddedDomain &&
                entity.ItemChangeType != ItemChangeType.AddedElement &&
                entity.ItemChangeType != ItemChangeType.AddedEntity)
                return true;

            return entity.OldSystemItemId == null;
        }

        public string GetFailureMessage(SystemItemVersionDelta entity)
        {
            return "An added version delta cannot have a previous version.";
        }

        public string GetPropertyName(SystemItemVersionDelta entity)
        {
            return null;
        }
    }
}