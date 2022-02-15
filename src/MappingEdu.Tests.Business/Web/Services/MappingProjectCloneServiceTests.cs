// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class MappingProjectCloneServiceTests
    {
        [TestFixture]
        public class When_posting_a_mapping_project_clone_with_bad_guid : TestBase
        {
            private IMappingProjectService service;
            private IMappingProjectRepository mappingProjectRepository;

            [OneTimeSetUp]
            public void Setup()
            {
                mappingProjectRepository = GenerateStub<IMappingProjectRepository>();

                service = new MappingProjectService(mappingProjectRepository, null, null, null, null);
            }

            [Test]
            public void Should_throw_with_no_name()
            {
                Action action = () => service.Clone(Guid.NewGuid(), new MappingProjectCloneModel {MappingProjectId = Guid.NewGuid()});
                action.ShouldThrow<ArgumentNullException>();
            }
        }
    }
}