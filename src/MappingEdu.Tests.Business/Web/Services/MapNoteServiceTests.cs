// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MapNote;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class MapNoteServiceTests
    {
        [TestFixture]
        public class When_getting_map_notes_for_system_item_map : TestBase
        {
            private MapNoteViewModel[] _mapNoteViewModels;
            private MapNoteViewModel[] _expected;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMapId = Guid.NewGuid();
                var mapNoteId1 = Guid.NewGuid();
                var mapNoteId2 = Guid.NewGuid();
                _expected = new[]
                {
                    new MapNoteViewModel {MapNoteId = mapNoteId1, Title = "Title1", Notes = "Notes1", CreateDate = DateTime.Now},
                    new MapNoteViewModel {MapNoteId = mapNoteId2, Title = "Title2", Notes = "Notes2", CreateDate = DateTime.Now}
                };

                var mapNoteRepository = GenerateStub<IRepository<MapNote>>();

                SystemItemMap map = New.SystemItemMap.WithId(systemItemMapId);
                New.MapNote.WithId(mapNoteId1).WithSystemItemMap(map);
                New.MapNote.WithId(mapNoteId2).WithSystemItemMap(map);

                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                systemItemMapRepository.Expect(x => x.Get(systemItemMapId)).Return(map);

                var mapper = GenerateStub<IMapper>();
                mapper.Expect(x => x.Map<MapNoteViewModel[]>(map.MapNotes)).Return(_expected);

                IMapNoteService mapNoteService = new MapNoteService(mapNoteRepository, systemItemMapRepository, mapper, null, null);
                _mapNoteViewModels = mapNoteService.Get(systemItemMapId);
            }

            [Test]
            public void Should_populate_view_models()
            {
                _mapNoteViewModels.ShouldNotBeNull();
                _mapNoteViewModels.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_map_notes_for_non_existant_system_item_map : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemMapId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var mapNoteRepository = GenerateStub<IRepository<MapNote>>();

                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                systemItemMapRepository.Expect(x => x.Get(_systemItemMapId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                IMapNoteService mapNoteService = new MapNoteService(mapNoteRepository, systemItemMapRepository, mapper, null, null);
                try
                {
                    mapNoteService.Get(_systemItemMapId);
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The system item map with id '{0}' does not exist.", _systemItemMapId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_specific_map_note : TestBase
        {
            private MapNoteViewModel _mapNoteViewModel;
            private MapNoteViewModel _expected;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemMapId = Guid.NewGuid();
                var mapNoteId = Guid.NewGuid();

                MapNote mapNote = New.MapNote.WithId(mapNoteId).WithSystemItemMap(New.SystemItemMap.WithId(systemItemMapId));
                var mapNoteRepository = GenerateStub<IRepository<MapNote>>();
                mapNoteRepository.Expect(x => x.Get(mapNoteId)).Return(mapNote);

                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();

                _expected = new MapNoteViewModel {MapNoteId = mapNoteId, Title = "Title", Notes = "Notes", CreateDate = DateTime.Now};
                var mapper = GenerateStub<IMapper>();
                mapper.Expect(x => x.Map<MapNoteViewModel>(mapNote)).Return(_expected);

                IMapNoteService mapNoteService = new MapNoteService(mapNoteRepository, systemItemMapRepository, mapper, null, null);
                _mapNoteViewModel = mapNoteService.Get(systemItemMapId, mapNoteId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _mapNoteViewModel.ShouldNotBeNull();
                _mapNoteViewModel.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_invalid_map_note : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _mapNoteId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemId = Guid.NewGuid();

                var mapNoteRepository = GenerateStub<IRepository<MapNote>>();
                mapNoteRepository.Expect(x => x.Get(_mapNoteId)).Return(null);

                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                var mapper = GenerateStub<IMapper>();

                IMapNoteService mapNoteService = new MapNoteService(mapNoteRepository, systemItemMapRepository, mapper, null, null);
                try
                {
                    mapNoteService.Get(systemItemId, _mapNoteId);
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The map note with id '{0}' does not exist.", _mapNoteId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_map_note_for_invalid_system_item_map : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _mapNoteId = Guid.NewGuid();
            private readonly Guid _systemItemMapId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                MapNote mapNote = New.MapNote.WithId(_mapNoteId).WithSystemItemMap(New.SystemItemMap.WithId(Guid.NewGuid()));
                var mapNoteRepository = GenerateStub<IRepository<MapNote>>();
                mapNoteRepository.Expect(x => x.Get(_mapNoteId)).Return(mapNote);

                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();

                var mapper = GenerateStub<IMapper>();

                IMapNoteService mapNoteService = new MapNoteService(mapNoteRepository, systemItemMapRepository, mapper, null, null);
                try
                {
                    mapNoteService.Get(_systemItemMapId, _mapNoteId);
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(
                    string.Format("The map note with id '{0}' does not have a parent system item map id of '{1}'.", _mapNoteId, _systemItemMapId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_creating_a_map_note_for_a_system_item_map_that_does_not_exist : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemMapId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var mapNoteRepository = GenerateStub<IRepository<MapNote>>();

                var systemItemMapRepository = GenerateStub<IRepository<SystemItemMap>>();
                systemItemMapRepository.Expect(x => x.Get(_systemItemMapId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                IMapNoteService mapNoteService = new MapNoteService(mapNoteRepository, systemItemMapRepository, mapper, null, null);
                try
                {
                    mapNoteService.Post(_systemItemMapId, new MapNoteCreateModel());
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The system item map with id '{0}' does not exist.", _systemItemMapId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }
    }
}