// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;
using MappingEdu.Core.Services.Validation;
using MappingEdu.Core.Services.Validation.NoteValidationRules;

namespace MappingEdu.Core.DataAccess.Services.Validation
{
    public class NoteValidateEntity : ValidateEntity<Note, NoteValidationFailure>
    {
        public NoteValidateEntity(IEntityValidator<Note, NoteValidationFailure> entityValidator) : base(entityValidator)
        {
        }
    }
}