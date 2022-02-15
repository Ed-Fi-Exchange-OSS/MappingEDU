// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.Services.CustomMigration;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Core.Services.CustomMigration
{
    public class CustomMigrationServiceTests
    {
        [TestFixture]
        public class When_no_custom_migrations_have_been_applied : EmptyDatabaseTestBase
        {
            private CustomMigrationService _service;

            protected override void EstablishContext()
            {
                _appliedMigrations = new List<string>();
                _container.EjectAllInstancesOf<ICustomMigration>();
                _container.Inject<ICustomMigration>(new Test1CustomMigration());
                _container.Inject<ICustomMigration>(new Test2CustomMigration());
                _container.Inject<ICustomMigration>(new Test3CustomMigration());

                _service = GetInstance<CustomMigrationService>();
                _service.ApplyPendingMigrations();
            }

            [Test]
            public void Should_apply_all_migrations()
            {
                _appliedMigrations.Count.ShouldEqual(3);
                _appliedMigrations.ElementAt(0).ShouldEqual("2_Test2");
                _appliedMigrations.ElementAt(1).ShouldEqual("4_Test3");
                _appliedMigrations.ElementAt(2).ShouldEqual("5_Test1");
            }

            [Test]
            public void Should_create_custom_migration_entities()
            {
                var dbContext = CreateDbContext();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "2_Test2").ShouldNotBeNull();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "4_Test3").ShouldNotBeNull();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "5_Test1").ShouldNotBeNull();
            }

            [Test]
            public void Database_should_be_up_to_date()
            {
                _service.DatabaseIsUpToDate.ShouldBeTrue();
            }

            [Test]
            public void Should_have_all_applied_migrations()
            {
                var appliedMigrations = _service.AppliedMigrations;
                appliedMigrations.Count().ShouldEqual(3);
                appliedMigrations.ShouldContain("2_Test2");
                appliedMigrations.ShouldContain("4_Test3");
                appliedMigrations.ShouldContain("5_Test1");
            }

            [Test]
            public void Should_return_no_migrations_as_pending()
            {
                _service.PendingMigrations.Any().ShouldBeFalse();
            }

            [Test]
            public void Should_return_all_local_migrations()
            {
                var localMigrations = _service.LocalMigrations;
                localMigrations.Count().ShouldEqual(3);
                localMigrations.ShouldContain("2_Test2");
                localMigrations.ShouldContain("4_Test3");
                localMigrations.ShouldContain("5_Test1");
            }
        }

        [TestFixture]
        public class When_some_custom_migrations_have_been_applied : EmptyDatabaseTestBase
        {
            private CustomMigrationService _service;

            protected override void EstablishContext()
            {
                _appliedMigrations = new List<string>();
                _container.EjectAllInstancesOf<ICustomMigration>();
                _container.Inject<ICustomMigration>(new Test1CustomMigration());
                _container.Inject<ICustomMigration>(new Test2CustomMigration());
                _container.Inject<ICustomMigration>(new Test3CustomMigration());

                var dbContext = CreateDbContext();
                CreateCustomMigration(dbContext, "4_Test3");

                _service = GetInstance<CustomMigrationService>();
                _service.ApplyPendingMigrations();
            }

            [Test]
            public void Should_apply_needed_migrations()
            {
                _appliedMigrations.Count.ShouldEqual(2);
                _appliedMigrations.ElementAt(0).ShouldEqual("2_Test2");
                _appliedMigrations.ElementAt(1).ShouldEqual("5_Test1");
            }

            [Test]
            public void Should_create_custom_migration_entities()
            {
                var dbContext = CreateDbContext();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "2_Test2").ShouldNotBeNull();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "5_Test1").ShouldNotBeNull();
            }

            [Test]
            public void Database_should_be_up_to_date()
            {
                _service.DatabaseIsUpToDate.ShouldBeTrue();
            }

            [Test]
            public void Should_have_all_applied_migrations()
            {
                var appliedMigrations = _service.AppliedMigrations;
                appliedMigrations.Count().ShouldEqual(3);
                appliedMigrations.ShouldContain("2_Test2");
                appliedMigrations.ShouldContain("4_Test3");
                appliedMigrations.ShouldContain("5_Test1");
            }

            [Test]
            public void Should_return_no_migrations_as_pending()
            {
                _service.PendingMigrations.Any().ShouldBeFalse();
            }

            [Test]
            public void Should_return_all_local_migrations()
            {
                var localMigrations = _service.LocalMigrations;
                localMigrations.Count().ShouldEqual(3);
                localMigrations.ShouldContain("2_Test2");
                localMigrations.ShouldContain("4_Test3");
                localMigrations.ShouldContain("5_Test1");
            }
        }

        [TestFixture]
        public class When_all_custom_migrations_have_been_applied : EmptyDatabaseTestBase
        {
            private CustomMigrationService _service;

            protected override void EstablishContext()
            {
                _appliedMigrations = new List<string>();
                _container.EjectAllInstancesOf<ICustomMigration>();
                _container.Inject<ICustomMigration>(new Test1CustomMigration());
                _container.Inject<ICustomMigration>(new Test2CustomMigration());
                _container.Inject<ICustomMigration>(new Test3CustomMigration());
                
                var dbContext = CreateDbContext();
                CreateCustomMigration(dbContext, "2_Test2");
                CreateCustomMigration(dbContext, "4_Test3");
                CreateCustomMigration(dbContext, "5_Test1");

                _service = GetInstance<CustomMigrationService>();
                _service.ApplyPendingMigrations();
            }

            [Test]
            public void Should_not_apply_any_migrations()
            {
                _appliedMigrations.Count.ShouldEqual(0);
            }

            [Test]
            public void Database_should_be_up_to_date()
            {
                _service.DatabaseIsUpToDate.ShouldBeTrue();
            }

            [Test]
            public void Should_have_all_applied_migrations()
            {
                var appliedMigrations = _service.AppliedMigrations;
                appliedMigrations.Count().ShouldEqual(3);
                appliedMigrations.ShouldContain("2_Test2");
                appliedMigrations.ShouldContain("4_Test3");
                appliedMigrations.ShouldContain("5_Test1");
            }

            [Test]
            public void Should_return_no_migrations_as_pending()
            {
                _service.PendingMigrations.Any().ShouldBeFalse();
            }

            [Test]
            public void Should_return_all_local_migrations()
            {
                var localMigrations = _service.LocalMigrations;
                localMigrations.Count().ShouldEqual(3);
                localMigrations.ShouldContain("2_Test2");
                localMigrations.ShouldContain("4_Test3");
                localMigrations.ShouldContain("5_Test1");
            }
        }

        [TestFixture]
        public class When_a_custom_migration_throws_an_exception : EmptyDatabaseTestBase
        {
            private bool _exceptionThrown;
            private CustomMigrationService _service;

            protected override void EstablishContext()
            {
                _appliedMigrations = new List<string>();
                _container.EjectAllInstancesOf<ICustomMigration>();
                _container.Inject<ICustomMigration>(new Test1CustomMigration());
                _container.Inject<ICustomMigration>(new TestExceptionCustomMigration());
                _container.Inject<ICustomMigration>(new Test3CustomMigration());
                _container.Inject<ICustomMigration>(new Test2CustomMigration());

                _service = GetInstance<CustomMigrationService>();
                try
                {
                    _service.ApplyPendingMigrations();
                }
                catch (ApplicationException)
                {
                    _exceptionThrown = true;
                }
            }

            [Test]
            public void Should_apply_migrations()
            {
                _appliedMigrations.Count.ShouldEqual(4);
            }

            [Test]
            public void Should_create_custom_migration_entities_for_migrations_that_completed()
            {
                var dbContext = CreateDbContext();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "2_Test2").ShouldNotBeNull();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "4_Test3").ShouldNotBeNull();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "5_Test1").ShouldNotBeNull();
            }

            [Test]
            public void Should_not_create_custom_migration_entities_for_migrations_that_failed()
            {
                var dbContext = CreateDbContext();
                dbContext.CustomMigrations.SingleOrDefault(x => x.Name == "7_TestException").ShouldBeNull();
            }

            [Test]
            public void Database_should_not_be_up_to_date()
            {
                _service.DatabaseIsUpToDate.ShouldBeFalse();
            }

            [Test]
            public void Exception_should_be_thrown()
            {
                _exceptionThrown.ShouldBeTrue();
            }
        }
        
        private static List<string> _appliedMigrations;

        private class Test1CustomMigration : ICustomMigration
        {
            public string Name { get { return "5_Test1"; } }
            public void Apply() { _appliedMigrations.Add(Name); }
        }

        private class Test2CustomMigration : ICustomMigration
        {
            public string Name { get { return "2_Test2"; } }
            public void Apply() { _appliedMigrations.Add(Name); }
        }

        private class Test3CustomMigration : ICustomMigration
        {
            public string Name { get { return "4_Test3"; } }
            public void Apply() { _appliedMigrations.Add(Name); }
        }

        private class TestExceptionCustomMigration : ICustomMigration
        {
            public string Name { get { return "7_TestException"; } }
            public void Apply()
            {
                _appliedMigrations.Add(Name);
                throw new ApplicationException("TestExceptionCustomMigration");
            }
        }
    }
}

