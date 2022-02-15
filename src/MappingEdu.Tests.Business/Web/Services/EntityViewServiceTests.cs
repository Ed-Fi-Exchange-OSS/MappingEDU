// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.BriefElement;
using MappingEdu.Service.Model.Entity;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.Model.SystemItemDefinition;
using MappingEdu.Service.Model.SystemItemName;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class EntityViewServiceTests
    {
        [TestFixture]
        public class When_getting_entity_view_model : TestBase
        {
            private readonly Guid _childEntityId = Guid.NewGuid();
            private EntityViewModel _result;
            private readonly SystemItemNameViewModel _systemItemNameExpected = new SystemItemNameViewModel();
            private readonly SystemItemDefinitionViewModel _systemItemDefinitionExpected = new SystemItemDefinitionViewModel();
            private readonly BriefElementViewModel[] _briefElementExpected = {new BriefElementViewModel(), new BriefElementViewModel()};
            private readonly NoteViewModel[] _notesExpected = {new NoteViewModel(), new NoteViewModel()};
            private readonly NextVersionDeltaViewModel[] _nextVersionsDeltaExpected = {new NextVersionDeltaViewModel()};
            private readonly PreviousVersionDeltaViewModel[] _previousVersionsDeltaExpected = {new PreviousVersionDeltaViewModel()};

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemNameService = GenerateStub<ISystemItemNameService>();
                systemItemNameService.Stub(x => x.Get(_childEntityId)).Return(_systemItemNameExpected);
                var systemItemDefinitionService = GenerateStub<ISystemItemDefinitionService>();
                systemItemDefinitionService.Stub(x => x.Get(_childEntityId)).Return(_systemItemDefinitionExpected);
                var noteService = GenerateStub<INoteService>();
                noteService.Stub(x => x.Get(_childEntityId)).Return(_notesExpected);
                var nextVersionDeltaService = GenerateStub<INextVersionDeltaService>();
                nextVersionDeltaService.Stub(x => x.Get(_childEntityId)).Return(_nextVersionsDeltaExpected);
                var previousVersionDeltaService = GenerateStub<IPreviousVersionDeltaService>();
                previousVersionDeltaService.Stub(x => x.Get(_childEntityId)).Return(_previousVersionsDeltaExpected);
                var briefElementService = GenerateStub<IBriefElementService>();
                briefElementService.Stub(x => x.Get(_childEntityId)).Return(_briefElementExpected);
                var elementService = GenerateStub<IElementService>();
                var systemItemCustomDetailService = GenerateStub<ISystemItemCustomDetailService>();

                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                var versionRepository = GenerateStub<IRepository<SystemItemVersionDelta>>();
                var customDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();
                var mappingRepository = GenerateStub<IRepository<SystemItemMap>>();

                var entityViewService = new EntityService(systemItemNameService, systemItemDefinitionService,
                    briefElementService, elementService, noteService, nextVersionDeltaService, previousVersionDeltaService,
                    systemItemCustomDetailService,
                    systemItemRepository, versionRepository, customDetailRepository, mappingRepository);
                _result = entityViewService.Get(_childEntityId);
            }

            [Test]
            public void Should_get_entity_view_model()
            {
                _result.ShouldNotBeNull();
                _result.SystemItemId.ShouldEqual(_childEntityId);
                _result.SystemItemName.ShouldEqual(_systemItemNameExpected);
                _result.SystemItemDefinition.ShouldEqual(_systemItemDefinitionExpected);
                _result.BriefElements.ShouldEqual(_briefElementExpected);
                _result.Notes.ShouldEqual(_notesExpected);
                _result.NextVersionDeltas.ShouldEqual(_nextVersionsDeltaExpected);
                _result.PreviousVersionDeltas.ShouldEqual(_previousVersionsDeltaExpected);
            }
        }
    }
}