// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.SystemItemTree;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class SystemItemTreeServiceTests
    {
        [TestFixture]
        public class When_getting_system_item_that_does_not_exist : TestBase
        {
            private readonly Guid _domainId = Guid.NewGuid();

            private bool _hasException;
            private Exception _exception;
            private Guid _invalidEntityId;

            [OneTimeSetUp]
            public void Setup()
            {
                _invalidEntityId = Guid.NewGuid();

                var systemItemTreeRepository = GenerateStub<ISystemItemRepository>();
                systemItemTreeRepository.Stub(x => x.Get(_invalidEntityId)).Return(null);

                var treeService = new SystemItemTreeService(systemItemTreeRepository, null);

                try
                {
                    treeService.Get(_domainId, _invalidEntityId);
                }
                catch (Exception exception)
                {
                    _hasException = true;
                    _exception = exception;
                }
            }

            [Test]
            public void Should_have_meaningful_error_message()
            {
                _exception.Message.ShouldEqual("The system item id '" + _invalidEntityId + "' was not found.");
            }

            [Test]
            public void Should_throw_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }
    }
}