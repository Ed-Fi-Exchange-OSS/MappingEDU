// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Tests.Business.Core.Services.Validation.SystemItemValidationRules;

namespace MappingEdu.Tests.Business.Builders
{
    public class SystemItemRuleBuilder
    {
        private readonly SystemItemRuleStub _systemItemRule = new SystemItemRuleStub();

        public SystemItemRuleBuilder AlwaysValid
        {
            get
            {
                _systemItemRule.StubIsValid = true;
                return this;
            }
        }

        public static implicit operator SystemItemRuleStub(SystemItemRuleBuilder builder)
        {
            return builder._systemItemRule;
        }

        public SystemItemRuleBuilder WithFailureMessage(string message)
        {
            _systemItemRule.StubIsValid = false;
            _systemItemRule.StubFailureMessage = message;
            return this;
        }
    }
}