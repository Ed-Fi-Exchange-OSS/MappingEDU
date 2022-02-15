// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Security.Claims;
using System.Threading;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Data.Entities.Repository
{
    public class MappedSystemRepositoryTests
    {
        public class When_getting_all_mapped_systems : EmptyDatabaseTestBase
        {
            private MappedSystem[] _result;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                CreateMappedSystem(dbContext, "system1", "1");
                CreateMappedSystem(dbContext, "system2", "2", false);
                CreateMappedSystem(dbContext, "system3", "3");

                var repository = GetInstance<IMappedSystemRepository>();
                _result = repository.GetAll();
            }

            [Test]
            public void Should_return_active_mapped_systems()
            {
                _result.ShouldNotBeNull();
                _result.Count().ShouldEqual(2);
                _result.Count(x => x.SystemName == "system1").ShouldEqual(1);
                _result.Count(x => x.SystemName == "system2").ShouldEqual(0);
                _result.Count(x => x.SystemName == "system3").ShouldEqual(1);
            }
        }
    }
}