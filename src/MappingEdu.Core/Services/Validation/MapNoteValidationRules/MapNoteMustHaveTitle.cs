// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation.MapNoteValidationRules
{
    public class MapNoteMustHaveTitle : IValidationRule<MapNote>, IAddValidationRule, IUpdateValidationRule
    {
        public bool IsValid(MapNote entity)
        {
            return !string.IsNullOrWhiteSpace(entity.Title);
        }

        public string GetFailureMessage(MapNote entity)
        {
            return "The map note title is required.";
        }

        public string GetPropertyName(MapNote entity)
        {
            return "Title";
        }
    }
}