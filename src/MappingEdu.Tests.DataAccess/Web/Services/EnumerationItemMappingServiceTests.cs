// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.EnumerationItemMapping;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;
using StatusReasonType = MappingEdu.Core.Domain.Enumerations.MappingStatusReasonType;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class EnumerationItemMappingServiceTests
    {
        [TestFixture]
        public class When_creating_an_enumeration_item_map : EmptyDatabaseTestBase
        {
            private EnumerationItemMappingViewModel _result;
            private Guid _sourceSystemEnumerationItemId;
            private Guid _targetSystemEnumerationItemId;
            private readonly EnumerationMappingStatusType _enumerationMappingStatusType = EnumerationMappingStatusType.ProposedEnumeration;
            private readonly EnumerationMappingStatusReasonType _enumerationMappingStatusReasonType = EnumerationMappingStatusReasonType.DerivedValue;
            private Guid _systemItemMapId;
            private const bool DeferredMapping = true;
            private const string SystemName = "System Name";
            private const string SystemVersion = "1.2.0";
            private const string DomainName = "Domain Name";
            private const string DomainDefinition = "Domain Definition";
            private const string DomainUrl = "http://domain.url";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                var domain = CreateDomain(dbContext, mappedSystem, DomainName, DomainDefinition, DomainUrl);
                var enumeration = CreateEnumeration(dbContext, domain, "Enumeration Name");
                var enumerationItem = CreateEnumerationItem(dbContext, enumeration, "CODE", "Description", "Short Description");
                var targetEnumerationItem = CreateEnumerationItem(dbContext, enumeration, "TARGET", "Target", "Short Target");

                var mappedSystem2 = CreateMappedSystem(dbContext, "Another System", "1");
                var domain2 = CreateDomain(dbContext, mappedSystem2, "domain2");
                var enum2 = CreateEnumeration(dbContext, domain2, "enum2");
                var mappingProject = CreateMappingProject(
                    dbContext, "Project Name", "Map This to That", mappedSystem, mappedSystem2);
                var systemItemMap = CreateSystemItemMap(dbContext, mappingProject, enum2);

                _systemItemMapId = systemItemMap.SystemItemMapId;
                _sourceSystemEnumerationItemId = enumerationItem.SystemEnumerationItemId;
                _targetSystemEnumerationItemId = targetEnumerationItem.SystemEnumerationItemId;

                var enumerationItemMappingService = GetInstance<IEnumerationItemMappingService>();

                var createModel = new EnumerationItemMappingCreateModel
                {
                    SourceSystemEnumerationItemId = enumerationItem.SystemEnumerationItemId,
                    TargetSystemEnumerationItemId = targetEnumerationItem.SystemEnumerationItemId,
                    DeferredMapping = DeferredMapping,
                    EnumerationMappingStatusTypeId = _enumerationMappingStatusType.Id,
                    EnumerationMappingStatusReasonTypeId = _enumerationMappingStatusReasonType.Id
                };

                _result = enumerationItemMappingService.Post(_systemItemMapId, createModel);
            }

            [Test]
            public void Should_create_an_element_system_item_map()
            {
                var dbContext = CreateDbContext();
                var enumerationItemMap = dbContext.SystemEnumerationItemMaps.First(x => x.SystemEnumerationItemMapId == _result.SystemEnumerationItemMapId);
                enumerationItemMap.SourceSystemEnumerationItemId.ShouldEqual(_sourceSystemEnumerationItemId);
                enumerationItemMap.TargetSystemEnumerationItemId.ShouldEqual(_targetSystemEnumerationItemId);
                enumerationItemMap.SystemItemMapId.ShouldEqual(_systemItemMapId);
                enumerationItemMap.DeferredMapping.ShouldEqual(DeferredMapping);
                enumerationItemMap.EnumerationMappingStatusType.ShouldEqual(_enumerationMappingStatusType);
                enumerationItemMap.EnumerationMappingStatusReasonType.ShouldEqual(_enumerationMappingStatusReasonType);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _result.SystemItemMapId.ShouldEqual(_systemItemMapId);
                _result.SourceSystemEnumerationItemId.ShouldEqual(_sourceSystemEnumerationItemId);
            }
        }

        [TestFixture]
        public class When_updating_an_element_system_item_map : EmptyDatabaseTestBase
        {
            private EnumerationItemMappingViewModel _result;
            private Guid _enumerationItemMapId;
            private Guid _sourceSystemEnumerationItemId;
            private Guid? _targetSystemEnumerationItemId;
            private const bool DeferredMapping = false;
            private const string SystemName = "System Name";
            private const string SystemVersion = "1.2.0";
            private const string DomainName = "Domain Name";
            private const string DomainDefinition = "Domain Definition";
            private const string DomainUrl = "http://domain.url";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                var otherMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(
                    dbContext, "Project Name", "Map This to That", mappedSystem, otherMappedSystem);
                var domain = CreateDomain(dbContext, mappedSystem, DomainName, DomainDefinition, DomainUrl);
                var enumeration = CreateEnumeration(dbContext, domain, "Enumeration Name");
                var enumerationItem = CreateEnumerationItem(dbContext, enumeration, "CODE", "Description", "Short Description");
                var target = CreateEnumerationItem(dbContext, domain, "TARGET", "Target Description", "Target Short");
                var mappedSystem2 = CreateMappedSystem(dbContext, "system2", "1");
                var systemItemMap = CreateSystemItemMap(dbContext, mappingProject, enumeration);
                var enumerationItemMap = CreatEnumerationItemMap(dbContext, systemItemMap, enumerationItem);
                _enumerationItemMapId = enumerationItemMap.SystemEnumerationItemMapId;

                _targetSystemEnumerationItemId = target.SystemEnumerationItemId;

                _sourceSystemEnumerationItemId = enumerationItem.SystemEnumerationItemId;

                var enumerationItemMappingService = GetInstance<IEnumerationItemMappingService>();

                var editModel = new EnumerationItemMappingEditModel
                {
                    DeferredMapping = DeferredMapping,
                    EnumerationMappingStatusReasonTypeId = EnumerationMappingStatusReasonType.DerivedValue.Id,
                    EnumerationMappingStatusTypeId = EnumerationMappingStatusType.ApprovedEnumeration.Id,
                    TargetSystemEnumerationItemId = _targetSystemEnumerationItemId
                };

                _result = enumerationItemMappingService.Put(systemItemMap.SystemItemMapId, _enumerationItemMapId, editModel);
            }

            [Test]
            public void Should_create_an_element_system_item_map()
            {
                var dbContext = CreateDbContext();
                var enumerationItemMap = dbContext.SystemEnumerationItemMaps.First(x => x.SystemEnumerationItemMapId == _result.SystemEnumerationItemMapId);
                enumerationItemMap.DeferredMapping.ShouldEqual(DeferredMapping);
                enumerationItemMap.EnumerationMappingStatusReasonType.ShouldEqual(EnumerationMappingStatusReasonType.DerivedValue);
                enumerationItemMap.EnumerationMappingStatusType.ShouldEqual(EnumerationMappingStatusType.ApprovedEnumeration);
                enumerationItemMap.SourceSystemEnumerationItemId.ShouldEqual(_sourceSystemEnumerationItemId);
                enumerationItemMap.TargetSystemEnumerationItemId.ShouldEqual(_targetSystemEnumerationItemId);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _result.DeferredMapping.ShouldEqual(DeferredMapping);
                _result.EnumerationMappingStatusReasonTypeId.ShouldEqual(EnumerationMappingStatusReasonType.DerivedValue.Id);
                _result.EnumerationMappingStatusTypeId.ShouldEqual(EnumerationMappingStatusType.ApprovedEnumeration.Id);
                _result.SourceSystemEnumerationItemId.ShouldEqual(_sourceSystemEnumerationItemId);
                _result.TargetSystemEnumerationItemId.ShouldEqual(_targetSystemEnumerationItemId);
            }
        }

        [TestFixture]
        public class When_deleting_an_element_system_item_map : EmptyDatabaseTestBase
        {
            private Guid _enumerationItemMapId;
            private Guid _sourceSystemEnumerationItemId;
            private const string SystemName = "System Name";
            private const string SystemVersion = "1.2.0";
            private const string DomainName = "Domain Name";
            private const string DomainDefinition = "Domain Definition";
            private const string DomainUrl = "http://domain.url";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                var otherMappedSystem = CreateMappedSystem(dbContext, "System Name", "2.0.0");
                var mappingProject = CreateMappingProject(
                    dbContext, "Project Name", "Map This to That", mappedSystem, otherMappedSystem);
                var domain = CreateDomain(dbContext, mappedSystem, DomainName, DomainDefinition, DomainUrl);
                var enumeration = CreateEnumeration(dbContext, domain, "Enumeration Name");
                var enumerationItem = CreateEnumerationItem(dbContext, enumeration, "CODE", "Description", "Short Description");
                var mappedSystem2 = CreateMappedSystem(dbContext, "system2", "1");
                var systemItemMap = CreateSystemItemMap(dbContext, mappingProject, enumeration);

                var enumerationItemMap = CreatEnumerationItemMap(dbContext, systemItemMap, enumerationItem);
                _enumerationItemMapId = enumerationItemMap.SystemEnumerationItemMapId;

                _sourceSystemEnumerationItemId = enumerationItem.SystemEnumerationItemId;

                var enumerationItemMappingService = GetInstance<IEnumerationItemMappingService>();

                enumerationItemMappingService.Delete(systemItemMap.SystemItemMapId, _enumerationItemMapId);
            }

            [Test]
            public void Should_remove_element_system_item_map()
            {
                var dbContext = CreateDbContext();
                dbContext.SystemEnumerationItemMaps.Find(_enumerationItemMapId).ShouldBeNull();
                dbContext.SystemEnumerationItems.Find(_sourceSystemEnumerationItemId).SourceSystemEnumerationItemMaps.Count.ShouldEqual(0);
            }
        }
    }
}