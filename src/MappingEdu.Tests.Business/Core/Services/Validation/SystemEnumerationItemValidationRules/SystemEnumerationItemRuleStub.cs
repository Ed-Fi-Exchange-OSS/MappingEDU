// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    public class SystemEnumerationItemRuleStub : IValidationRule<SystemEnumerationItem>
    {
        public string StubFailureMessage { private get; set; }

        public bool StubIsValid { private get; set; }

        public string StubPropertyName { private get; set; }

        public bool IsValid(SystemEnumerationItem systemItem)
        {
            return StubIsValid;
        }

        public string GetFailureMessage(SystemEnumerationItem systemItem)
        {
            return StubFailureMessage;
        }

        public string GetPropertyName(SystemEnumerationItem systemItem)
        {
            return StubPropertyName;
        }
    }
}