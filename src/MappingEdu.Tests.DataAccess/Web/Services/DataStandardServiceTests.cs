// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class DataStandardServiceTests
    {
        private const string SystemName = "Test System";
        private const string SystemVersion = "v 1.0";

        [TestFixture]
        public class When_creating_data_standard : EmptyDatabaseTestBase
        {
            private DataStandardViewModel _result;
            private MappedSystem _previousMappedSystem;
            private const string OtherSystemName = "Other Test System";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                _previousMappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);

                var dataStandardCreateModel = new DataStandardCreateModel
                {
                    SystemName = OtherSystemName,
                    SystemVersion = SystemVersion,
                    PreviousDataStandardId = _previousMappedSystem.MappedSystemId
                };

                var mappedSystemService = GetInstance<IDataStandardService>();
                _result = mappedSystemService.Post(dataStandardCreateModel);
            }

            [Test]
            public void Should_create_data_standard()
            {
                var dbContext = CreateDbContext();
                var mappedSystem = dbContext.MappedSystems.Single(data => data.MappedSystemId == _result.DataStandardId);
                mappedSystem.ShouldNotBeNull();
                mappedSystem.MappedSystemId.ShouldNotEqual(Guid.Empty);
                mappedSystem.SystemName.ShouldEqual(OtherSystemName);
                mappedSystem.SystemVersion.ShouldEqual(SystemVersion);
                mappedSystem.PreviousMappedSystemId.ShouldEqual(_previousMappedSystem.MappedSystemId);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _result.ShouldNotBeNull();
                _result.DataStandardId.ShouldNotEqual(Guid.Empty);
                _result.SystemName.ShouldEqual(OtherSystemName);
                _result.SystemVersion.ShouldEqual(SystemVersion);
                _result.PreviousDataStandardId.ShouldEqual(_previousMappedSystem.MappedSystemId);
            }
        }

        [TestFixture]
        public class When_editing_data_standard : EmptyDatabaseTestBase
        {
            private Guid _mappedSystemId;
            private DataStandardViewModel _result;
            private const string ChangedSystemName = "Changed Test System";
            private const string ChangedSystemVersion = "v 1.5";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                _mappedSystemId = mappedSystem.MappedSystemId;

                var mappedSystemEditModel = new DataStandardEditModel
                {
                    SystemName = ChangedSystemName,
                    SystemVersion = ChangedSystemVersion
                };

                var mappedSystemService = GetInstance<IDataStandardService>();
                _result = mappedSystemService.Put(_mappedSystemId, mappedSystemEditModel);
            }

            [Test]
            public void Should_return_updated_view_model()
            {
                _result.ShouldNotBeNull();
                _result.DataStandardId.ShouldNotEqual(Guid.Empty);
                _result.SystemName.ShouldEqual(ChangedSystemName);
                _result.SystemVersion.ShouldEqual(ChangedSystemVersion);
            }

            [Test]
            public void Should_update_data_standard()
            {
                var dbContext = CreateDbContext();
                var mappedSystem = dbContext.MappedSystems.Single(data => data.MappedSystemId == _mappedSystemId);
                mappedSystem.ShouldNotBeNull();
                mappedSystem.MappedSystemId.ShouldNotEqual(Guid.Empty);
                mappedSystem.SystemName.ShouldEqual(ChangedSystemName);
                mappedSystem.SystemVersion.ShouldEqual(ChangedSystemVersion);
            }
        }

        [TestFixture]
        public class When_deleting_data_standard : EmptyDatabaseTestBase
        {
            private Guid _mappedSystemId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, SystemName, SystemVersion);
                _mappedSystemId = mappedSystem.MappedSystemId;

                var mappedSystemService = GetInstance<IDataStandardService>();
                mappedSystemService.Delete(_mappedSystemId);
            }

            [Test]
            public void Should_mark_data_standard_as_inactive()
            {
                var dbContext = CreateDbContext();

                var mappedSystem = dbContext.MappedSystems.Single(data => data.MappedSystemId == _mappedSystemId);
                mappedSystem.ShouldNotBeNull();
                mappedSystem.IsActive.ShouldBeFalse();
            }
        }
    }
}