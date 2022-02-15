// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class EnumerationViewServiceTests
    {
        [TestFixture]
        public class When_deleting_enumeration_that_is_mapped_to : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem enumeration = New.SystemItem.AsEnumeration.WithId(Guid.NewGuid());
                New.SystemItemMap.WithTargetSystemItem(enumeration);

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Stub(x => x.Get(enumeration.SystemItemId)).Return(enumeration);

                var enumerationService = new EnumerationService(null, null, null, null, null, null, null, null, systemItemRepository, null, null);
                try
                {
                    enumerationService.Delete(enumeration.SystemItemId);
                }
                catch (Exception ex)
                {
                    _exceptionThrown = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual("Cannot delete this enumeration because it is mapped to.");
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }
    }
}