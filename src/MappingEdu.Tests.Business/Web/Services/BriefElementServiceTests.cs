// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.BriefElement;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class BriefElementServiceTests
    {
        [TestFixture]
        public class When_getting_elements_for_specific_entity : TestBase
        {
            private readonly Guid _entitySystemItemId = Guid.NewGuid();
            private BriefElementViewModel[] _expected;
            private BriefElementViewModel[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem entity = New.SystemItem.AsEntity.WithId(_entitySystemItemId);
                New.SystemItem.AsElement.WithParentSystemItem(entity);
                New.SystemItem.AsElement.WithParentSystemItem(entity);

                var entityRepository = GenerateStub<IRepository<SystemItem>>();
                entityRepository.Stub(x => x.Get(_entitySystemItemId)).Return(entity);

                _expected = new[] {new BriefElementViewModel(), new BriefElementViewModel()};
                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<BriefElementViewModel[]>(entity.ChildSystemItems)).Return(_expected);

                var briefElementService = new BriefElementService(entityRepository, mapper);
                _result = briefElementService.Get(_entitySystemItemId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_specific_element : TestBase
        {
            private readonly Guid _entitySystemItemId = Guid.NewGuid();
            private readonly Guid _elementId = Guid.NewGuid();
            private BriefElementViewModel _expected;
            private BriefElementViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem entity = New.SystemItem.AsEntity.WithId(_entitySystemItemId);
                SystemItem element = New.SystemItem.AsElement.WithParentSystemItem(entity).WithId(_elementId);

                var entityRepository = GenerateStub<IRepository<SystemItem>>();
                entityRepository.Stub(x => x.Get(_elementId)).Return(element);

                _expected = new BriefElementViewModel();
                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<BriefElementViewModel>(element)).Return(_expected);

                var briefElementService = new BriefElementService(entityRepository, mapper);
                _result = briefElementService.Get(_entitySystemItemId, _elementId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_entity_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _id = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var repository = GenerateStub<IRepository<SystemItem>>();
                repository.Stub(x => x.Get(_id)).Return(null);

                var briefElementService = new BriefElementService(repository, null);
                try
                {
                    briefElementService.Get(_id);
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
                _exception.Message.ShouldEqual(string.Format("The system item with id '{0}' does not exist.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
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
                var repository = GenerateStub<IRepository<SystemItem>>();
                repository.Stub(x => x.Get(_id)).Return(null);

                var briefElementService = new BriefElementService(repository, null);
                try
                {
                    briefElementService.Get(Guid.NewGuid(), _id);
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
                _exception.Message.ShouldEqual(string.Format("The system item with id '{0}' does not exist.", _id));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_specific_element_that_does_not_match_parent_id : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _entityId = Guid.NewGuid();
            private readonly Guid _elementId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var entity = New.SystemItem.AsEntity.WithId(Guid.NewGuid());
                SystemItem element = New.SystemItem.AsElement.WithId(_elementId).WithParentSystemItem(entity);
                var repository = GenerateStub<IRepository<SystemItem>>();
                repository.Stub(x => x.Get(_elementId)).Return(element);

                var briefElementService = new BriefElementService(repository, null);
                try
                {
                    briefElementService.Get(_entityId, _elementId);
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
                _exception.Message.ShouldEqual(string.Format("The system item with id '{0}' does not have a parent system item with id '{1}'.", _elementId, _entityId));
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