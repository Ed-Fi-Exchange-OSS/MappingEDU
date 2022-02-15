// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Validation.SystemItemValidationRules;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemNameMustBeUniqueAcrossSiblingsTests
    {
        [TestFixture]
        public class When_system_item_name_is_unique_within_its_parent : TestBase
        {
            private bool _result;

            [OneTimeSetUp]
            public void Setup()
            {
                var parent = New.SystemItem;
                var systemItem = New.SystemItem.WithParentSystemItem(parent).WithName("Name 1");
                New.SystemItem.WithParentSystemItem(parent).WithName("Name 2");
                New.SystemItem.WithParentSystemItem(parent).WithName("Name 3");

                var rule = new SystemItemNameMustBeUniqueAcrossSiblings();
                _result = rule.IsValid(systemItem);
            }

            [Test]
            public void Should_be_valid()
            {
                _result.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_system_item_name_is_not_unique_within_its_parent : TestBase
        {
            private bool _result;
            private SystemItemNameMustBeUniqueAcrossSiblings _rule;
            private SystemItem _systemItem;

            [OneTimeSetUp]
            public void Setup()
            {
                var parent = New.SystemItem.WithName("Parent Item");
                _systemItem = New.SystemItem.WithParentSystemItem(parent).WithName("Name 1");
                New.SystemItem.WithParentSystemItem(parent).WithName("Name 2");
                New.SystemItem.WithParentSystemItem(parent).WithName("Name 1");
                New.SystemItem.WithParentSystemItem(parent).WithName("Name 3");

                _rule = new SystemItemNameMustBeUniqueAcrossSiblings();
                _result = _rule.IsValid(_systemItem);
            }

            [Test]
            public void Should_generate_meaningful_error_message()
            {
                _rule.GetFailureMessage(_systemItem).ShouldEqual("Item with name 'Name 1' already exists for parent item 'Parent Item'.");
            }

            [Test]
            public void Should_not_be_valid()
            {
                _result.ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_system_item_does_not_have_a_parent : TestBase
        {
            private bool _result;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItem = New.SystemItem.WithName("Name 1");

                var rule = new SystemItemNameMustBeUniqueAcrossSiblings();
                _result = rule.IsValid(systemItem);
            }

            [Test]
            public void Should_be_valid()
            {
                _result.ShouldBeTrue();
            }
        }
    }
}