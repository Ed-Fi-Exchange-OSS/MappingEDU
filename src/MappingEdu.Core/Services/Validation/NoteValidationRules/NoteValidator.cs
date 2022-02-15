// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation.NoteValidationRules
{
    public class NoteValidator : EntityValidator<Note, NoteValidationFailure>
    {
        public NoteValidator(IRuleProvider<IValidationRule<Note>, Note> provider) : base(provider)
        {
        }

        protected override NoteValidationFailure BuildFailure(Note entity, FailureInfo failureInfo)
        {
            var failure = new NoteValidationFailure
            {
                ValidationError = failureInfo.Message,
                PropertyName = failureInfo.PropertyName,
                FailingRule = failureInfo.RuleType,
                Id = entity.NoteId,
                Title = entity.Title,
                Notes = entity.Notes
            };

            return failure;
        }
    }
}