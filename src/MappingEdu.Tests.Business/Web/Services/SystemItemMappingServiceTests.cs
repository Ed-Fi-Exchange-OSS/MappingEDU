// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.Mapping;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class SystemItemMappingServiceTests
    {
        [TestFixture]
        public class When_getting_a_system_item_map : TestBase
        {
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private SystemItemMappingViewModel _result;
            private SystemItemMappingViewModel _expected;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItem = New.SystemItem
                    .WithId(_sourceSystemItemId)
                    .WithMappedSystem(New.MappedSystem.WithId(_mappedSystemId));
                SystemItemMap systemItemMap = New.SystemItemMap.WithSourceSystemItem(systemItem);
                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMappingRepository.Stub(x => x.Get(_systemItemMapId)).Return(systemItemMap);

                _expected = new SystemItemMappingViewModel();

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<SystemItemMappingViewModel>(systemItemMap)).Return(_expected);

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(systemItemMappingRepository, null, null, null, mapper, null, null);
                _result = systemItemMappingService.Get(_sourceSystemItemId, _systemItemMapId);
            }

            [Test]
            public void Should_return_result_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_all_system_item_maps_for_a_system_item : TestBase
        {
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private readonly Guid _map1Id = Guid.NewGuid();
            private readonly Guid _map2Id = Guid.NewGuid();
            private readonly Guid _map3Id = Guid.NewGuid();
            private SystemItemMappingViewModel[] _result;
            private SystemItemMappingViewModel[] _expected;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem systemItem = New.SystemItem.WithId(_sourceSystemItemId);
                SystemItemMap map1 = New.SystemItemMap.WithSourceSystemItem(systemItem).WithId(_map1Id);
                SystemItemMap map2 = New.SystemItemMap.WithSourceSystemItem(systemItem).WithId(_map2Id);
                SystemItemMap map3 = New.SystemItemMap.WithSourceSystemItem(systemItem).WithId(_map3Id);

                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_sourceSystemItemId)).Return(systemItem);

                _expected = new[]
                {
                    new SystemItemMappingViewModel {SystemItemMapId = map1.SystemItemMapId},
                    new SystemItemMappingViewModel {SystemItemMapId = map2.SystemItemMapId},
                    new SystemItemMappingViewModel {SystemItemMapId = map3.SystemItemMapId}
                };

                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMappingRepository.Stub(x => x.GetAllMaps())
                    .Return(systemItem.SourceSystemItemMaps.AsQueryable());

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<SystemItemMappingViewModel[]>(systemItem.SourceSystemItemMaps.ToArray())).Return(_expected);

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(systemItemMappingRepository, systemItemRepository, null, null, mapper, null, null);
                _result = systemItemMappingService.GetSourceMappings(_sourceSystemItemId);
            }

            [Test]
            public void Should_return_result_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_all_system_item_maps_for_a_system_item_and_mapping_project : TestBase
        {
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private readonly Guid _mappingProjectId = Guid.NewGuid();
            private readonly Guid _map1Id = Guid.NewGuid();
            private readonly Guid _map2Id = Guid.NewGuid();
            private readonly Guid _map3Id = Guid.NewGuid();
            private SystemItemMappingViewModel[] _result;
            private SystemItemMappingViewModel[] _expected;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem systemItem = New.SystemItem.WithId(_sourceSystemItemId);
                MappingProject mappingProject = New.MappingProject.WithId(_mappingProjectId);
                SystemItemMap map1 = New.SystemItemMap
                    .WithSourceSystemItem(systemItem)
                    .WithId(_map1Id)
                    .WithMappingProject(mappingProject);
                map1.UserNotifications = new List<UserNotification>();
                SystemItemMap map2 = New.SystemItemMap
                    .WithSourceSystemItem(systemItem)
                    .WithId(_map2Id)
                    .WithMappingProject(mappingProject);
                map2.UserNotifications = new List<UserNotification>();
                SystemItemMap map3 = New.SystemItemMap
                    .WithSourceSystemItem(systemItem)
                    .WithId(_map3Id)
                    .WithMappingProject(New.MappingProject.WithId(Guid.NewGuid()));
                map3.UserNotifications = new List<UserNotification>();

                var systemItemMapRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMapRepository.Stub(x => x.GetAllMaps()).Return(systemItem.SourceSystemItemMaps.AsQueryable());

                _expected = new[]
                {
                    new SystemItemMappingViewModel {SystemItemMapId = map1.SystemItemMapId},
                    new SystemItemMappingViewModel {SystemItemMapId = map2.SystemItemMapId}
                };

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<SystemItemMappingViewModel[]>(
                    systemItem.SourceSystemItemMaps
                        .Where(sim => sim.SourceSystemItemId == _sourceSystemItemId &&
                                      sim.MappingProjectId == _mappingProjectId).ToArray()))
                    .Return(_expected);

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(systemItemMapRepository, null, null, null, mapper, null, null);
                _result = systemItemMappingService.GetItemMappingsByProject(_sourceSystemItemId, _mappingProjectId);
            }

            [Test]
            public void Should_return_result_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_a_system_item_map_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMappingRepository.Stub(x => x.Get(_systemItemMapId)).Return(null);

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(systemItemMappingRepository, null, null, null, null, null, null);

                try
                {
                    systemItemMappingService.Get(_sourceSystemItemId, _systemItemMapId);
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
                    string.Format("System Item Map with id '{0}' does not exist.", _systemItemMapId));
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
        public class When_getting_a_system_item_map_for_a_system_item_that_does_not_exist : TestBase
        {
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMappingRepository.Stub(x => x.Get(_systemItemMapId)).Return(New.SystemItemMap);
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(
                        systemItemMappingRepository, systemItemRepository, null, null, null, null, null);

                try
                {
                    systemItemMappingService.Get(_sourceSystemItemId, _systemItemMapId);
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
                        "System Item Map with id '{0}' does not have System Item id of '{1}'.", _systemItemMapId, _sourceSystemItemId));
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
        public class When_creating_a_system_item_map_for_a_system_item_that_does_not_exist : TestBase
        {
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private bool _exceptionThrown;
            private Exception _exception;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Expect(x => x.Get(_sourceSystemItemId)).Return(null);

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(
                        systemItemMappingRepository, systemItemRepository, null, null, null, null, null);
                var model = new SystemItemMappingCreateModel();

                try
                {
                    systemItemMappingService.Post(_sourceSystemItemId, model);
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
                    string.Format("System Item with id '{0}' does not exist.", _sourceSystemItemId));
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
        public class When_updating_a_system_item_map_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _systemItemMapId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMappingRepository.Stub(x => x.Get(_systemItemMapId)).Return(null);

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(systemItemMappingRepository, null, null, null, null, null, null);

                try
                {
                    systemItemMappingService.Put(Guid.Empty, _systemItemMapId, new SystemItemMappingEditModel());
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
                    string.Format("System Item Map with id '{0}' does not exist.", _systemItemMapId));
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
        public class When_deleting_a_system_item_map_that_does_not_exist : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemItemMapId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMappingRepository.Stub(x => x.Get(_systemItemMapId)).Return(null);

                ISystemItemMappingService systemItemMappingService =
                    new SystemItemMappingService(systemItemMappingRepository, null, null, null, null, null, null);

                try
                {
                    systemItemMappingService.Delete(_systemItemId, _systemItemMapId);
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
                    string.Format("System Item Map with id '{0}' does not exist.", _systemItemMapId));
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
        public class When_updating_a_system_item_map_to_remove_an_enumeration_mapping : TestBase
        {
            private SystemItemMap _mapping;
            private SystemEnumerationItemMap _enumerationItemMapToKeep1;
            private SystemEnumerationItemMap _enumerationItemMapToKeep2;
            private SystemEnumerationItemMap _enumerationItemMapToRemove;
            private IRepository<SystemEnumerationItemMap> _enumerationMapRepository;

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem sourceEnumeration = New.SystemItem.AsEnumeration.WithId(Guid.NewGuid());
                var sourceEnumerationItem = New.SystemEnumerationItem.WithSystemItem(sourceEnumeration);

                MappedSystem targetMappedSystem = New.MappedSystem;
                MappingProject mappingProject = New.MappingProject.WithTarget(targetMappedSystem);
                SystemItem targetEnumeration = New.SystemItem.AsEnumeration.WithMappedSystem(targetMappedSystem);
                SystemEnumerationItem targetEnumerationItem = New.SystemEnumerationItem.WithSystemItem(targetEnumeration);

                _mapping = New.SystemItemMap
                    .WithSourceSystemItem(sourceEnumeration)
                    .WithMappingProject(mappingProject).WithId(Guid.NewGuid());
                _enumerationItemMapToKeep1 = New.EnumerationItemMap
                    .WithSystemItemMap(_mapping)
                    .WithSourceSystemEnumerationItem(sourceEnumerationItem)
                    .WithTargetSystemEnumerationItem(targetEnumerationItem);
                _enumerationItemMapToRemove = New.EnumerationItemMap
                    .WithSystemItemMap(_mapping)
                    .WithSourceSystemEnumerationItem(sourceEnumerationItem)
                    .WithTargetSystemEnumerationItem(New.SystemEnumerationItem
                        .WithSystemItem(New.SystemItem));
                _enumerationItemMapToKeep2 = New.EnumerationItemMap
                    .WithSystemItemMap(_mapping)
                    .WithSourceSystemEnumerationItem(sourceEnumerationItem);

                var businessLogicParser = GenerateStub<IBusinessLogicParser>();
                businessLogicParser.Stub(
                    x => x.ParseReferencedSystemItems("my business logic", true, targetMappedSystem))
                    .Return(new[] {targetEnumeration});

                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();
                mappedSystemRepository.Stub(x => x.Get(targetMappedSystem.MappedSystemId))
                    .Return(targetMappedSystem);

                var systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMappingRepository.Stub(x => x.Get(_mapping.SystemItemMapId))
                    .Return(_mapping);

                _enumerationMapRepository = GenerateStub<IRepository<SystemEnumerationItemMap>>();
                var mapper = GenerateStub<IMapper>();

                var mappingProjectRepository = GenerateStub<IRepository<MappingProject>>();
                mappingProjectRepository.Stub(x => x.Get(mappingProject.MappingProjectId)).Return(mappingProject);

                ISystemItemMappingService service = new SystemItemMappingService(
                    systemItemMappingRepository, null, mappingProjectRepository, _enumerationMapRepository,
                    mapper, businessLogicParser, null);

                service.Put(
                    sourceEnumeration.SystemItemId, _mapping.SystemItemMapId, new SystemItemMappingEditModel
                    {
                        BusinessLogic = "my business logic",
                        MappingProjectId = mappingProject.MappingProjectId
                    });
            }

            [Test]
            public void Should_delete_enumeration_item_map()
            {
                _enumerationMapRepository.AssertWasCalled(x => x.Delete(_enumerationItemMapToRemove));
            }

            [Test]
            public void Should_keep_valid_enumeration_mappings()
            {
                _mapping.SystemEnumerationItemMaps.ShouldContain(_enumerationItemMapToKeep1);
                _mapping.SystemEnumerationItemMaps.ShouldContain(_enumerationItemMapToKeep2);
            }
        }
    }
}
