// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.BriefElement;
using MappingEdu.Service.Model.Entity;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.Model.Note;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class NewEntityServiceTests
    {
        [TestFixture]
        public class When_getting_a_new_entity : TestBase
        {
            private readonly Guid _childEntityId = Guid.NewGuid();
            private NewEntityViewModel _result;
            private Guid _entityId;
            private Guid _mappingProjectId;

            private readonly NoteViewModel[] _notesExpected = {new NoteViewModel(), new NoteViewModel()};
            private readonly NextVersionDeltaViewModel[] _nextVersionsDeltaExpected = {new NextVersionDeltaViewModel()};
            private readonly PreviousVersionDeltaViewModel[] _previousVersionsDeltaExpected = {new PreviousVersionDeltaViewModel()};
            private readonly BriefElementViewModel[] _briefElementExpected = {new BriefElementViewModel(), new BriefElementViewModel()};

            [OneTimeSetUp]
            public void Setup()
            {
                _mappingProjectId = Guid.NewGuid();

                _entityId = Guid.NewGuid();
                SystemItem entity = New.SystemItem
                    .AsEntity
                    .WithId(_entityId)
                    .WithChildSystemItem(New.SystemItem)
                    .WithPreviousSystemItemVersion(
                        New.SystemItemVersionDelta.WithOldSystemItem(
                            New.SystemItem.WithName("Gamgee").WithParentSystemItem(New.SystemItem.WithName("Samwise"))
                                .WithMappedSystem(New.MappedSystem.WithSystemName("Mayor"))))
                    .WithNextSystemItemVersion(
                        New.SystemItemVersionDelta.WithNewSystemItem(
                            New.SystemItem.WithName("Took").WithParentSystemItem(New.SystemItem.WithName("Peregrin"))
                                .WithMappedSystem(New.MappedSystem.WithSystemName("Thain").WithSystemVersion("of the Shire"))));

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

                var versionRepository = GenerateStub<IRepository<SystemItemVersionDelta>>();
                var customDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();
                var mappingRepository = GenerateStub<IRepository<SystemItemMap>>();
                var userRepository = GenerateStub<IUserRepository>();

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Stub(sir => sir.Get(_entityId)).Return(entity);

                INewEntityService newEntityService = new NewEntityService(briefElementService,
                    elementService, nextVersionDeltaService, systemItemCustomDetailService, systemItemRepository, 
                    versionRepository, customDetailRepository, mappingRepository, userRepository);
                _result = newEntityService.Get(_entityId, _mappingProjectId);
            }

            [Test]
            public void Should_return_a_valid_view_model()
            {
                _result.ShouldNotBeNull();
                _result.SystemItemId.ShouldEqual(_entityId);
                _result.Elements.ShouldNotBeNull();
                _result.Elements.Length.ShouldBeGreaterThanOrEqualTo(1);
                _result.PreviousVersions.Length.ShouldBeGreaterThanOrEqualTo(1);
                string.Join(" ", _result.PreviousVersions[0].PreviousVersionItems.Select(p => p.Name)).ShouldEqual("Mayor  Samwise Gamgee");
                _result.NextVersions.Length.ShouldBeGreaterThanOrEqualTo(1);
                string.Join(" ", _result.NextVersions[0].NextVersionItems.Select(n => n.Name)).ShouldEqual("Thain of the Shire Peregrin Took");
            }
        }
    }
}