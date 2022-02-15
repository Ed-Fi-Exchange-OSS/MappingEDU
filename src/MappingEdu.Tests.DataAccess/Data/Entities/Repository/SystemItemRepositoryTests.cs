// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Security.Claims;
using System.Threading;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Data.Entities.Repository
{
    public class SystemItemRepositoryTests
    {
        public class When_getting_all_system_items : EmptyDatabaseTestBase
        {
            private SystemItem[] _result;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "system1", "1");
                CreateDomain(dbContext, mappedSystem, "domain1", string.Empty, string.Empty);
                CreateDomain(dbContext, mappedSystem, "domain2", string.Empty, string.Empty, false);
                CreateDomain(dbContext, mappedSystem, "domain3", string.Empty, string.Empty);
                CreateUser(dbContext, "1", "test", "user", "test@dataaccess.com");

                // TODO: Create a global approach to security in unit tests (cpt)
                var principal = new ClaimsPrincipal(identityFactory.CreateIdentity("TESTS", "test", "1", true));
                Thread.CurrentPrincipal = principal;

                var repository = GetInstance<ISystemItemRepository>();
                _result = repository.GetAll();
            }

            [Test]
            public void Should_return_active_system_items()
            {
                _result.ShouldNotBeNull();
                _result.Count().ShouldEqual(2);
                _result.Count(x => x.ItemName == "domain1").ShouldEqual(1);
                _result.Count(x => x.ItemName == "domain2").ShouldEqual(0);
                _result.Count(x => x.ItemName == "domain3").ShouldEqual(1);
            }
        }

        public class When_getting_system_items_from_specific_mapped_system_and_parent : EmptyDatabaseTestBase
        {
            private SystemItem[] _result;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "system1", "1");
                CreateDomain(dbContext, mappedSystem, "domain1", string.Empty, string.Empty);
                CreateDomain(dbContext, mappedSystem, "domain2", string.Empty, string.Empty, false);
                var domain3 = CreateDomain(dbContext, mappedSystem, "domain3", string.Empty, string.Empty);
                CreateEntity(dbContext, domain3, "entity", string.Empty);

                var repository = GetInstance<ISystemItemRepository>();
                _result = repository.GetWhere(mappedSystem.MappedSystemId, null);
            }

            [Test]
            public void Should_return_active_system_items()
            {
                _result.ShouldNotBeNull();
                _result.Count().ShouldEqual(2);
                _result.Count(x => x.ItemName == "domain1").ShouldEqual(1);
                _result.Count(x => x.ItemName == "domain2").ShouldEqual(0);
                _result.Count(x => x.ItemName == "domain3").ShouldEqual(1);
                _result.Count(x => x.ItemName == "entity").ShouldEqual(0);
            }
        }
    }
}