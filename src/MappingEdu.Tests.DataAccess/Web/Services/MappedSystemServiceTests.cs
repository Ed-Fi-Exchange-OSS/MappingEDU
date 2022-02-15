// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.MappedSystem;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class MappedSystemServiceTests
    {
        [TestFixture]
        public class When_creating_mapped_system : EmptyDatabaseTestBase
        {
            private const string _systemName = "Test System";
            private const string _systemVersion = "v 1.0";

            private Guid _mappedSystemId;
            private MappedSystemViewModel _result;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var systemCreateModel = new MappedSystemCreateModel
                {
                    MappedSystemDetails = new MappedSystemCreateModel.Details
                    {
                        SystemName = _systemName,
                        SystemVersion = _systemVersion
                    }
                };

                var mappedSystemService = GetInstance<IMappedSystemService>();
                _result = mappedSystemService.Post(systemCreateModel);
                _mappedSystemId = _result.MappedSystemId;
            }

            [Test]
            public void Should_create_mapped_system()
            {
                var dbContext = CreateDbContext();
                var mappedSystem = dbContext.MappedSystems.Single(data => data.MappedSystemId == _mappedSystemId);
                mappedSystem.ShouldNotBeNull();
                mappedSystem.SystemName.ShouldEqual(_systemName);
                mappedSystem.SystemVersion.ShouldEqual(_systemVersion);
                mappedSystem.IsActive.ShouldBeTrue();
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _result.ShouldNotBeNull();
                _result.MappedSystemId.ShouldNotEqual(Guid.Empty);
                _result.DataStandard.SystemName.ShouldEqual(_systemName);
                _result.DataStandard.SystemVersion.ShouldEqual(_systemVersion);
            }
        }
    }
}