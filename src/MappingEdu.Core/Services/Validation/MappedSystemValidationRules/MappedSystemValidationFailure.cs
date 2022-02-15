// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation.MappedSystemValidationRules
{
    public class MappedSystemValidationFailure : IValidationFailure
    {
        public bool IsActive { get; set; }

        // MappedSystem specific properties
        public string MappedSystemName { get; set; }

        public string MappedSystemVersion { get; set; }

        public Guid? PreviousMappedSystemId { get; set; }

        public string FullMessage
        {
            get
            {
                return
                    string.Format(
                        "Mapped System [Mapped System Id: '{0}', Mapped System Name: '{1}', Mapped System Version: '{2}', Previous Mapping System Id: '{3}', Is Active: '{4}']: {5}",
                        Id, MappedSystemName, MappedSystemVersion, PreviousMappedSystemId.GetValueOrDefault(), IsActive,
                        ValidationError);
            }
        }

        public string ShortMessage
        {
            get { return string.Format("Mapped System for '{0}': {1}", MappedSystemName, ValidationError); }
        }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id { get; set; }

        public Type FailingRule { get; set; }
    }
}