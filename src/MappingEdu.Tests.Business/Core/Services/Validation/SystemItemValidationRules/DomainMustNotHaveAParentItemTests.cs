// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation.SystemItemValidationRules;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemItemValidationRules
{
    public class DomainMustNotHaveAParentItemTests
    {
        [TestFixture]
        public class When_domain_item_has_no_parent
        {
            private SystemItemBuilder _systemItem;
            private DomainMustNotHaveAParentItem _rule;

            [OneTimeSetUp]
            public void Setup()
            {
                _systemItem = New.SystemItem.AsDomain;
                _rule = new DomainMustNotHaveAParentItem();
            }

            [Test]
            public void Should_be_valid()
            {
                _rule.IsValid(_systemItem).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_item_is_not_domain
        {
            private SystemItemBuilder _systemItem;
            private DomainMustNotHaveAParentItem _rule;

            [OneTimeSetUp]
            public void Setup()
            {
                _systemItem = New.SystemItem.AsEntity.WithParentSystemItem(
                    New.SystemItem.WithId(Guid.NewGuid()));
                _rule = new DomainMustNotHaveAParentItem();
            }

            [Test]
            public void Should_be_valid()
            {
                _rule.IsValid(_systemItem).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_domain_item_has_parent
        {
            private SystemItem _systemItem;
            private DomainMustNotHaveAParentItem _rule;

            [OneTimeSetUp]
            public void Setup()
            {
                _systemItem = New.SystemItem.AsDomain.WithParentSystemItem(
                    New.SystemItem.WithId(Guid.NewGuid()));

                _rule = new DomainMustNotHaveAParentItem();
            }

            [Test]
            public void Should_have_a_meaningful_failure_message()
            {
                _rule.GetFailureMessage(_systemItem).ShouldEqual("Domain System Items cannot have a Parent.");
            }

            [Test]
            public void Should_not_be_valid()
            {
                _rule.IsValid(_systemItem).ShouldBeFalse();
            }

            [Test]
            public void Should_provide_correct_property_name()
            {
                _rule.GetPropertyName(_systemItem).ShouldEqual("ParentSystemItemId");
            }
        }
    }
}