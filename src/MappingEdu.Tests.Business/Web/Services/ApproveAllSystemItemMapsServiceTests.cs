// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class ApproveAllSystemItemMapsServiceTests
    {
        [TestFixture]
        public class When_approving_reviewed_item_mappings : TestBase
        {
            private readonly Guid _mappingProjectId = Guid.NewGuid();
            private readonly Guid _map1Id = Guid.NewGuid();
            private readonly Guid _map2Id = Guid.NewGuid();
            private readonly Guid _map3Id = Guid.NewGuid();
            private MappingProjectViewModel _expected;
            private SystemItemMap[] _maps;
            private BulkActionResultsModel _result;

            [OneTimeSetUp]
            public void Setup()
            {
                MappingProject mappingProject = New.MappingProject.WithId(_mappingProjectId);
                _maps = new SystemItemMap[]
                {
                    New.SystemItemMap.WithMappingProject(mappingProject).WithId(_map1Id).WithWorkflowStatus(WorkflowStatusType.Reviewed),
                    New.SystemItemMap.WithMappingProject(mappingProject).WithId(_map2Id).WithWorkflowStatus(WorkflowStatusType.Approved),
                    New.SystemItemMap.WithMappingProject(mappingProject).WithId(_map3Id).WithWorkflowStatus(WorkflowStatusType.Incomplete)
                };

                var systemItemMapRepository = GenerateStub<ISystemItemMapRepository>();
                systemItemMapRepository.Stub(x => x.GetAllMaps()).Return(_maps.AsQueryable());

                _expected = new MappingProjectViewModel
                {
                    MappingProjectId = _mappingProjectId
                };

                IApproveAllSystemItemMapsService approveAllSystemItemMapsService =
                    new ApproveAllSystemItemMapsService(systemItemMapRepository);

                _result = approveAllSystemItemMapsService.Put(_mappingProjectId, _expected);
            }

            [Test]
            public void Should_approve_reviewed_mappings()
            {
                _result.CountUpdated.ShouldEqual(1);
                _maps.Count(m => m.WorkflowStatusTypeId == WorkflowStatusType.Approved.Id).ShouldEqual(2);
                _maps.Count(m => m.WorkflowStatusTypeId == WorkflowStatusType.Incomplete.Id).ShouldEqual(1);
            }
        }
    }
}
