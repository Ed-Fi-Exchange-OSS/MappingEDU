// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation
{
    public class BasicValidationFailure : IValidationFailure
    {
        public string FullMessage
        {
            get { return ValidationError; }
        }

        public string ShortMessage
        {
            get { return FullMessage; }
        }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id
        {
            get
            {
                var message = string.Format("If you need an Id, you should create a specific type of {0}",
                    typeof (IValidationFailure).Name);
                throw new Exception(message);
            }
        }

        public Type FailingRule { get; set; }
    }
}