// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Tests.Business.Core.Services.Validation.SystemEnumerationItemValidationRules;

namespace MappingEdu.Tests.Business.Builders
{
    public class SystemEnumerationItemRuleBuilder
    {
        private readonly SystemEnumerationItemRuleStub _systemEnumerationItemRuleStub = new SystemEnumerationItemRuleStub();

        public SystemEnumerationItemRuleBuilder AlwaysValid
        {
            get
            {
                _systemEnumerationItemRuleStub.StubIsValid = true;
                return this;
            }
        }

        public static implicit operator SystemEnumerationItemRuleStub(SystemEnumerationItemRuleBuilder builder)
        {
            return builder._systemEnumerationItemRuleStub;
        }

        public SystemEnumerationItemRuleBuilder WithFailureMessage(string message)
        {
            _systemEnumerationItemRuleStub.StubIsValid = false;
            _systemEnumerationItemRuleStub.StubFailureMessage = message;
            return this;
        }
    }
}