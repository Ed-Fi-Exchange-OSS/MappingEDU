// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Element;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.Model.SystemItemName;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class ElementViewServiceTests
    {
        [TestFixture]
        public class When_getting_element_view_model : TestBase
        {
            private readonly Guid _childEntityId = Guid.NewGuid();
            private ElementViewModel _result;
            private readonly SystemItemNameViewModel _systemItemNameExpected = new SystemItemNameViewModel();
            private readonly NoteViewModel[] _notesExpected = {new NoteViewModel(), new NoteViewModel()};
            private readonly NextVersionDeltaViewModel[] _nextVersionsDeltaExpected = {new NextVersionDeltaViewModel()};
            private readonly PreviousVersionDeltaViewModel[] _previousVersionsDeltaExpected = {new PreviousVersionDeltaViewModel()};
            private readonly ElementDetailsViewModel _elementDetailsExpected = new ElementDetailsViewModel();
            private readonly SystemItemMappingViewModel[] _systemItemMappingExpected = {new SystemItemMappingViewModel(), new SystemItemMappingViewModel()};
            private readonly SystemItemCustomDetailViewModel[] _systemItemCustomDetailsExpected = {new SystemItemCustomDetailViewModel(), new SystemItemCustomDetailViewModel()};

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemNameService = GenerateStub<ISystemItemNameService>();
                systemItemNameService.Stub(x => x.Get(_childEntityId)).Return(_systemItemNameExpected);
                var noteService = GenerateStub<INoteService>();
                noteService.Stub(x => x.Get(_childEntityId)).Return(_notesExpected);
                var nextVersionDeltaService = GenerateStub<INextVersionDeltaService>();
                nextVersionDeltaService.Stub(x => x.Get(_childEntityId)).Return(_nextVersionsDeltaExpected);
                var previousVersionDeltaService = GenerateStub<IPreviousVersionDeltaService>();
                previousVersionDeltaService.Stub(x => x.Get(_childEntityId)).Return(_previousVersionsDeltaExpected);

                var elementDetailsService = GenerateStub<IElementDetailsService>();
                elementDetailsService.Stub(x => x.Get(_childEntityId)).Return(_elementDetailsExpected);
                var systemItemMappingService = GenerateStub<ISystemItemMappingService>();
                systemItemMappingService.Stub(x => x.GetSourceMappings(_childEntityId)).Return(_systemItemMappingExpected);

                var systemItemCustomDetailService = GenerateStub<ISystemItemCustomDetailService>();
                systemItemCustomDetailService.Stub(x => x.Get(_childEntityId)).Return(_systemItemCustomDetailsExpected);

                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                var versionRepository = GenerateStub<IRepository<SystemItemVersionDelta>>();
                var customDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();
                var mappingRepository = GenerateStub<IRepository<SystemItemMap>>();

                var elementViewService = new ElementService(
                    systemItemNameService, noteService, nextVersionDeltaService, previousVersionDeltaService, elementDetailsService,
                    systemItemMappingService, systemItemCustomDetailService,
                    systemItemRepository, versionRepository, customDetailRepository, mappingRepository);
                _result = elementViewService.Get(_childEntityId);
            }

            [Test]
            public void Should_get_element_view_model()
            {
                _result.ShouldNotBeNull();
                _result.SystemItemId.ShouldEqual(_childEntityId);
                _result.SystemItemName.ShouldEqual(_systemItemNameExpected);
                _result.Notes.ShouldEqual(_notesExpected);
                _result.NextVersionDeltas.ShouldEqual(_nextVersionsDeltaExpected);
                _result.PreviousVersionDeltas.ShouldEqual(_previousVersionsDeltaExpected);
                _result.ElementDetails.ShouldEqual(_elementDetailsExpected);
                _result.SystemItemMappings.ShouldEqual(_systemItemMappingExpected);
            }
        }
    }
}