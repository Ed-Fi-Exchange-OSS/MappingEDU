// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation.CustomDetailMetadataValidaitonRules
{
    public class CustomDetailMetadtaValidationFailure : IValidationFailure
    {
        public string CustomDetailDisplayName { get; set; }

        public Guid MappedSystemId { get; set; }

        public string MappedSystemName { get; set; }

        public string FullMessage
        {
            get { return string.Format("Custom Detail Metadata [Mapped System Id: '{0}', Custom Detail Metadata Id: '{1}']: {2}", MappedSystemId, Id, ValidationError); }
        }

        public string ShortMessage
        {
            get { return string.Format("'{0}' for '{1}': {2}", CustomDetailDisplayName, MappedSystemName, ValidationError); }
        }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id { get; set; }

        public Type FailingRule { get; set; }
    }
}