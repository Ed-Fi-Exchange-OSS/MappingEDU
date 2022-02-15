// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class MappingProjectStatusTests
    {
        [TestFixture]
        public class when_getting_a_project_status_for_a_project_with_all_approved_items : TestBase
        {
            private MappingProjectStatusViewModel _result;
            private readonly Guid _mappingProjectId = Guid.NewGuid();
            private SystemItem[] _systemItems;

            [OneTimeSetUp]
            public void SetUp()
            {
                var mappedSystem = New.MappedSystem;
                var mappingProject = New.MappingProject.IsActive(true).WithSource(mappedSystem).WithId(_mappingProjectId);
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();
                mappingProjectRepository.Expect(x => x.Get(_mappingProjectId)).Return(mappingProject);

                _systemItems = new SystemItem[]
                {
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element").WithType(ItemType.Element).WithDefinition("I am an element"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Enumeration").WithType(ItemType.Enumeration).WithDefinition("I am an enumeration"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element 2").WithType(ItemType.Element).WithDefinition("I am an element too")
                };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Expect(x => x.GetAllItems()).Return(_systemItems.AsQueryable());

                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[0])
                    .WithBusinessLogic("i am the logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);
                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[1])
                    .WithBusinessLogic("i am enum logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);
                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[2])
                    .WithBusinessLogic("i am more logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);

                IMappingProjectStatusService service = new MappingProjectStatusService(mappingProjectRepository, systemItemRepository);
                _result = service.Get(_mappingProjectId);
            }

            [Test]
            public void ShouldBeApproved()
            {
                _result.Approved.ShouldBeTrue();
            }

            [Test]
            public void ShouldReturnModel()
            {
                _result.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class when_getting_a_project_status_for_a_project_without_all_approved_items : TestBase
        {
            private MappingProjectStatusViewModel _result;
            private readonly Guid _mappingProjectId = Guid.NewGuid();
            private SystemItem[] _systemItems;

            [OneTimeSetUp]
            public void SetUp()
            {
                var mappedSystem = New.MappedSystem;
                var mappingProject = New.MappingProject.IsActive(true).WithSource(mappedSystem).WithId(_mappingProjectId);
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();
                mappingProjectRepository.Expect(x => x.Get(_mappingProjectId)).Return(mappingProject);

                _systemItems = new SystemItem[]
                {
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element").WithType(ItemType.Element).WithDefinition("I am an element"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Enumeration").WithType(ItemType.Enumeration).WithDefinition("I am an enumeration"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element 2").WithType(ItemType.Element).WithDefinition("I am an element too")
                };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Expect(x => x.GetAllItems()).Return(_systemItems.AsQueryable());

                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[0])
                    .WithBusinessLogic("i am the logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);
                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[1])
                    .WithBusinessLogic("i am enum logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);
                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[2])
                    .WithBusinessLogic("i am more logic")
                    .WithWorkflowStatus(WorkflowStatusType.Complete)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);

                IMappingProjectStatusService service = new MappingProjectStatusService(mappingProjectRepository, systemItemRepository);
                _result = service.Get(_mappingProjectId);
            }

            [Test]
            public void ShouldNotBeApproved()
            {
                _result.Approved.ShouldBeFalse();
            }

            [Test]
            public void ShouldReturnModel()
            {
                _result.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class when_getting_a_project_status_for_a_project_with_unmapped_items : TestBase
        {
            private MappingProjectStatusViewModel _result;
            private readonly Guid _mappingProjectId = Guid.NewGuid();
            private SystemItem[] _systemItems;

            [OneTimeSetUp]
            public void SetUp()
            {
                var mappedSystem = New.MappedSystem;
                var mappingProject = New.MappingProject.IsActive(true).WithSource(mappedSystem).WithId(_mappingProjectId);
                var mappingProjectRepository = GenerateStub<IMappingProjectRepository>();
                mappingProjectRepository.Expect(x => x.Get(_mappingProjectId)).Return(mappingProject);

                _systemItems = new SystemItem[]
                {
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element").WithType(ItemType.Element).WithDefinition("I am an element"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Enumeration").WithType(ItemType.Enumeration).WithDefinition("I am an enumeration"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element 2").WithType(ItemType.Element).WithDefinition("I am an element too"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element 3").WithType(ItemType.Element).WithDefinition("I am an element three")
                };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Expect(x => x.GetAllItems()).Return(_systemItems.AsQueryable());

                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[0])
                    .WithBusinessLogic("i am the logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);
                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[1])
                    .WithBusinessLogic("i am enum logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);
                New.SystemItemMap.WithMappingProject(mappingProject)
                    .WithSourceSystemItem(_systemItems[2])
                    .WithBusinessLogic("i am more logic")
                    .WithWorkflowStatus(WorkflowStatusType.Approved)
                    .WithMappingMethod(MappingMethodType.EnterMappingBusinessLogic);

                IMappingProjectStatusService service = new MappingProjectStatusService(mappingProjectRepository, systemItemRepository);
                _result = service.Get(_mappingProjectId);
            }

            [Test]
            public void ShouldNotBeApproved()
            {
                _result.Approved.ShouldBeFalse();
            }

            [Test]
            public void ShouldReturnModel()
            {
                _result.ShouldNotBeNull();
            }
        }
    }
}