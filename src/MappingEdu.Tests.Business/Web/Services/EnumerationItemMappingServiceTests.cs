// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.EnumerationItemMapping;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class EnumerationItemMappingServiceTests
    {
        [TestFixture]
        public class When_getting_an_enumeration_item_map : TestBase
        {
            private EnumerationItemMappingViewModel _expected;
            private EnumerationItemMappingViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItemMap systemItemMap = New.SystemItemMap.WithId(Guid.NewGuid());
                SystemEnumerationItem sourceEnumerationItem = New.SystemEnumerationItem.WithId(Guid.NewGuid());
                SystemEnumerationItemMap enumerationItemMap = New.EnumerationItemMap.WithId(Guid.NewGuid())
                    .WithSourceSystemEnumerationItem(sourceEnumerationItem)
                    .WithSystemItemMap(systemItemMap);

                var enumerationItemMappingRepository = GenerateStub<IRepository<SystemEnumerationItemMap>>();
                enumerationItemMappingRepository.Stub(x => x.Get(enumerationItemMap.SystemEnumerationItemMapId)).Return(enumerationItemMap);

                var mapper = GenerateStub<IMapper>();
                _expected = new EnumerationItemMappingViewModel();
                mapper.Stub(x => x.Map<EnumerationItemMappingViewModel>(enumerationItemMap)).Return(_expected);

                IEnumerationItemMappingService enumerationItemMappingService =
                    new EnumerationItemMappingService(enumerationItemMappingRepository, null, null, mapper);
                _result = enumerationItemMappingService.Get(systemItemMap.SystemItemMapId, enumerationItemMap.SystemEnumerationItemMapId);
            }

            [Test]
            public void Should_return_result_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_all_enumeration_item_maps_for_system_item_map : TestBase
        {
            private EnumerationItemMappingViewModel[] _expected;
            private EnumerationItemMappingViewModel[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItemMap systemItemMap = New.SystemItemMap.WithId(Guid.NewGuid());
                New.EnumerationItemMap.WithId(Guid.NewGuid())
                    .WithSourceSystemEnumerationItem(New.SystemEnumerationItem.WithId(Guid.NewGuid()))
                    .WithSystemItemMap(systemItemMap);
                New.EnumerationItemMap.WithId(Guid.NewGuid())
                    .WithSourceSystemEnumerationItem(New.SystemEnumerationItem.WithId(Guid.NewGuid()))
                    .WithSystemItemMap(systemItemMap);
                New.EnumerationItemMap.WithId(Guid.NewGuid())
                    .WithSourceSystemEnumerationItem(New.SystemEnumerationItem.WithId(Guid.NewGuid()))
                    .WithSystemItemMap(systemItemMap);

                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                systemItemMapRepository.Stub(x => x.Get(systemItemMap.SystemItemMapId)).Return(systemItemMap);

                var mapper = GenerateStub<IMapper>();
                _expected = new[] {new EnumerationItemMappingViewModel(), new EnumerationItemMappingViewModel(), new EnumerationItemMappingViewModel()};
                mapper.Stub(x => x.Map<EnumerationItemMappingViewModel[]>(systemItemMap.SystemEnumerationItemMaps)).Return(_expected);

                IEnumerationItemMappingService enumerationItemMappingService = new EnumerationItemMappingService(null, null, systemItemMapRepository, mapper);
                _result = enumerationItemMappingService.Get(systemItemMap.SystemItemMapId);
            }

            [Test]
            public void Should_return_result_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_an_enumeration_item_map_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private readonly Guid _enumerationItemMapId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationItemMappingRepository = GenerateStub<IRepository<SystemEnumerationItemMap>>();
                enumerationItemMappingRepository.Stub(x => x.Get(_enumerationItemMapId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                IEnumerationItemMappingService enumerationItemMappingService =
                    new EnumerationItemMappingService(enumerationItemMappingRepository, null, null, mapper);

                try
                {
                    enumerationItemMappingService.Get(_systemItemMapId, _enumerationItemMapId);
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
                _exception.Message.ShouldEqual(string.Format("Enumeration Item Map with id '{0}' does not exist.", _enumerationItemMapId));
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
        public class When_getting_an_enumeration_item_map_for_a_system_item_map_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private readonly Guid _enumerationItemMapId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationItemMappingRepository = GenerateStub<IRepository<SystemEnumerationItemMap>>();
                enumerationItemMappingRepository.Stub(x => x.Get(_enumerationItemMapId)).Return(New.EnumerationItemMap);
                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                var mapper = GenerateStub<IMapper>();

                IEnumerationItemMappingService systemItemMappingService = new EnumerationItemMappingService(enumerationItemMappingRepository, null, systemItemMapRepository, mapper);

                try
                {
                    systemItemMappingService.Get(_systemItemMapId, _enumerationItemMapId);
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
                    string.Format(
                        "Enumeration Item Map with id '{0}' does not have System Item Map id of '{1}'.", _enumerationItemMapId, _systemItemMapId));
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
        public class When_creating_an_enumeration_item_map_for_a_system_item_map_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                systemItemMapRepository.Expect(x => x.Get(_systemItemMapId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                IEnumerationItemMappingService systemItemMappingService =
                    new EnumerationItemMappingService(null, null, systemItemMapRepository, mapper);

                try
                {
                    systemItemMappingService.Post(_systemItemMapId, new EnumerationItemMappingCreateModel());
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
                _exception.Message.ShouldEqual(string.Format("System item map with id '{0}' does not exist.", _systemItemMapId));
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
        public class When_updating_an_enumeration_item_map_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _sourceSystemEnumerationItemId = Guid.NewGuid();
            private readonly Guid _enumerationMapId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationItemMappingRepository = GenerateStub<IRepository<SystemEnumerationItemMap>>();
                enumerationItemMappingRepository.Stub(x => x.Get(_enumerationMapId)).Return(null);

                var systemEnumerationItemRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                var mapper = GenerateStub<IMapper>();

                IEnumerationItemMappingService systemItemMappingService =
                    new EnumerationItemMappingService(enumerationItemMappingRepository, systemEnumerationItemRepository, systemItemMapRepository, mapper);

                try
                {
                    systemItemMappingService.Put(_sourceSystemEnumerationItemId, _enumerationMapId, new EnumerationItemMappingEditModel());
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
                _exception.Message.ShouldEqual(string.Format("Enumeration Item Map with id '{0}' does not exist.", _enumerationMapId));
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
        public class When_deleting_an_enumeration_item_map_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _sourceSystemEnumerationItemId = Guid.NewGuid();
            private readonly Guid _systemItemMapId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var enumerationItemMappingRepository = GenerateStub<IRepository<SystemEnumerationItemMap>>();
                enumerationItemMappingRepository.Stub(x => x.Get(_systemItemMapId)).Return(null);

                var systemEnumerationItemRepository = GenerateStub<IRepository<SystemEnumerationItem>>();
                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                var mapper = GenerateStub<IMapper>();

                IEnumerationItemMappingService systemItemMappingService =
                    new EnumerationItemMappingService(enumerationItemMappingRepository, systemEnumerationItemRepository, systemItemMapRepository, mapper);

                try
                {
                    systemItemMappingService.Delete(_sourceSystemEnumerationItemId, _systemItemMapId);
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
                _exception.Message.ShouldEqual(string.Format("Enumeration Item Map with id '{0}' does not exist.", _systemItemMapId));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }
    }
}