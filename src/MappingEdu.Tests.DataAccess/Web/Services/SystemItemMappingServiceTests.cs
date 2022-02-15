// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;
using StatusReasonType = MappingEdu.Core.Domain.Enumerations.MappingStatusReasonType;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class SystemItemMappingServiceTests
    {
        [TestFixture]
        public class When_creating_an_element_system_item_map : EmptyDatabaseTestBase
        {
            private SystemItemMappingViewModel _result;
            private Guid _sourceSystemItemId;
            private Guid _mappingProjectId;
            private SystemItemMappingCreateModel _createModel;
            private const string SystemName = "System Name";
            private const string SystemVersion = "1.2.0";
            private const string ProjectName = "Project Name";
            private const string ProjectDescription = "Map This to That";
            private const string DomainName = "Domain Name";
            private const string DomainDefinition = "Domain Definition";
            private const string DomainUrl = "http://domain.url";
            private const string EntityName = "Entity Name";
            private const string EntityDefinition = "Entity Definition";
            private const string ElementName = "Element Name";
            private const string ElementDefinition = "Element Definition";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var sourceMappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                var targetMappedSystem = CreateMappedSystem(dbContext, "Other System Name", SystemVersion);
                var mappingProject = CreateMappingProject(
                    dbContext, ProjectName, ProjectDescription, sourceMappedSystem, targetMappedSystem);
                var domain = CreateDomain(dbContext, sourceMappedSystem, DomainName, DomainDefinition, DomainUrl);
                var entity = CreateEntity(dbContext, domain, EntityName, EntityDefinition);
                var element = CreateElement(dbContext, entity, ElementName, ElementDefinition);
                _sourceSystemItemId = element.SystemItemId;
                _mappingProjectId = mappingProject.MappingProjectId;

                var systemItemMappingService = GetInstance<ISystemItemMappingService>();

                _createModel = new SystemItemMappingCreateModel
                {
                    MappingProjectId = mappingProject.MappingProjectId,
                    WorkflowStatusTypeId = WorkflowStatusType.Incomplete.Id,
                    MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id
                };

                _result = systemItemMappingService.Post(_sourceSystemItemId, _createModel);
            }

            [Test]
            public void Should_create_an_element_system_item_map()
            {
                var dbContext = CreateDbContext();
                var systemItemMap = dbContext.SystemItemMaps.First(x => x.SystemItemMapId == _result.SystemItemMapId);
                systemItemMap.MappingProjectId.ShouldEqual(_mappingProjectId);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _result.MappingProject.ShouldNotBeNull();
                _result.MappingProject.MappingProjectId.ShouldEqual(_mappingProjectId);
                _result.MappingProject.ProjectName.ShouldEqual(ProjectName);
                _result.MappingProject.Description.ShouldEqual(ProjectDescription);
            }
        }

        [TestFixture]
        public class When_updating_an_element_system_item_map : EmptyDatabaseTestBase
        {
            private SystemItemMappingViewModel _result;
            private Guid _sourceSystemItemId;
            private Guid _targetSystemItemId;
            private Guid _mappingProjectId;
            private const string SystemName = "System Name";
            private const string SystemVersion = "1.2.0";
            private const string ProjectName = "Project Name";
            private const string ProjectDescription = "Map This to That";
            private const string DomainName = "DomainName";
            private const string DomainDefinition = "Domain Definition";
            private const string DomainUrl = "http://domain.url";
            private const string EntityName = "EntityName";
            private const string EntityDefinition = "Entity Definition";
            private const string ElementName = "ElementName";
            private const string ElementDefinition = "Element Definition";
            private const string BusinessLogic = "Business Logic [DomainName.EntityName.ElementName]";
            private const string OmissionReason = "No Reason";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var targetMappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                var sourceMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(
                    dbContext, ProjectName, ProjectDescription, sourceMappedSystem, targetMappedSystem);
                var domain = CreateDomain(dbContext, targetMappedSystem, DomainName, DomainDefinition, DomainUrl);
                var entity = CreateEntity(dbContext, domain, EntityName, EntityDefinition);
                var element = CreateElement(dbContext, entity, ElementName, ElementDefinition);
                var elementSystemItemMap = CreateSystemItemMap(dbContext, mappingProject, element);

                _targetSystemItemId = element.SystemItemId;
                _sourceSystemItemId = element.SystemItemId;
                _mappingProjectId = mappingProject.MappingProjectId;

                var editModel = new SystemItemMappingEditModel
                {
                    BusinessLogic = BusinessLogic,
                    CompleteStatusTypeId = CompleteStatusType.Incomplete.Id,
                    DeferredMapping = false,
                    ExcludeInExternalReports = true,
                    MappingStatusTypeId = MappingStatusType.NeedFurtherReview.Id,
                    OmissionReason = OmissionReason,
                    MappingStatusReasonTypeId = StatusReasonType.PendingUseCase.Id,
                    MappingProjectId = mappingProject.MappingProjectId,
                    MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                    WorkflowStatusTypeId = WorkflowStatusType.Incomplete.Id
                };

                var systemItemMappingService = GetInstance<ISystemItemMappingService>();
                _result = systemItemMappingService.Put(element.SystemItemId, elementSystemItemMap.SystemItemMapId, editModel);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _result.BusinessLogic.ShouldEqual(BusinessLogic);
                _result.CompleteStatusType.ShouldEqual(CompleteStatusType.Incomplete);
                _result.DeferredMapping.ShouldEqual(false);
                _result.ExcludeInExternalReports.ShouldEqual(true);
                _result.MappingProject.MappingProjectId.ShouldEqual(_mappingProjectId);
                _result.MappingStatusReasonType.ShouldEqual(StatusReasonType.PendingUseCase);
                _result.MappingStatusType.ShouldEqual(MappingStatusType.NeedFurtherReview);
                _result.OmissionReason.ShouldEqual(OmissionReason);

                _result.MappingProject.ShouldNotBeNull();
                _result.MappingProject.MappingProjectId.ShouldEqual(_mappingProjectId);
                _result.MappingProject.ProjectName.ShouldEqual(ProjectName);
                _result.MappingProject.Description.ShouldEqual(ProjectDescription);
            }

            [Test]
            public void Should_update_element_system_item_map()
            {
                var dbContext = CreateDbContext();
                var systemItemMap = dbContext.SystemItemMaps.First(x => x.SystemItemMapId == _result.SystemItemMapId);
                systemItemMap.ShouldNotBeNull();
                systemItemMap.SystemItemMapId.ShouldNotEqual(Guid.Empty);
                systemItemMap.SourceSystemItemId.ShouldEqual(_sourceSystemItemId);
                systemItemMap.TargetSystemItems.Count.ShouldEqual(1);
                systemItemMap.TargetSystemItems.First().SystemItemId.ShouldEqual(_targetSystemItemId);

                systemItemMap.BusinessLogic.ShouldEqual(BusinessLogic);
                systemItemMap.CompleteStatusType.ShouldEqual(CompleteStatusType.Incomplete);
                systemItemMap.DeferredMapping.ShouldEqual(false);
                systemItemMap.ExcludeInExternalReports.ShouldEqual(true);
                systemItemMap.MappingProjectId.ShouldEqual(_mappingProjectId);
                systemItemMap.MappingStatusReasonType.ShouldEqual(StatusReasonType.PendingUseCase);
                systemItemMap.MappingStatusType.ShouldEqual(MappingStatusType.NeedFurtherReview);
                systemItemMap.OmissionReason.ShouldEqual(OmissionReason);
            }
        }

        [TestFixture]
        public class When_updating_an_element_system_item_map_with_empty_business_logic : EmptyDatabaseTestBase
        {
            private SystemItemMappingViewModel _result;
            private Guid _sourceSystemItemId;
            private Guid _mappingProjectId;
            private const string SystemName = "System Name";
            private const string SystemVersion = "1.2.0";
            private const string ProjectName = "Project Name";
            private const string ProjectDescription = "Map This to That";
            private const string DomainName = "DomainName";
            private const string DomainDefinition = "Domain Definition";
            private const string DomainUrl = "http://domain.url";
            private const string EntityName = "EntityName";
            private const string EntityDefinition = "Entity Definition";
            private const string ElementName = "ElementName";
            private const string ElementDefinition = "Element Definition";
            private const string BusinessLogic = "";
            private const string OmissionReason = "No Reason";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                var otherMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(
                    dbContext, ProjectName, ProjectDescription, mappedSystem, otherMappedSystem);
                var domain = CreateDomain(dbContext, mappedSystem, DomainName, DomainDefinition, DomainUrl);
                var entity = CreateEntity(dbContext, domain, EntityName, EntityDefinition);
                var element = CreateElement(dbContext, entity, ElementName, ElementDefinition);
                var elementSystemItemMap = CreateSystemItemMap(dbContext, mappingProject, element);

                var targetSystemItem = CreateElement(dbContext, entity, "Target Item");
                elementSystemItemMap.TargetSystemItems.Add(targetSystemItem);
                dbContext.SaveChanges();

                _sourceSystemItemId = element.SystemItemId;
                _mappingProjectId = mappingProject.MappingProjectId;

                var editModel = new SystemItemMappingEditModel
                {
                    BusinessLogic = BusinessLogic,
                    CompleteStatusTypeId = CompleteStatusType.Incomplete.Id,
                    DeferredMapping = false,
                    ExcludeInExternalReports = true,
                    MappingStatusTypeId = MappingStatusType.NeedFurtherReview.Id,
                    OmissionReason = OmissionReason,
                    MappingStatusReasonTypeId = StatusReasonType.PendingUseCase.Id,
                    MappingProjectId = mappingProject.MappingProjectId,
                    MappingMethodTypeId = MappingMethodType.EnterMappingBusinessLogic.Id,
                    WorkflowStatusTypeId = WorkflowStatusType.Incomplete.Id
                };

                var systemItemMappingService = GetInstance<ISystemItemMappingService>();
                _result = systemItemMappingService.Put(element.SystemItemId, elementSystemItemMap.SystemItemMapId, editModel);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _result.BusinessLogic.ShouldEqual(BusinessLogic);
                _result.CompleteStatusType.ShouldEqual(CompleteStatusType.Incomplete);
                _result.DeferredMapping.ShouldEqual(false);
                _result.ExcludeInExternalReports.ShouldEqual(true);
                _result.MappingProject.MappingProjectId.ShouldEqual(_mappingProjectId);
                _result.MappingStatusReasonType.ShouldEqual(StatusReasonType.PendingUseCase);
                _result.MappingStatusType.ShouldEqual(MappingStatusType.NeedFurtherReview);
                _result.OmissionReason.ShouldEqual(OmissionReason);

                _result.MappingProject.ShouldNotBeNull();
                _result.MappingProject.MappingProjectId.ShouldEqual(_mappingProjectId);
                _result.MappingProject.ProjectName.ShouldEqual(ProjectName);
                _result.MappingProject.Description.ShouldEqual(ProjectDescription);
            }

            [Test]
            public void Should_update_element_system_item_map()
            {
                var dbContext = CreateDbContext();
                var systemItemMap = dbContext.SystemItemMaps.First(x => x.SystemItemMapId == _result.SystemItemMapId);
                systemItemMap.ShouldNotBeNull();
                systemItemMap.SystemItemMapId.ShouldNotEqual(Guid.Empty);
                systemItemMap.SourceSystemItemId.ShouldEqual(_sourceSystemItemId);
                systemItemMap.TargetSystemItems.Count.ShouldEqual(0);

                systemItemMap.BusinessLogic.ShouldEqual(BusinessLogic);
                systemItemMap.CompleteStatusType.ShouldEqual(CompleteStatusType.Incomplete);
                systemItemMap.DeferredMapping.ShouldEqual(false);
                systemItemMap.ExcludeInExternalReports.ShouldEqual(true);
                systemItemMap.MappingProjectId.ShouldEqual(_mappingProjectId);
                systemItemMap.MappingStatusReasonType.ShouldEqual(StatusReasonType.PendingUseCase);
                systemItemMap.MappingStatusType.ShouldEqual(MappingStatusType.NeedFurtherReview);
                systemItemMap.OmissionReason.ShouldEqual(OmissionReason);
            }
        }

        [TestFixture]
        public class When_deleting_an_element_system_item_map : EmptyDatabaseTestBase
        {
            private Guid _sourceSystemItemId;
            private Guid _systemItemMapId;
            private const string SystemName = "System Name";
            private const string SystemVersion = "1.2.0";
            private const string ProjectName = "Project Name";
            private const string ProjectDescription = "Map This to That";
            private const string DomainName = "Domain Name";
            private const string DomainDefinition = "Domain Definition";
            private const string DomainUrl = "http://domain.url";
            private const string EntityName = "Entity Name";
            private const string EntityDefinition = "Entity Definition";
            private const string ElementName = "Element Name";
            private const string ElementDefinition = "Element Definition";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                var otherMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(
                    dbContext, ProjectName, ProjectDescription, mappedSystem, otherMappedSystem);
                var domain = CreateDomain(dbContext, mappedSystem, DomainName, DomainDefinition, DomainUrl);
                var entity = CreateEntity(dbContext, domain, EntityName, EntityDefinition);
                var element = CreateElement(dbContext, entity, ElementName, ElementDefinition);
                var elementSystemItemMap = CreateSystemItemMap(dbContext, mappingProject, element);

                _sourceSystemItemId = element.SystemItemId;
                _systemItemMapId = elementSystemItemMap.SystemItemMapId;

                var systemItemMappingService = GetInstance<ISystemItemMappingService>();
                systemItemMappingService.Delete(element.SystemItemId, _systemItemMapId);
            }

            [Test]
            public void Should_remove_element_system_item_map()
            {
                var dbContext = CreateDbContext();
                dbContext.SystemItemMaps.Find(_systemItemMapId).ShouldBeNull();
                dbContext.SystemItems.Find(_sourceSystemItemId).SourceSystemItemMaps.Count.ShouldEqual(0);
            }
        }
    }
}