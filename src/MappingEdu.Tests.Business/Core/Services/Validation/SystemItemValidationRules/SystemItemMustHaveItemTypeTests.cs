// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation.SystemItemValidationRules;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemMustHaveItemTypeTests
    {
        [TestFixture]
        public class When_system_item_has_an_item_type
        {
            private SystemItem _systemItem;
            private SystemItemMustHaveItemType _rule;

            [OneTimeSetUp]
            public void Setup()
            {
                _systemItem = New.SystemItem.AsElement;
                _rule = new SystemItemMustHaveItemType();
            }

            [Test]
            public void Should_be_valid()
            {
                _rule.IsValid(_systemItem).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_system_item_type_is_not_present
        {
            private SystemItem _systemItem;
            private SystemItemMustHaveItemType _rule;

            [OneTimeSetUp]
            public void Setup()
            {
                _systemItem = New.SystemItem;
                _rule = new SystemItemMustHaveItemType();
            }

            [Test]
            public void Should_have_a_meaningful_failure_message()
            {
                _rule.GetFailureMessage(_systemItem).ShouldEqual("System Item, Item Type is not set.");
            }

            [Test]
            public void Should_not_be_valid()
            {
                _rule.IsValid(_systemItem).ShouldBeFalse();
            }

            [Test]
            public void Should_provide_correct_property_name()
            {
                _rule.GetPropertyName(_systemItem).ShouldEqual("ItemType");
            }
        }
    }
}