// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Security.Claims;
using System.Threading;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Data.Entities.Repository
{
    public class MappingProjectRepositoryTests
    {
        public class When_getting_all_mapping_projects : EmptyDatabaseTestBase
        {
            private MappingProject[] _result;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                CreateMappingProject(dbContext, "project1", "desc1", CreateMappedSystem(dbContext, "system1", "1"), CreateMappedSystem(dbContext, "system2", "2"));
                CreateMappingProject(dbContext, "project2", "desc2", CreateMappedSystem(dbContext, "system3", "3"), CreateMappedSystem(dbContext, "system4", "4"), false);
                CreateMappingProject(dbContext, "project3", "desc3", CreateMappedSystem(dbContext, "system5", "5"), CreateMappedSystem(dbContext, "system6", "6"));
                CreateUser(dbContext, "1", "test", "user", "test@dataaccess.com");

                // TODO: Create a global approach to security in unit tests (cpt)
                var principal = new ClaimsPrincipal(identityFactory.CreateIdentity("TESTS", "test", "1", true));
                Thread.CurrentPrincipal = principal;

                var repository = GetInstance<IMappingProjectRepository>();
                _result = repository.GetAll();
            }

            [Test]
            public void Should_return_active_mapping_projects()
            {
                _result.ShouldNotBeNull();
                _result.Count().ShouldEqual(2);
                _result.Count(x => x.ProjectName == "project1").ShouldEqual(1);
                _result.Count(x => x.ProjectName == "project2").ShouldEqual(0);
                _result.Count(x => x.ProjectName == "project3").ShouldEqual(1);
            }
        }
    }
}