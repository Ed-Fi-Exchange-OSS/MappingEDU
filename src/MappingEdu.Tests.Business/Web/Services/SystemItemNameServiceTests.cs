// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemName;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class SystemItemNameServiceTests
    {
        [TestFixture]
        public class When_getting_system_item_name : TestBase
        {
            private SystemItemNameViewModel _results;
            private readonly SystemItemNameViewModel _expected = new SystemItemNameViewModel();

            [OneTimeSetUp]
            public void Setup()
            {
                var id = Guid.NewGuid();
                SystemItem entity = New.SystemItem;

                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(id)).Return(entity);

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<SystemItemNameViewModel>(entity)).Return(_expected);

                var entityNameService = new SystemItemNameService(systemItemRepository, mapper);
                _results = entityNameService.Get(id);
            }

            [Test]
            public void Should_populate_entity_name()
            {
                _results.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_a_system_item_that_does_not_exist : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_id)).Return(null);

                var mapper = GenerateStub<IMapper>();

                var entityNameService = new SystemItemNameService(systemItemRepository, mapper);

                try
                {
                    entityNameService.Get(_id);
                }
                catch (Exception exception)
                {
                    _hasException = true;
                    _exception = exception;
                }
            }

            [Test]
            public void Should_have_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }

            [Test]
            public void Should_have_meaningful_error_message()
            {
                _exception.Message.ShouldEqual("The system item with id '" + _id + "' does not exist.");
            }
        }
    }
}