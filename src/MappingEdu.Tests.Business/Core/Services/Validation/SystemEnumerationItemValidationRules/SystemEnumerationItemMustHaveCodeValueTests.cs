// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    public class SystemEnumerationItemMustHaveCodeValueTests
    {
        [TestFixture]
        public class When_system_enumeration_item_has_code_value : TestBase
        {
            private SystemEnumerationItem _systemEnumerationItem;
            private SystemEnumerationItemMustHaveCodeValue _rule;

            [OneTimeSetUp]
            public void Setup()
            {
                _systemEnumerationItem = New.SystemEnumerationItem.WithCodeValue("FOO");
                _rule = new SystemEnumerationItemMustHaveCodeValue();
            }

            [Test]
            public void Should_be_valid()
            {
                _rule.IsValid(_systemEnumerationItem).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_system_enumeration_item_has_no_code_value : TestBase
        {
            private SystemEnumerationItem _systemEnumerationItem;
            private SystemEnumerationItemMustHaveCodeValue _rule;

            [OneTimeSetUp]
            public void Setup()
            {
                _systemEnumerationItem = New.SystemEnumerationItem;
                _rule = new SystemEnumerationItemMustHaveCodeValue();
            }

            [Test]
            public void Should_have_a_meaningful_failure_message()
            {
                _rule.GetFailureMessage(_systemEnumerationItem).ShouldEqual("Enumeration Code Value is not set.");
            }

            [Test]
            public void Should_not_be_valid()
            {
                _rule.IsValid(_systemEnumerationItem).ShouldBeFalse();
            }

            [Test]
            public void Should_provide_correct_property_name()
            {
                _rule.GetPropertyName(_systemEnumerationItem).ShouldEqual("CodeValue");
            }
        }
    }
}