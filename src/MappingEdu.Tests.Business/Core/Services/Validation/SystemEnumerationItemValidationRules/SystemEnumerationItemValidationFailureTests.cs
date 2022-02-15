// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Services.Validation.SystemEnumerationItemValidationRules;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Validation.SystemEnumerationItemValidationRules
{
    [TestFixture]
    public class SystemEnumerationItemValidationFailureTests
    {
        private SystemEnumerationItemValidationFailure _failure;
        private readonly Guid _systemEnumerationItemId = Guid.NewGuid();
        private readonly Guid _systemItemId = Guid.NewGuid();
        private const string systemItemName = "System Item Name";
        private const string codeValue = "CODVAL";
        private const string description = "System Enumeration Item Description";
        private const string shortDescription = "Short Description";
        private const string failMessage = "Fail Message";

        [OneTimeSetUp]
        public void Setup()
        {
            _failure = new SystemEnumerationItemValidationFailure
            {
                Id = _systemEnumerationItemId,
                SystemItemId = _systemItemId,
                SystemItemName = systemItemName,
                CodeValue = codeValue,
                Description = description,
                ShortDescription = shortDescription,
                ValidationError = failMessage
            };
        }

        [Test]
        public void Should_build_full_failure_message()
        {
            _failure.FullMessage.ShouldEqual(
                string.Format("System Enumeration Item [System Item Id: '{0}', System Enumeration Item Id: '{1}']: Fail Message", _systemItemId, _systemEnumerationItemId));
        }

        [Test]
        public void Should_build_short_failure_message()
        {
            _failure.ShortMessage.ShouldEqual("'CODVAL' System Enumeration Item Description for 'System Item Name': Fail Message");
        }
    }
}