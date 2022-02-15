// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation.MappingProjectValidationRules
{
    public class MappingProjectValidationFailure : IValidationFailure
    {
        public string Description { get; set; }

        public string ProjectName { get; set; }

        public string FullMessage
        {
            get { return string.Format("Mapping Project [Mapping Project Id: '{0}', Name: '{1}', Description: '{2}']: {3}", Id, ProjectName, Description, ValidationError); }
        }

        public string ShortMessage
        {
            get { return string.Format("Mapping Project for '{0}': {1}", ProjectName, ValidationError); }
        }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id { get; set; }

        public Type FailingRule { get; set; }
    }
}