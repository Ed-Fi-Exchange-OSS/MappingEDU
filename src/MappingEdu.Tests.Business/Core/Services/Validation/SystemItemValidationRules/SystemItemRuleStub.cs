// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemRuleStub : IValidationRule<SystemItem>
    {
        public string StubFailureMessage { private get; set; }

        public bool StubIsValid { private get; set; }

        public string StubPropertyName { private get; set; }

        public bool IsValid(SystemItem systemItem)
        {
            return StubIsValid;
        }

        public string GetFailureMessage(SystemItem systemItem)
        {
            return StubFailureMessage;
        }

        public string GetPropertyName(SystemItem systemItem)
        {
            return StubPropertyName;
        }
    }
}