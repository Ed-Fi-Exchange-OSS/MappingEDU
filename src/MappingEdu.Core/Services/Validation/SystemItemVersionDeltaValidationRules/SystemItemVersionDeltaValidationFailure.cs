// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation.SystemItemVersionDeltaValidationRules
{
    public class SystemItemVersionDeltaValidationFailure : IValidationFailure
    {
        public string FullMessage
        {
            get { return string.Format("System Item Version Delta '{0}': {1}", Id, ValidationError); }
        }

        public string ShortMessage
        {
            get { return ValidationError; }
        }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id { get; set; }

        public Type FailingRule { get; set; }
    }
}