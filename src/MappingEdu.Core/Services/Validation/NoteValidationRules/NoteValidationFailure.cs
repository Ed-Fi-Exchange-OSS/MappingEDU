// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation.NoteValidationRules
{
    public class NoteValidationFailure : IValidationFailure
    {
        public string Notes { get; set; }

        public string Title { get; set; }

        public string FullMessage
        {
            get
            {
                return string.Format("Note [Note Id: '{0}', Note Title: '{1}', Notes: '{2}']: {3}", Id, Title, Notes,
                    ValidationError);
            }
        }

        public string ShortMessage
        {
            get { return string.Format("Note for '{0}': {1}", Title, ValidationError); }
        }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id { get; set; }

        public Type FailingRule { get; set; }
    }
}