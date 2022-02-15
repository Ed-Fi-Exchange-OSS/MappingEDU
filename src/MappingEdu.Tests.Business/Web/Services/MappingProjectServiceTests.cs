// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Mapping;
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
    public class MappingProjectServiceTests
    {
        [TestFixture]
        public class When_getting_all_mapping_projects : TestBase
        {
            private MappingProjectViewModel[] _result;

            [OneTimeSetUp]
            public void Setup()
            {
                var userId = new Guid();

                Thread.CurrentPrincipal = new ClaimsPrincipal(IdentityFactory.CreateIdentity("TEST", "admin", "1", true));

                MappingProject[] mappingProjects =
                {
                    New.MappingProject.IsActive(true).WithCreatedById(userId),
                    New.MappingProject.IsActive(true).WithCreatedById(userId)
                };

                var repository = GenerateStub<IMappingProjectRepository>();
                repository.Expect(x => x.GetAll()).Return(mappingProjects);

                var userRepository = GenerateStub<IUserRepository>();
                userRepository.Expect(x => x.GetAllUsers()).Return((new List<ApplicationUser> {new ApplicationUser {Id = userId.ToString(), FirstName = "First", LastName = "Last", Email = "test@email.com"}}).AsQueryable());

                MappingProjectViewModel[] mappingProjectModels =
                {
                    new MappingProjectViewModel(),
                    new MappingProjectViewModel()
                };

                var mapper = GenerateStub<IMapper>();
                mapper.Expect(x => x.Map<MappingProjectViewModel>(mappingProjects[0])).Return(mappingProjectModels[0]);
                mapper.Expect(x => x.Map<MappingProjectViewModel>(mappingProjects[1])).Return(mappingProjectModels[1]);

                IMappingProjectService mappingProjectService = new MappingProjectService(repository, mapper, null, null, userRepository);

                _result = mappingProjectService.Get();
            }

            [Test]
            public void ShouldPopulateViewModels()
            {
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(2);
            }
        }

        [TestFixture]
        public class When_getting_a_mapping_project : TestBase
        {
            private MappingProjectViewModel _result;
            private Guid _mappingProjectId;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappingProjectId = Guid.NewGuid();

                var mappingProject = New.MappingProject.IsActive(true);
                var repository = GenerateStub<IMappingProjectRepository>();
                repository.Expect(x => x.Get(_mappingProjectId)).Return(mappingProject);

                var mapper = GenerateStub<IMapper>();
                mapper
                    .Expect(x => x.Map<MappingProjectViewModel>(mappingProject))
                    .Return(new MappingProjectViewModel()).IgnoreArguments();

                IMappingProjectService mappingProjectService = new MappingProjectService(repository, mapper, null, null, null);
                _result = mappingProjectService.Get(_mappingProjectId);
            }

            [Test]
            public void ShouldPopulateViewModel()
            {
                _result.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_source_mapping_projects : TestBase
        {
            private DataStandardMappingProjectsViewModel[] _result;
            private Guid _dataStandardId;

            [OneTimeSetUp]
            public void Setup()
            {
                _dataStandardId = Guid.NewGuid();

                MappingProject[] mappingProjects =
                {
                    New.MappingProject.IsActive(true),
                    New.MappingProject.IsActive(true)
                };

                var repository = GenerateStub<IMappingProjectRepository>();
                repository.Expect(x => x.GetSourceMappingProjects(_dataStandardId)).Return(mappingProjects);

                DataStandardMappingProjectsViewModel[] mappingProjectsViewModels =
                {
                    new DataStandardMappingProjectsViewModel(),
                    new DataStandardMappingProjectsViewModel()
                };

                var mapper = GenerateStub<IMapper>();
                mapper
                    .Expect(x => x.Map<DataStandardMappingProjectsViewModel[]>(mappingProjects))
                    .Return(mappingProjectsViewModels);

                IMappingProjectService mappingProjectService = new MappingProjectService(repository, mapper, null, null, null);
                _result = mappingProjectService.GetSourceMappingProjects(_dataStandardId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(2);
            }
        }

        [TestFixture]
        public class When_getting_target_mapping_projects : TestBase
        {
            private DataStandardMappingProjectsViewModel[] _result;
            private Guid _dataStandardId;

            [OneTimeSetUp]
            public void Setup()
            {
                _dataStandardId = Guid.NewGuid();

                MappingProject[] mappingProjects =
                {
                    New.MappingProject.IsActive(true),
                    New.MappingProject.IsActive(true)
                };

                var repository = GenerateStub<IMappingProjectRepository>();
                repository.Expect(x => x.GetTargetMappingProjects(_dataStandardId)).Return(mappingProjects);

                DataStandardMappingProjectsViewModel[] mappingProjectsViewModels =
                {
                    new DataStandardMappingProjectsViewModel(),
                    new DataStandardMappingProjectsViewModel()
                };

                var mapper = GenerateStub<IMapper>();
                mapper
                    .Expect(x => x.Map<DataStandardMappingProjectsViewModel[]>(mappingProjects))
                    .Return(mappingProjectsViewModels);

                IMappingProjectService mappingProjectService = new MappingProjectService(repository, mapper, null, null, null);
                _result = mappingProjectService.GetTargetMappingProjects(_dataStandardId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(2);
            }
        }
    }
}