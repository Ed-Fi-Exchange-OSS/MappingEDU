// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Service.Model.SystemItemTree;
using MappingEdu.Service.SystemItemTree;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class SystemItemTreeServiceTests
    {
        public class When_getting_all_system_items_for_a_domain : EmptyDatabaseTestBase
        {
            private SystemItemTypeViewModel[] _result;

            protected override void EstablishContext()
            {
                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "mappedsystem", "1");

                var domain = CreateDomain(dbContext, mappedSystem, "Domain Item");

                var topLevelEntity1 = CreateEntity(dbContext, domain, "Top Level Entity 1");
                var subEntity1 = CreateSubEntity(dbContext, topLevelEntity1, "Sub Level Entity 1");
                CreateSubEntity(dbContext, subEntity1, "Sub Sub Level Entity 1");
                CreateSubEntity(dbContext, topLevelEntity1, "Sub Level Entity 2");
                CreateEntity(dbContext, domain, "Top Level Entity 2");
                CreateEnumeration(dbContext, domain, "Enumeration 1");
                CreateEnumeration(dbContext, domain, "Enumeration 2");

                var treeService = GetInstance<ISystemItemTreeService>();
                _result = treeService.Get(domain.SystemItemId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                // validate the top level item values
                _result.ShouldNotBeNull();
                _result.Length.ShouldEqual(2);

                // validate the entities section
                _result[0].ItemTypeName.ShouldEqual("Entities");
                _result[0].Children.Length.ShouldEqual(2);
                _result[0].Children[0].Children.Any().ShouldBeTrue();
                _result[0].Children[1].Children.Any().ShouldBeFalse();

                // validate the enumerations sections
                _result[1].ItemTypeName.ShouldEqual("Enumerations");
                _result[1].Children.Length.ShouldEqual(2);
                _result[1].Children[0].Children.Any().ShouldBeFalse();
                _result[1].Children[1].Children.Any().ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_getting_specific_system_item : EmptyDatabaseTestBase
        {
            private SystemItemTreeViewModel _result;

            protected override void EstablishContext()
            {
                var dbContext = CreateDbContext();

                var mappedSystem = CreateMappedSystem(dbContext, "mappedsystem", "1");

                var domain = CreateDomain(dbContext, mappedSystem, "Domain Item");

                var topLevelEntity1 = CreateEntity(dbContext, domain, "Top Level Entity 1");
                var subEntity1 = CreateSubEntity(dbContext, topLevelEntity1, "Sub Level Entity 1");
                CreateSubEntity(dbContext, subEntity1, "Sub Sub Level Entity 1");

                var treeService = GetInstance<ISystemItemTreeService>();
                _result = treeService.Get(domain.SystemItemId, topLevelEntity1.SystemItemId);
            }

            [Test]
            public void Should_populate_view_model()
            {
                _result.ShouldNotBeNull();
                _result.Children.Length.ShouldEqual(1);

                _result.Children[0].Children.Any().ShouldBeTrue();
            }
        }
    }
}