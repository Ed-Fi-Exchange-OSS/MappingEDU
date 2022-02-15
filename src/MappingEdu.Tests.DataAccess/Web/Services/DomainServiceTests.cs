// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Domains;
using MappingEdu.Service.Model.Domain;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class DomainServiceTests
    {
        private const string domainDefinition = "Domain Definition";
        private const string domainName = "Domain Name";
        private const string domainUrl = "http://domain.url";
        private const string systemName = "System Name";
        private const string systemVersion = "1.0.0";

        [TestFixture]
        public class When_saving_new_domain : EmptyDatabaseTestBase
        {
            private Guid _mappedSystemId;
            private DomainViewModel _result;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);
                _mappedSystemId = mappedSystem.MappedSystemId;

                var domainCreateModel = new DomainCreateModel
                {
                    DataStandardId = _mappedSystemId,
                    Definition = domainDefinition,
                    ItemName = domainName,
                    ItemUrl = domainUrl
                };

                var domainService = GetInstance<IDomainService>();
                _result = domainService.Post(domainCreateModel);
            }

            [Test]
            public void Should_create_domain()
            {
                var dbContext = CreateDbContext();

                var domain = dbContext.SystemItems.FirstOrDefault(x => x.SystemItemId == _result.SystemItemId);
                domain.SystemItemId.ShouldNotEqual(Guid.Empty);

                domain.Definition.ShouldEqual(domainDefinition);
                domain.ItemName.ShouldEqual(domainName);
                domain.ItemUrl.ShouldEqual(domainUrl);
                domain.ItemType.ShouldEqual(ItemType.Domain);
                domain.IsActive.ShouldBeTrue();
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _result.ShouldNotBeNull();
                _result.MappedSystemId.ShouldNotEqual(Guid.Empty);
                _result.SystemItemId.ShouldNotEqual(Guid.Empty);
                _result.Definition.ShouldEqual(domainDefinition);
                _result.ItemName.ShouldEqual(domainName);
                _result.ItemUrl.ShouldEqual(domainUrl);
            }
        }

        [TestFixture]
        public class When_setting_domain : EmptyDatabaseTestBase
        {
            private Guid _mappedSystemId;
            private Guid _systemItemId;
            private DomainViewModel _result;
            private const string newDomainDefinition = "Updated Domain Definition that is longer than the first one.";
            private const string newDomainName = "Updated Domain Name that is also longer then the first.";
            private const string newDomainUrl = "http://domain.url.updated.value";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);
                _mappedSystemId = mappedSystem.MappedSystemId;

                var domain = CreateDomain(dbContext, mappedSystem, domainName, domainDefinition, domainUrl);
                _systemItemId = domain.SystemItemId;

                var domainEditModel = new DomainEditModel
                {
                    SystemItemId = _systemItemId,
                    Definition = newDomainDefinition,
                    ItemName = newDomainName,
                    ItemUrl = newDomainUrl
                };

                var domainService = GetInstance<IDomainService>();

                _result = domainService.Put(_mappedSystemId, domainEditModel);
            }

            [Test]
            public void Should_return_updated_view_model()
            {
                _result.ShouldNotBeNull();
                _result.MappedSystemId.ShouldNotEqual(Guid.Empty);
                _result.SystemItemId.ShouldNotEqual(Guid.Empty);
                _result.Definition.ShouldEqual(newDomainDefinition);
                _result.ItemName.ShouldEqual(newDomainName);
                _result.ItemUrl.ShouldEqual(newDomainUrl);
            }

            [Test]
            public void Should_update_domain()
            {
                var dbContext = CreateDbContext();

                var domain = dbContext.SystemItems.FirstOrDefault(x => x.SystemItemId == _systemItemId);
                domain.SystemItemId.ShouldNotEqual(Guid.Empty);
                domain.SystemItemId.ShouldEqual(_systemItemId);

                domain.Definition.ShouldEqual(newDomainDefinition);
                domain.ItemName.ShouldEqual(newDomainName);
                domain.ItemUrl.ShouldEqual(newDomainUrl);
                domain.ItemType.ShouldEqual(ItemType.Domain);
                domain.IsActive.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_deleting_existing_domain : EmptyDatabaseTestBase
        {
            private Guid _mappedSystemId;
            private Guid _systemItemId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);
                _mappedSystemId = mappedSystem.MappedSystemId;

                var domain = CreateDomain(dbContext, mappedSystem, domainName, domainDefinition, domainUrl);
                _systemItemId = domain.SystemItemId;

                var domainService = GetInstance<IDomainService>();
                domainService.Delete(_mappedSystemId, _systemItemId);
            }

            [Test]
            public void Should_delete_domain()
            {
                var dbContext = CreateDbContext();

                var domain = dbContext.SystemItems.FirstOrDefault(x => x.SystemItemId == _systemItemId);
                domain.SystemItemId.ShouldNotEqual(Guid.Empty);
                domain.SystemItemId.ShouldEqual(_systemItemId);

                domain.ItemType.ShouldEqual(ItemType.Domain);
                domain.IsActive.ShouldBeFalse();
            }
        }
    }
}