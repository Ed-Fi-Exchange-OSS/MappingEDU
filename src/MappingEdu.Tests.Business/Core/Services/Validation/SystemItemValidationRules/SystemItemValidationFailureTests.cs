// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Services.Validation.SystemItemValidationRules;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemItemValidationRules
{
    [TestFixture]
    public class SystemItemValidationFailureTests
    {
        private readonly Guid _mappedSystemId = Guid.NewGuid();
        private readonly Guid _systemItemId = Guid.NewGuid();

        private const string SystemItemName = "Item Name";
        private const string MappedSystemName = "System Name";
        private const string Fail = "Fail Message";

        private SystemItemValidationFailure _failure;

        [OneTimeSetUp]
        public void Setup()
        {
            _failure = new SystemItemValidationFailure
            {
                Id = _systemItemId,
                MappedSystemId = _mappedSystemId,
                SystemItemName = SystemItemName,
                MappedSystemName = MappedSystemName,
                SystemItemType = ItemType.Entity,
                ValidationError = Fail
            };
        }

        [Test]
        public void Should_build_full_failure_message()
        {
            _failure.FullMessage.ShouldEqual(
                string.Format("System Item [Mapped System Id: '{0}', System Item Id: '{1}']: Fail Message", _mappedSystemId, _systemItemId));
        }

        [Test]
        public void Should_build_short_failure_message()
        {
            _failure.ShortMessage.ShouldEqual("'Item Name' Entity for 'System Name': Fail Message");
        }
    }
}