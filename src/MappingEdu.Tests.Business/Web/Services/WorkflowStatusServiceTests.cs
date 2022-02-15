// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class WorkflowStatusServiceTests
    {
        [TestFixture]
        public class When_updating_a_system_item_map_workflow_flag : TestBase
        {
            private SystemItemMap _mapping;
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private readonly bool _flagged = true;
            private ISystemItemMapRepository _systemItemMappingRepository;

            [OneTimeSetUp]
            public void Setup()
            {
                _mapping = New.SystemItemMap.WithId(_systemItemMapId).WithSourceSystemItem(New.SystemItem.WithId(_sourceSystemItemId));
                _mapping.UserNotifications = new List<UserNotification>();

                _systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                _systemItemMappingRepository.Stub(x => x.Get(_mapping.SystemItemMapId))
                    .Return(_mapping);

                var systemItemMappingService = GenerateStub<ISystemItemMappingService>();
                var userRepository = GenerateStub<IUserRepository>();

                systemItemMappingService.Stub(x => x.Get(_sourceSystemItemId, _systemItemMapId))
                    .Return(new SystemItemMappingViewModel());

                IWorkflowStatusService service = new WorkflowStatusService(_systemItemMappingRepository, systemItemMappingService, userRepository);
                service.Put(_sourceSystemItemId, _systemItemMapId, new SystemItemMappingEditModel
                {
                    Flagged = _flagged
                });
            }

            [Test]
            public void Should_change_workflow_status()
            {
                _systemItemMappingRepository.Get(_systemItemMapId).Flagged.ShouldEqual(_flagged);
            }
        }

        [TestFixture]
        public class When_updating_a_system_item_map_workflow_status : TestBase
        {
            private SystemItemMap _mapping;
            private readonly Guid _sourceSystemItemId = Guid.NewGuid();
            private readonly Guid _systemItemMapId = Guid.NewGuid();
            private readonly WorkflowStatusType _workflowStatusType = WorkflowStatusType.Complete;
            private ISystemItemMapRepository _systemItemMappingRepository;

            [OneTimeSetUp]
            public void Setup()
            {
                _mapping = New.SystemItemMap.WithId(_systemItemMapId).WithSourceSystemItem(New.SystemItem.WithId(_sourceSystemItemId));
                _mapping.UserNotifications = new List<UserNotification>();

                _systemItemMappingRepository = GenerateStub<ISystemItemMapRepository>();
                _systemItemMappingRepository.Stub(x => x.Get(_mapping.SystemItemMapId))
                    .Return(_mapping);

                var systemItemMappingService = GenerateStub<ISystemItemMappingService>();
                var userRepository = GenerateStub<IUserRepository>();

                systemItemMappingService.Stub(x => x.Get(_sourceSystemItemId, _systemItemMapId))
                    .Return(new SystemItemMappingViewModel());

                IWorkflowStatusService service = new WorkflowStatusService(_systemItemMappingRepository, systemItemMappingService, userRepository);
                service.Put(_sourceSystemItemId, _systemItemMapId, new SystemItemMappingEditModel
                {
                    WorkflowStatusTypeId = _workflowStatusType.Id,
                    StatusNote = "Note"
                });
            }

            [Test]
            public void Should_change_workflow_status()
            {
                var systemItemMap = _systemItemMappingRepository.Get(_systemItemMapId);
                systemItemMap.WorkflowStatusType.ShouldEqual(WorkflowStatusType.Complete);
                systemItemMap.StatusNote.ShouldEqual("Note");
            }
        }
    }
}