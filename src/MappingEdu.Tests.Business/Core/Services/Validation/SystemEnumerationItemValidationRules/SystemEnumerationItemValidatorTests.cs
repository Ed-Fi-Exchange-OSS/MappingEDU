// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation;
using MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    public class SystemEnumerationItemValidatorTests
    {
        [TestFixture]
        public class When_system_enumeration_item_is_valid_for_all_rules : TestBase
        {
            private SystemEnumerationItemValidationFailure[] _result;
            private bool _isValid;

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemEnumerationItem>[] rules = {(SystemEnumerationItemRuleStub) New.SystemEnumerationItemRule.AlwaysValid};
                var provider = new SystemEnumerationItemRuleProvider(rules);
                var validator = new SystemEnumerationItemValidator(provider);

                SystemEnumerationItem enumeration = New.SystemEnumerationItem;

                _result = validator.Validate(enumeration);
                _isValid = validator.IsValid(enumeration);
            }

            [Test]
            public void Should_report_is_valid()
            {
                _isValid.ShouldBeTrue();
            }

            [Test]
            public void Should_report_zero_failures()
            {
                _result.Length.ShouldEqual(0);
            }
        }

        [TestFixture]
        public class When_system_item_is_valid_for_two_and_invalid_for_two_other_rules
        {
            private class TestRuleOne : SystemEnumerationItemRuleStub
            {
                public TestRuleOne()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Failed 1";
                }
            }

            private class TestRuleTwo : SystemEnumerationItemRuleStub
            {
                public TestRuleTwo()
                {
                    StubIsValid = true;
                    StubFailureMessage = "Passed 2";
                }
            }

            private SystemEnumerationItemValidationFailure[] _result;
            private bool _isValid;
            private SystemEnumerationItem _systemEnumerationItem;
            private const string codeValue = "CODVAL";
            private const string description = "Description";
            private const string shortDescription = "Short";

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemEnumerationItem>[] rules =
                {
                    new TestRuleOne(),
                    new TestRuleTwo(),
                    (SystemEnumerationItemRuleStub) New.SystemEnumerationItemRule.WithFailureMessage("Failed 2"),
                    (SystemEnumerationItemRuleStub) New.SystemEnumerationItemRule.AlwaysValid
                };

                var provider = new SystemEnumerationItemRuleProvider(rules);
                var validator = new SystemEnumerationItemValidator(provider);

                _systemEnumerationItem = New.SystemEnumerationItem
                    .WithCodeValue(codeValue)
                    .WithDescription(description)
                    .WithShortDescription(shortDescription);

                _result = validator.Validate(_systemEnumerationItem);
                _isValid = validator.IsValid(_systemEnumerationItem);
            }

            [Test]
            public void Should_include_code_value_in_failure()
            {
                _result.All(x => x.CodeValue == codeValue).ShouldBeTrue();
            }

            [Test]
            public void Should_include_description_value_in_failure()
            {
                _result.All(x => x.Description == description).ShouldBeTrue();
            }

            [Test]
            public void Should_include_failed_rule_types_in_failure()
            {
                _result.Any(x => x.FailingRule == typeof (TestRuleOne)).ShouldBeTrue();
                _result.Any(x => x.FailingRule == typeof (SystemEnumerationItemRuleStub)).ShouldBeTrue();
                _result.Any(x => x.FailingRule == typeof (TestRuleTwo)).ShouldBeFalse();
            }

            [Test]
            public void Should_include_message_for_failing_rules()
            {
                _result.Any(x => x.ValidationError == "Failed 1").ShouldBeTrue();
                _result.Any(x => x.ValidationError == "Failed 2").ShouldBeTrue();
            }

            [Test]
            public void Should_include_short_description_value_in_failure()
            {
                _result.All(x => x.ShortDescription == shortDescription).ShouldBeTrue();
            }

            [Test]
            public void Should_include_system_enumeration_item_id_in_failure()
            {
                _result.All(x => x.Id == _systemEnumerationItem.SystemEnumerationItemId).ShouldBeTrue();
            }

            [Test]
            public void Should_include_system_item_id_in_failure()
            {
                _result.All(x => x.SystemItemId == _systemEnumerationItem.SystemItemId).ShouldBeTrue();
            }

            [Test]
            public void Should_report_is_not_valid()
            {
                _isValid.ShouldBeFalse();
            }

            [Test]
            public void Should_report_two_failures()
            {
                _result.Length.ShouldEqual(2);
            }
        }

        [TestFixture]
        public class When_using_multiple_rules : TestBase
        {
            private class TestRuleA : SystemEnumerationItemRuleStub
            {
            }

            private ValidationRuleDescription[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemEnumerationItem>[] rules =
                {
                    (SystemEnumerationItemRuleStub) New.SystemEnumerationItemRule.AlwaysValid,
                    (SystemEnumerationItemRuleStub) New.SystemEnumerationItemRule.WithFailureMessage("Failed 1"),
                    new TestRuleA()
                };

                var provider = new SystemEnumerationItemRuleProvider(rules);
                var validator = new SystemEnumerationItemValidator(provider);

                _result = validator.GetAllRuleDescriptions();
            }

            [Test]
            public void Should_report_validation_descriptions_for_each_rule()
            {
                _result.Length.ShouldEqual(3);
                _result.Select(x => x.Description).ShouldContain("System Enumeration Item Rule Stub");
                _result.Select(x => x.Description).ShouldContain("Test Rule A");
            }
        }

        [TestFixture]
        public class When_validating_entity_for_add : TestBase
        {
            private SystemEnumerationItem _systemEnumerationItem;
            private SystemEnumerationItemValidationFailure[] _result;
            private bool _isValid;

            private class TestRuleAdd : SystemEnumerationItemRuleStub, IAddValidationRule
            {
                public TestRuleAdd()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Add Fail.";
                }
            }

            private class TestRuleUpdate : SystemEnumerationItemRuleStub, IUpdateValidationRule
            {
                public TestRuleUpdate()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Update Fail.";
                }
            }

            private class TestRuleAddValid : SystemEnumerationItemRuleStub, IAddValidationRule
            {
                public TestRuleAddValid()
                {
                    StubIsValid = true;
                    StubFailureMessage = "Add Success.";
                }
            }

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemEnumerationItem>[] rules =
                {
                    new TestRuleAdd(),
                    new TestRuleUpdate(),
                    new TestRuleAddValid()
                };

                var provider = new SystemEnumerationItemRuleProvider(rules);
                var validator = new SystemEnumerationItemValidator(provider);

                _systemEnumerationItem = New.SystemEnumerationItem;

                _result = validator.ValidateForAdd(_systemEnumerationItem);
                _isValid = validator.IsValid(_systemEnumerationItem);
            }

            [Test]
            public void Should_include_message_for_failing_rule()
            {
                _result.Any(x => x.ValidationError == "Add Fail.").ShouldBeTrue();
            }

            [Test]
            public void Should_include_rule_type_in_failure()
            {
                _result.Any(x => x.FailingRule == typeof (TestRuleAdd)).ShouldBeTrue();
            }

            [Test]
            public void Should_report_single_failure()
            {
                _isValid.ShouldBeFalse();
                _result.Length.ShouldEqual(1);
            }
        }

        [TestFixture]
        public class When_validating_entity_for_update : TestBase
        {
            private SystemEnumerationItem _systemEnumerationItem;
            private SystemEnumerationItemValidationFailure[] _result;
            private bool _isValid;

            private class TestRuleAdd : SystemEnumerationItemRuleStub, IAddValidationRule
            {
                public TestRuleAdd()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Add Fail.";
                }
            }

            private class TestRuleUpdate : SystemEnumerationItemRuleStub, IUpdateValidationRule
            {
                public TestRuleUpdate()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Update Fail.";
                }
            }

            private class TestRuleAddValid : SystemEnumerationItemRuleStub, IAddValidationRule
            {
                public TestRuleAddValid()
                {
                    StubIsValid = true;
                    StubFailureMessage = "Add Success.";
                }
            }

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemEnumerationItem>[] rules =
                {
                    new TestRuleAdd(),
                    new TestRuleUpdate(),
                    new TestRuleAddValid()
                };

                var provider = new SystemEnumerationItemRuleProvider(rules);
                var validator = new SystemEnumerationItemValidator(provider);

                _systemEnumerationItem = New.SystemEnumerationItem;

                _result = validator.ValidateForUpdate(_systemEnumerationItem);
                _isValid = validator.IsValid(_systemEnumerationItem);
            }

            [Test]
            public void Should_include_message_for_failing_rule()
            {
                _result.Any(x => x.ValidationError == "Update Fail.").ShouldBeTrue();
            }

            [Test]
            public void Should_include_rule_type_in_failure()
            {
                _result.Any(x => x.FailingRule == typeof (TestRuleUpdate)).ShouldBeTrue();
            }

            [Test]
            public void Should_report_single_failure()
            {
                _isValid.ShouldBeFalse();
                _result.Length.ShouldEqual(1);
            }
        }
    }
}