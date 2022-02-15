// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation;
using MappingEdu.Core.Services.Validation.SystemItemVersionDeltaValidationRules;

namespace MappingEdu.Core.DataAccess.Services.Validation
{
    public class SystemItemVersionDeltaValidateEntity : ValidateEntity<SystemItemVersionDelta, SystemItemVersionDeltaValidationFailure>
    {
        public SystemItemVersionDeltaValidateEntity(IEntityValidator<SystemItemVersionDelta, SystemItemVersionDeltaValidationFailure> entityValidator) : base(entityValidator)
        {
        }
    }
}