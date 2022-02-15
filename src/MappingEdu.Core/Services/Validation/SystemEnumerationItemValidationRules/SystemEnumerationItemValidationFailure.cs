// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    public class SystemEnumerationItemValidationFailure : IValidationFailure
    {
        public string CodeValue { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public Guid SystemItemId { get; set; }

        public string SystemItemName { get; set; }

        public string FullMessage
        {
            get { return string.Format("System Enumeration Item [System Item Id: '{0}', System Enumeration Item Id: '{1}']: {2}", SystemItemId, Id, ValidationError); }
        }

        public string ShortMessage
        {
            get { return string.Format("'{0}' {1} for '{2}': {3}", CodeValue, Description ?? ShortDescription, SystemItemName, ValidationError); }
        }

        public Type FailingRule { get; set; }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id { get; set; }
    }
}