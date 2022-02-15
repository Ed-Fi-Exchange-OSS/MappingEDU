// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class PreviousVersionDeltaServiceTests
    {
        [TestFixture]
        public class When_getting_all_previous_versions_for_system_item : TestBase
        {
            private PreviousVersionDeltaViewModel[] _nextVersionsDelta;
            private readonly Guid _childEntity1Id = Guid.NewGuid();
            private readonly Guid _childEntity2Id = Guid.NewGuid();
            private readonly Guid _childEntity3Id = Guid.NewGuid();
            private readonly Guid _version1Id = Guid.NewGuid();
            private readonly Guid _version2Id = Guid.NewGuid();
            private readonly Guid _version3Id = Guid.NewGuid();
            private readonly Guid _oldMappedSystemId = Guid.NewGuid();
            private readonly Guid _newMappedSystemId = Guid.NewGuid();
            private readonly PreviousVersionDeltaViewModel[] _expectedViewModels = {new PreviousVersionDeltaViewModel(), new PreviousVersionDeltaViewModel()};

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem oldMappedSystem = New.MappedSystem.WithId(_oldMappedSystemId);
                MappedSystem newMappedSystem = New.MappedSystem.WithId(_newMappedSystemId).WithPreviousVersion(oldMappedSystem);

                SystemItem childEntity1 = New.SystemItem.WithId(_childEntity1Id).WithMappedSystem(newMappedSystem);
                SystemItem childEntity2 = New.SystemItem.WithId(_childEntity2Id);
                SystemItem childEntity3 = New.SystemItem.WithId(_childEntity3Id);

                New.SystemItemVersionDelta.WithId(_version1Id)
                    .WithNewSystemItem(childEntity1);

                New.SystemItemVersionDelta.WithId(_version2Id)
                    .WithOldSystemItem(childEntity1)
                    .WithNewSystemItem(childEntity2);

                New.SystemItemVersionDelta.WithId(_version3Id)
                    .WithOldSystemItem(childEntity3)
                    .WithNewSystemItem(childEntity1);

                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_childEntity1Id)).Return(childEntity1);

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<PreviousVersionDeltaViewModel[]>(childEntity1.PreviousSystemItemVersionDeltas)).Return(_expectedViewModels);

                var versionService = new PreviousVersionDeltaService(null, systemItemRepository, mapper);
                _nextVersionsDelta = versionService.Get(_childEntity1Id);
            }

            [Test]
            public void Should_load_view_model()
            {
                _nextVersionsDelta.ShouldNotBeNull();
                _nextVersionsDelta.ShouldEqual(_expectedViewModels);
            }

            [Test]
            public void Should_set_mapped_system_id()
            {
                _expectedViewModels[0].OldMappedSystemId.ShouldEqual(_oldMappedSystemId);
                _expectedViewModels[1].OldMappedSystemId.ShouldEqual(_oldMappedSystemId);
            }
        }

        [TestFixture]
        public class When_getting_specific_version_delta : TestBase
        {
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemItemVersionDeltaId = Guid.NewGuid();
            private readonly Guid _oldMappedSystemId = Guid.NewGuid();
            private readonly Guid _newMappedSystemId = Guid.NewGuid();
            private readonly PreviousVersionDeltaViewModel _expectedViewModel = new PreviousVersionDeltaViewModel();
            private PreviousVersionDeltaViewModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem oldMappedSystem = New.MappedSystem.WithId(_oldMappedSystemId);
                MappedSystem newMappedSystem = New.MappedSystem.WithId(_newMappedSystemId).WithPreviousVersion(oldMappedSystem);
                SystemItem systemItem = New.SystemItem.WithId(_systemItemId).WithMappedSystem(newMappedSystem);

                SystemItemVersionDelta versionDelta = New.SystemItemVersionDelta.WithNewSystemItem(systemItem).WithId(_systemItemVersionDeltaId);

                var repo = GenerateStub<IRepository<SystemItemVersionDelta>>();
                repo.Stub(x => x.Get(_systemItemVersionDeltaId)).Return(versionDelta);

                var mapper = GenerateStub<IMapper>();
                mapper.Stub(x => x.Map<PreviousVersionDeltaViewModel>(versionDelta)).Return(_expectedViewModel);

                var nextVersionService = new PreviousVersionDeltaService(repo, null, mapper);
                _result = nextVersionService.Get(_systemItemId, _systemItemVersionDeltaId);
            }

            [Test]
            public void Should_return_view_model()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expectedViewModel);
            }

            [Test]
            public void Should_set_mapped_system_id()
            {
                _expectedViewModel.OldMappedSystemId.ShouldEqual(_oldMappedSystemId);
            }
        }

        [TestFixture]
        public class When_getting_version_deltas_for_system_item_that_does_not_exist : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Stub(x => x.Get(_systemItemId)).Return(null);

                var noteService = new PreviousVersionDeltaService(null, systemItemRepository, null);

                try
                {
                    noteService.Get(_systemItemId);
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
                _exception.Message.ShouldEqual("The system item with id '" + _systemItemId + "' does not exist.");
            }

            [Test]
            public void Should_throw_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_version_delta_that_does_not_exist : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemItemVersionDeltaId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemVersionDeltaRepo = GenerateStub<IRepository<SystemItemVersionDelta>>();
                systemItemVersionDeltaRepo.Stub(x => x.Get(_systemItemVersionDeltaId)).Return(null);

                var noteService = new PreviousVersionDeltaService(systemItemVersionDeltaRepo, null, null);

                try
                {
                    noteService.Get(_systemItemId, _systemItemVersionDeltaId);
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
                _exception.Message.ShouldEqual("The version record with id '" + _systemItemVersionDeltaId + "' does not exist.");
            }

            [Test]
            public void Should_throw_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_version_delta_with_incorrect_system_item_id : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemItemVersionDeltaId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItem systemItem = New.SystemItem.WithId(Guid.NewGuid());
                SystemItemVersionDelta systemItemVersionDelta = New.SystemItemVersionDelta.WithId(_systemItemVersionDeltaId).WithOldSystemItem(systemItem);

                var systemItemVersionDeltaRepo = GenerateStub<IRepository<SystemItemVersionDelta>>();
                systemItemVersionDeltaRepo.Stub(x => x.Get(_systemItemVersionDeltaId)).Return(systemItemVersionDelta);

                var noteService = new PreviousVersionDeltaService(systemItemVersionDeltaRepo, null, null);

                try
                {
                    noteService.Get(_systemItemId, _systemItemVersionDeltaId);
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
                _exception.Message.ShouldEqual("The version record with id '" + _systemItemVersionDeltaId + "' does not have system item '" + _systemItemId + "' as its new version.");
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