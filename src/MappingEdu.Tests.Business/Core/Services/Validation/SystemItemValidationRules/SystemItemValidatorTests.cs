// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation;
using MappingEdu.Core.Services.Validation.SystemItemValidationRules;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemValidatorTests
    {
        [TestFixture]
        public class When_system_item_is_valid_for_all_rules : TestBase
        {
            private SystemItemValidationFailure[] _result;
            private bool _isValid;

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemItem>[] rules = {(SystemItemRuleStub) New.SystemItemRule.AlwaysValid};

                var provider = new SystemItemRuleProvider(rules);
                var validator = new SystemItemValidator(provider);

                SystemItem entity = New.SystemItem;

                _result = validator.Validate(entity);
                _isValid = validator.IsValid(entity);
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
            private class TestRuleOne : SystemItemRuleStub
            {
                public TestRuleOne()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Failed 1";
                }
            }

            private class TestRuleTwo : SystemItemRuleStub
            {
                public TestRuleTwo()
                {
                    StubIsValid = true;
                    StubFailureMessage = "Passed 2";
                }
            }

            private SystemItemValidationFailure[] _result;
            private bool _isValid;
            private SystemItem _systemItem;

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemItem>[] rules =
                {
                    new TestRuleOne(),
                    new TestRuleTwo(),
                    (SystemItemRuleStub) New.SystemItemRule.WithFailureMessage("Failed 2"),
                    (SystemItemRuleStub) New.SystemItemRule.AlwaysValid
                };

                var provider = new SystemItemRuleProvider(rules);
                var validator = new SystemItemValidator(provider);

                _systemItem = New.SystemItem.AsDomain.WithMappedSystem(New.MappedSystem);

                _result = validator.Validate(_systemItem);
                _isValid = validator.IsValid(_systemItem);
            }

            [Test]
            public void Should_include_failed_rule_types_in_failure()
            {
                _result.Any(x => x.FailingRule == typeof (TestRuleOne)).ShouldBeTrue();
                _result.Any(x => x.FailingRule == typeof (SystemItemRuleStub)).ShouldBeTrue();
                _result.Any(x => x.FailingRule == typeof (TestRuleTwo)).ShouldBeFalse();
            }

            [Test]
            public void Should_include_item_type_in_failure()
            {
                _result.All(x => x.SystemItemType.Id == ItemType.Domain.Id).ShouldBeTrue();
            }

            [Test]
            public void Should_include_mapped_system_id_in_failure()
            {
                _result.All(x => x.MappedSystemId == _systemItem.MappedSystemId).ShouldBeTrue();
            }

            [Test]
            public void Should_include_message_for_failing_rules()
            {
                _result.Any(x => x.ValidationError == "Failed 1").ShouldBeTrue();
                _result.Any(x => x.ValidationError == "Failed 2").ShouldBeTrue();
            }

            [Test]
            public void Should_include_system_item_id_in_failure()
            {
                _result.All(x => x.Id == _systemItem.SystemItemId).ShouldBeTrue();
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
            private class TestRuleA : SystemItemRuleStub
            {
            }

            private ValidationRuleDescription[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                IValidationRule<SystemItem>[] rules =
                {
                    (SystemItemRuleStub) New.SystemItemRule.AlwaysValid,
                    (SystemItemRuleStub) New.SystemItemRule.WithFailureMessage("Failed 1"),
                    new TestRuleA()
                };

                var provider = new SystemItemRuleProvider(rules);
                var validator = new SystemItemValidator(provider);

                _result = validator.GetAllRuleDescriptions();
            }

            [Test]
            public void Should_report_validation_descriptions_for_each_rule()
            {
                _result.Length.ShouldEqual(3);
                _result.Select(x => x.Description).ShouldContain("System Item Rule Stub");
                _result.Select(x => x.Description).ShouldContain("Test Rule A");
            }
        }

        [TestFixture]
        public class When_validating_entity_for_add : TestBase
        {
            private SystemItem _systemItem;
            private SystemItemValidationFailure[] _result;
            private bool _isValid;

            private class TestRuleAdd : SystemItemRuleStub, IAddValidationRule
            {
                public TestRuleAdd()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Add Fail.";
                }
            }

            private class TestRuleUpdate : SystemItemRuleStub, IUpdateValidationRule
            {
                public TestRuleUpdate()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Update Fail.";
                }
            }

            private class TestRuleAddValid : SystemItemRuleStub, IAddValidationRule
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
                IValidationRule<SystemItem>[] rules =
                {
                    new TestRuleAdd(),
                    new TestRuleUpdate(),
                    new TestRuleAddValid()
                };

                var provider = new SystemItemRuleProvider(rules);
                var validator = new SystemItemValidator(provider);

                _systemItem = New.SystemItem.AsDomain.WithMappedSystem(New.MappedSystem);

                _result = validator.ValidateForAdd(_systemItem);
                _isValid = validator.IsValid(_systemItem);
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
            private SystemItem _systemItem;
            private SystemItemValidationFailure[] _result;
            private bool _isValid;

            private class TestRuleAdd : SystemItemRuleStub, IAddValidationRule
            {
                public TestRuleAdd()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Add Fail.";
                }
            }

            private class TestRuleUpdate : SystemItemRuleStub, IUpdateValidationRule
            {
                public TestRuleUpdate()
                {
                    StubIsValid = false;
                    StubFailureMessage = "Update Fail.";
                }
            }

            private class TestRuleAddValid : SystemItemRuleStub, IAddValidationRule
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
                IValidationRule<SystemItem>[] rules =
                {
                    new TestRuleAdd(),
                    new TestRuleUpdate(),
                    new TestRuleAddValid()
                };

                var provider = new SystemItemRuleProvider(rules);
                var validator = new SystemItemValidator(provider);

                _systemItem = New.SystemItem.AsDomain.WithMappedSystem(New.MappedSystem);

                _result = validator.ValidateForUpdate(_systemItem);
                _isValid = validator.IsValid(_systemItem);
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