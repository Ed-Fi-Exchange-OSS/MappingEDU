// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.CustomMigration;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.CustomMigration
{
    public class CustomMigrationServiceTests
    {
        [TestFixture]
        public class When_no_custom_migrations_have_been_applied : TestBase
        {
            private CustomMigrationService _service;

            [OneTimeSetUp]
            public void Setup()
            {
                var respository = GenerateStub<IRepository<MappingEdu.Core.Domain.CustomMigration>>();
                respository.Stub(x => x.GetAll()).Return(new MappingEdu.Core.Domain.CustomMigration[0]);

                var migrations = new ICustomMigration[] {new Test1CustomMigration(), new Test2CustomMigration(), new Test3CustomMigration()};

                _service = new CustomMigrationService(respository, migrations);
            }

            [Test]
            public void Database_should_not_be_up_to_date()
            {
                _service.DatabaseIsUpToDate.ShouldBeFalse();
            }

            [Test]
            public void Should_have_no_applied_migrations()
            {
                _service.AppliedMigrations.Any().ShouldBeFalse();
            }

            [Test]
            public void Should_return_all_local_migrations()
            {
                var localMigrations = _service.LocalMigrations;
                localMigrations.Count().ShouldEqual(3);
                localMigrations.ShouldContain("Test1");
                localMigrations.ShouldContain("Test2");
                localMigrations.ShouldContain("Test3");
            }

            [Test]
            public void Should_return_all_migrations_as_pending()
            {
                var pendingMigrations = _service.PendingMigrations;
                pendingMigrations.Count().ShouldEqual(3);
                pendingMigrations.ShouldContain("Test1");
                pendingMigrations.ShouldContain("Test2");
                pendingMigrations.ShouldContain("Test3");
            }
        }

        [TestFixture]
        public class When_some_custom_migrations_have_been_applied : TestBase
        {
            private CustomMigrationService _service;

            [OneTimeSetUp]
            public void Setup()
            {
                var respository = GenerateStub<IRepository<MappingEdu.Core.Domain.CustomMigration>>();
                respository.Stub(x => x.GetAll()).Return(new[] {new MappingEdu.Core.Domain.CustomMigration {Name = "Test2"}, new MappingEdu.Core.Domain.CustomMigration {Name = "NoLongerExists"}});

                var migrations = new ICustomMigration[] {new Test1CustomMigration(), new Test2CustomMigration(), new Test3CustomMigration()};

                _service = new CustomMigrationService(respository, migrations);
            }

            [Test]
            public void Database_should_not_be_up_to_date()
            {
                _service.DatabaseIsUpToDate.ShouldBeFalse();
            }

            [Test]
            public void Should_have_some_applied_migrations()
            {
                var appliedMigrations = _service.AppliedMigrations;
                appliedMigrations.Count().ShouldEqual(2);
                appliedMigrations.ShouldContain("Test2");
                appliedMigrations.ShouldContain("NoLongerExists");
            }

            [Test]
            public void Should_return_all_local_migrations()
            {
                var localMigrations = _service.LocalMigrations;
                localMigrations.Count().ShouldEqual(3);
                localMigrations.ShouldContain("Test1");
                localMigrations.ShouldContain("Test2");
                localMigrations.ShouldContain("Test3");
            }

            [Test]
            public void Should_return_some_migrations_as_pending()
            {
                var pendingMigrations = _service.PendingMigrations;
                pendingMigrations.Count().ShouldEqual(2);
                pendingMigrations.ShouldContain("Test1");
                pendingMigrations.ShouldContain("Test3");
            }
        }

        [TestFixture]
        public class When_all_custom_migrations_have_been_applied : TestBase
        {
            private CustomMigrationService _service;

            [OneTimeSetUp]
            public void Setup()
            {
                var respository = GenerateStub<IRepository<MappingEdu.Core.Domain.CustomMigration>>();
                respository.Stub(x => x.GetAll()).Return(new[]
                {
                    new MappingEdu.Core.Domain.CustomMigration {Name = "Test2"},
                    new MappingEdu.Core.Domain.CustomMigration {Name = "Test1"},
                    new MappingEdu.Core.Domain.CustomMigration {Name = "Test3"}
                });

                var migrations = new ICustomMigration[] {new Test1CustomMigration(), new Test2CustomMigration(), new Test3CustomMigration()};

                _service = new CustomMigrationService(respository, migrations);
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
                appliedMigrations.ShouldContain("Test1");
                appliedMigrations.ShouldContain("Test2");
                appliedMigrations.ShouldContain("Test3");
            }

            [Test]
            public void Should_return_all_local_migrations()
            {
                var localMigrations = _service.LocalMigrations;
                localMigrations.Count().ShouldEqual(3);
                localMigrations.ShouldContain("Test1");
                localMigrations.ShouldContain("Test2");
                localMigrations.ShouldContain("Test3");
            }

            [Test]
            public void Should_return_no_migrations_as_pending()
            {
                var pendingMigrations = _service.PendingMigrations;
                pendingMigrations.Count().ShouldEqual(0);
            }
        }

        private class Test1CustomMigration : ICustomMigration
        {
            public string Name
            {
                get { return "Test1"; }
            }

            public void Apply()
            {
            }
        }

        private class Test2CustomMigration : ICustomMigration
        {
            public string Name
            {
                get { return "Test2"; }
            }

            public void Apply()
            {
            }
        }

        private class Test3CustomMigration : ICustomMigration
        {
            public string Name
            {
                get { return "Test3"; }
            }

            public void Apply()
            {
            }
        }
    }
}