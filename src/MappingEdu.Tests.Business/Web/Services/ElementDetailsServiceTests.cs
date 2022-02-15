// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class ElementDetailsServiceTests
    {
        [TestFixture]
        public class When_getting_specific_element : TestBase
        {
            private readonly Guid _id = Guid.NewGuid();
            private readonly ElementDetailsViewModel _expected = new ElementDetailsViewModel();
            private ElementDetailsViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem systemItem = New.SystemItem.AsElement.WithId(_id);
                var elementRepository = GenerateStub<IRepository<SystemItem>>();
                var mapper = GenerateStub<IMapper>();

                elementRepository.Stub(x => x.Get(_id)).Return(systemItem);
                mapper.Stub(x => x.Map<ElementDetailsViewModel>(systemItem)).Return(_expected);

                IElementDetailsService elementDetailsService = new ElementDetailsService(elementRepository, mapper);
                _result = elementDetailsService.Get(_id);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_specific_element_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var elementRepository = GenerateStub<IRepository<SystemItem>>();
                elementRepository.Stub(x => x.Get(_id)).Return(null);

                IElementDetailsService elementDetailsService = new ElementDetailsService(elementRepository, null);
                try
                {
                    elementDetailsService.Get(_id);
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("System Item with id '{0}' does not exist.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_creating_element_for_parent_system_item_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _parentSystemItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();

                systemItemRepository.Stub(x => x.Get(_parentSystemItemId)).Return(null);

                IElementDetailsService elementDetailsService = new ElementDetailsService(systemItemRepository, null);

                var createModel = new ElementDetailsCreateModel
                {
                    ParentSystemItemId = _parentSystemItemId
                };

                try
                {
                    elementDetailsService.Post(createModel);
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
                _exception.Message.ShouldEqual(string.Format("System Item with id '{0}' does not exist.", _parentSystemItemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_updating_an_element_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var elementRepository = GenerateStub<IRepository<SystemItem>>();
                elementRepository.Stub(x => x.Get(_id)).Return(null);

                IElementDetailsService elementDetailsService = new ElementDetailsService(elementRepository, null);
                try
                {
                    elementDetailsService.Put(_id, new ElementDetailsEditModel());
                }
                catch (Exception e)
                {
                    _exceptionThrown = true;
                    _exception = e;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("System Item with id '{0}' does not exist.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }
    }
}