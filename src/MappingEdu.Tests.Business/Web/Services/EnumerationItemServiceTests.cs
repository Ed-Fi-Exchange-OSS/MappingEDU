// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.EnumerationItem;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class EnumerationItemServiceTests
    {
        [TestFixture]
        public class When_getting_a_system_enumeration_item : TestBase
        {
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemEnumerationItemId = Guid.NewGuid();
            private EnumerationItemViewModel _expected;
            private EnumerationItemViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItem = New.SystemItem.AsEnumeration.WithId(_systemItemId);

                var enumerationRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                SystemEnumerationItem enumerationItem = New.SystemEnumerationItem.WithSystemItem(systemItem);
                enumerationRepository.Stub(x => x.Get(_systemEnumerationItemId)).Return(enumerationItem);

                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();

                _expected = new EnumerationItemViewModel();

                var mapper = GenerateStub<IMapper>();
                mapper.Expect(x => x.Map<EnumerationItemViewModel>(enumerationItem)).Return(_expected);

                IEnumerationItemService enumerationItemService = new EnumerationItemService(enumerationRepository, systemItemRepository, mapper);
                _result = enumerationItemService.Get(_systemItemId, _systemEnumerationItemId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_a_system_enumeration_item_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemEnumerationItemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                enumerationRepository.Stub(x => x.Get(_systemEnumerationItemId)).Return(null);

                var systemItemRepository = MockRepository.GenerateStrictMock<IRepository<SystemItem>>();

                IEnumerationItemService enumerationItemService = new EnumerationItemService(enumerationRepository, systemItemRepository, null);
                try
                {
                    enumerationItemService.Get(_systemItemId, _systemEnumerationItemId);
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
                _exception.Message.ShouldEqual(string.Format("System Enumeration Item with id '{0}' does not exist.", _systemEnumerationItemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }

        [TestFixture]
        public class When_getting_a_system_enumeration_item_for_a_system_item_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemEnumerationItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                enumerationRepository.Stub(x => x.Get(_systemEnumerationItemId)).Return(new SystemEnumerationItem());

                var systemItemRepository = MockRepository.GenerateStrictMock<IRepository<SystemItem>>();

                IEnumerationItemService enumerationItemService = new EnumerationItemService(enumerationRepository, systemItemRepository, null);
                try
                {
                    enumerationItemService.Get(_systemItemId, _systemEnumerationItemId);
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
                _exception.Message.ShouldEqual(
                    string.Format("System Enumeration Item with id '{0}' does not have System Item id of '{1}'.", _systemEnumerationItemId, _systemItemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }

        [TestFixture]
        public class When_getting_all_system_enumeration_items_for_a_system_item : TestBase
        {
            private readonly Guid _systemItemId = Guid.NewGuid();
            private EnumerationItemViewModel[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem systemItem = New.SystemItem.WithId(_systemItemId);
                New.SystemEnumerationItem.WithSystemItem(systemItem);
                New.SystemEnumerationItem.WithSystemItem(systemItem);
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_systemItemId)).Return(systemItem);

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<EnumerationItemViewModel[]>(systemItem.SystemEnumerationItems))
                    .Return(new[] {new EnumerationItemViewModel(), new EnumerationItemViewModel()});

                IEnumerationItemService enumerationItemService = new EnumerationItemService(null, systemItemRepository, mapper);
                _result = enumerationItemService.Get(_systemItemId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(2);
            }
        }

        [TestFixture]
        public class When_getting_all_system_enumeration_items_for_a_system_item_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_systemItemId)).Return(null);

                IEnumerationItemService enumerationItemService = new EnumerationItemService(null, systemItemRepository, null);
                try
                {
                    enumerationItemService.Get(_systemItemId);
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
                _exception.Message.ShouldEqual(string.Format("System Item with id '{0}' does not exist.", _systemItemId));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_creating_a_system_enumeration_item_for_a_system_item_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_systemItemId)).Return(null);

                IEnumerationItemService enumerationItemService = new EnumerationItemService(null, systemItemRepository, null);

                try
                {
                    enumerationItemService.Post(_systemItemId, new EnumerationItemCreateModel());
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
                _exception.Message.ShouldEqual(string.Format("System Item with id '{0}' does not exist.", _systemItemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }

        [TestFixture]
        public class When_updating_a_system_enumeration_item_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _systemEnumerationItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                enumerationRepository.Stub(x => x.Get(_systemEnumerationItemId)).Return(null);

                IEnumerationItemService enumerationItemService = new EnumerationItemService(enumerationRepository, null, null);
                try
                {
                    enumerationItemService.Put(Guid.NewGuid(), _systemEnumerationItemId, new EnumerationItemEditModel());
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
                _exception.Message.ShouldEqual(string.Format("System Enumeration Item with id '{0}' does not exist.", _systemEnumerationItemId));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }

        [TestFixture]
        public class When_deleting_a_system_enumeration_item_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _systemEnumerationItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                enumerationRepository.Stub(x => x.Get(_systemEnumerationItemId)).Return(null);

                IEnumerationItemService enumerationItemService = new EnumerationItemService(enumerationRepository, null, null);
                try
                {
                    enumerationItemService.Delete(Guid.NewGuid(), _systemEnumerationItemId);
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
                _exception.Message.ShouldEqual(string.Format("System Enumeration Item with id '{0}' does not exist.", _systemEnumerationItemId));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }

        [TestFixture]
        public class When_deleting_enumeration_item_that_is_mapped_to : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemEnumerationItem enumeration = New.SystemEnumerationItem.WithId(Guid.NewGuid()).WithSystemItem(New.SystemItem.WithId(Guid.NewGuid()));
                New.EnumerationItemMap.WithTargetSystemEnumerationItem(enumeration);

                var systemEnumerationItemRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                systemEnumerationItemRepository.Stub(x => x.Get(enumeration.SystemEnumerationItemId)).Return(enumeration);

                var enumerationService = new EnumerationItemService(systemEnumerationItemRepository, null, null);
                try
                {
                    enumerationService.Delete(enumeration.SystemItem.SystemItemId, enumeration.SystemEnumerationItemId);
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
                _exception.Message.ShouldEqual("Cannot delete this enumeration item because it is mapped to.");
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