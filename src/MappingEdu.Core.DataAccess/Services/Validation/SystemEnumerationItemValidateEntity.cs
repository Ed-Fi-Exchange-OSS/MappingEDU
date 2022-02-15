// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation;
using MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules;

namespace MappingEdu.Core.DataAccess.Services.Validation
{
    public class SystemEnumerationItemValidateEntity : ValidateEntity<SystemEnumerationItem, SystemEnumerationItemValidationFailure>
    {
        public SystemEnumerationItemValidateEntity(IEntityValidator<SystemEnumerationItem, SystemEnumerationItemValidationFailure> entityValidator)
            : base(entityValidator)
        {
        }
    }
}