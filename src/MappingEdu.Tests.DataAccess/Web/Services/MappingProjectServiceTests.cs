// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using MappingEdu.Core.Domain;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;
using ProjectStatusType = MappingEdu.Core.Domain.Enumerations.ProjectStatusType;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class MappingProjectServiceTests
    {
        [TestFixture]
        public class When_creating_mapping_project : EmptyDatabaseTestBase
        {
            private Guid _mappingProjectId;
            private MappedSystem _sourceMappedSystem;
            private MappedSystem _targetMappedSystem;
            private MappingProjectViewModel _result;
            private const string SourceSystemName = "System Name";
            private const string SourceSystemVersion = "1.2.0";
            private const string TargetSystemName = "Target System";
            private const string TargetSystemVersion = "1.3.0";
            private const string ProjectName = "Test Project";
            private const string Description = "Description for Test Project";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                _sourceMappedSystem = CreateMappedSystem(dbContext, SourceSystemName, SourceSystemVersion);
                _targetMappedSystem = CreateMappedSystem(dbContext, TargetSystemName, TargetSystemVersion);

                var mappingProjectCreateModel = new MappingProjectCreateModel
                {
                    ProjectName = ProjectName,
                    Description = Description,
                    SourceDataStandardId = _sourceMappedSystem.MappedSystemId,
                    TargetDataStandardId = _targetMappedSystem.MappedSystemId
                };

                var mappingProjectService = GetInstance<IMappingProjectService>();
                _result = mappingProjectService.Post(mappingProjectCreateModel);
                _mappingProjectId = _result.MappingProjectId;
            }

            [Test]
            public void Should_create_mapping_project()
            {
                var dbContext = CreateDbContext();
                var mappingProject = dbContext.MappingProjects.Single(data => data.MappingProjectId == _mappingProjectId);
                mappingProject.ShouldNotBeNull();
                mappingProject.ProjectName.ShouldEqual(ProjectName);
                mappingProject.Description.ShouldEqual(Description);
                mappingProject.SourceDataStandardMappedSystemId.ShouldEqual(_sourceMappedSystem.MappedSystemId);
                mappingProject.TargetDataStandardMappedSystemId.ShouldEqual(_targetMappedSystem.MappedSystemId);
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _result.ShouldNotBeNull();
                _result.MappingProjectId.ShouldNotEqual(Guid.Empty);
                _result.ProjectName.ShouldEqual(ProjectName);
                _result.Description.ShouldEqual(Description);
                _result.ProjectStatusTypeName.ShouldEqual(ProjectStatusType.Active.Name);
                _result.SourceDataStandardId.ShouldEqual(_sourceMappedSystem.MappedSystemId);
                _result.TargetDataStandardId.ShouldEqual(_targetMappedSystem.MappedSystemId);
            }
        }

        [TestFixture]
        public class When_updating_mapping_project : EmptyDatabaseTestBase
        {
            private Guid _mappingProjectId;
            private MappingProject _mappingProject;
            private MappedSystem _sourceMappedSystem;
            private MappedSystem _targetMappedSystem;
            private MappingProjectViewModel _result;
            private const string SourceSystemName = "System Name";
            private const string SourceSystemVersion = "1.2.0";
            private const string TargetSystemName = "Target System";
            private const string TargetSystemVersion = "1.3.0";
            private const string ProjectName = "Test Project";
            private const string Description = "Description for Test Project";

            private const string UpdatedProjectName = "Testy Projecty";
            private const string UpdatedDescription = "Desc for Tesy Projecty";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                _sourceMappedSystem = CreateMappedSystem(dbContext, SourceSystemName, SourceSystemVersion);
                _targetMappedSystem = CreateMappedSystem(dbContext, TargetSystemName, TargetSystemVersion);
                _mappingProject = CreateMappingProject(dbContext, ProjectName, Description,
                    _sourceMappedSystem, _targetMappedSystem);
                _mappingProjectId = _mappingProject.MappingProjectId;

                var mappingProjectEditModel = new MappingProjectEditModel
                {
                    ProjectName = UpdatedProjectName,
                    Description = UpdatedDescription,
                    SourceDataStandardId = _targetMappedSystem.MappedSystemId,
                    TargetDataStandardId = _sourceMappedSystem.MappedSystemId,
                    ProjectStatusTypeId = ProjectStatusType.Active.Id
                };

                var mappingProjectService = GetInstance<IMappingProjectService>();
                _result = mappingProjectService.Put(_mappingProjectId, mappingProjectEditModel);
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _result.ShouldNotBeNull();
                _result.MappingProjectId.ShouldNotEqual(Guid.Empty);
                _result.ProjectName.ShouldEqual(UpdatedProjectName);
                _result.ProjectStatusTypeName.ShouldEqual(ProjectStatusType.Active.Name);
                _result.Description.ShouldEqual(UpdatedDescription);
                _result.SourceDataStandardId.ShouldEqual(_targetMappedSystem.MappedSystemId);
                _result.TargetDataStandardId.ShouldEqual(_sourceMappedSystem.MappedSystemId);
            }

            [Test]
            public void Should_update_mapping_project()
            {
                var dbContext = CreateDbContext();
                var mappingProject = dbContext.MappingProjects.Single(data => data.MappingProjectId == _mappingProjectId);
                mappingProject.ShouldNotBeNull();
                mappingProject.ProjectName.ShouldEqual(UpdatedProjectName);
                mappingProject.Description.ShouldEqual(UpdatedDescription);
                mappingProject.SourceDataStandardMappedSystemId.ShouldEqual(_targetMappedSystem.MappedSystemId);
                mappingProject.TargetDataStandardMappedSystemId.ShouldEqual(_sourceMappedSystem.MappedSystemId);
            }
        }

        [TestFixture]
        public class When_deleting_mapping_project : EmptyDatabaseTestBase
        {
            private Guid _mappingProjectId;
            private MappedSystem _sourceMappedSystem;
            private MappedSystem _targetMappedSystem;
            private MappingProject _mappingProject;
            private const string SourceSystemName = "System Name";
            private const string SourceSystemVersion = "1.2.0";
            private const string TargetSystemName = "Target System";
            private const string TargetSystemVersion = "1.3.0";
            private const string ProjectName = "Test Project";
            private const string Description = "Description for Test Project";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                _sourceMappedSystem = CreateMappedSystem(dbContext, SourceSystemName, SourceSystemVersion);
                _targetMappedSystem = CreateMappedSystem(dbContext, TargetSystemName, TargetSystemVersion);
                _mappingProject = CreateMappingProject(dbContext, ProjectName, Description,
                    _sourceMappedSystem, _targetMappedSystem);
                _mappingProject.IsActive.ShouldBeTrue();
                _mappingProjectId = _mappingProject.MappingProjectId;

                var mappingProjectService = GetInstance<IMappingProjectService>();
                mappingProjectService.Delete(_mappingProjectId);
            }

            [Test]
            public void Should_create_mapping_project()
            {
                var dbContext = CreateDbContext();
                var mappingProject = dbContext.MappingProjects.Single(data => data.MappingProjectId == _mappingProjectId);
                mappingProject.ShouldNotBeNull();
                mappingProject.ProjectName.ShouldEqual(ProjectName);
                mappingProject.Description.ShouldEqual(Description);
                mappingProject.SourceDataStandardMappedSystemId.ShouldEqual(_sourceMappedSystem.MappedSystemId);
                mappingProject.TargetDataStandardMappedSystemId.ShouldEqual(_targetMappedSystem.MappedSystemId);
                mappingProject.IsActive.ShouldBeFalse();
            }
        }
    }
}