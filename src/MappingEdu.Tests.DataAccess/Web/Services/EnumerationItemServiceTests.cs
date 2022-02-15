// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Service.Model.EnumerationItem;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class EnumerationItemServiceTests
    {
        private const string systemName = "System Name";
        private const string systemVersion = "1.0.0";
        private const string domainName = "Domain Name";
        private const string domainDefinition = "Domain Definition";
        private const string domainUrl = "http://domain.url";
        private const string entityName = "Entity Name";
        private const string entityDefinition = "Entity Definition";
        private const string elementName = "Element Name";
        private const string elementDefinition = "Element Definition";

        [TestFixture]
        public class When_saving_a_new_system_enumeration_item : EmptyDatabaseTestBase
        {
            private const string codeValue = "ENU";
            private const string descripiton = "Enumeration Description";
            private const string shortDescription = "Short Description";
            private EnumerationItemViewModel _result;
            private Guid _mappedSystemId;
            private Guid _elementSystemItemId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);
                _mappedSystemId = mappedSystem.MappedSystemId;

                var domain = CreateDomain(dbContext, mappedSystem, domainName, domainDefinition, domainUrl);

                var entity = CreateEntity(dbContext, domain, entityName, entityDefinition);

                var element = CreateElement(dbContext, entity, elementName, elementDefinition);
                _elementSystemItemId = element.SystemItemId;

                var enumerationCreateModel = new EnumerationItemCreateModel
                {
                    CodeValue = codeValue,
                    Description = descripiton,
                    ShortDescription = shortDescription
                };

                var enumerationItemService = GetInstance<IEnumerationItemService>();
                _result = enumerationItemService.Post(_elementSystemItemId, enumerationCreateModel);
            }

            [Test]
            public void Should_create_enumeration_item()
            {
                var dbContext = CreateDbContext();

                var enumeration = dbContext.SystemEnumerationItems.First(x => x.SystemEnumerationItemId == _result.SystemEnumerationItemId);
                enumeration.ShouldNotBeNull();
                enumeration.SystemEnumerationItemId.ShouldNotEqual(Guid.Empty);
                enumeration.SystemItemId.ShouldEqual(_elementSystemItemId);
                enumeration.CodeValue.ShouldEqual(codeValue);
                enumeration.Description.ShouldEqual(descripiton);
                enumeration.ShortDescription.ShouldEqual(shortDescription);
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _result.ShouldNotBeNull();
                _result.SystemEnumerationItemId.ShouldNotEqual(Guid.Empty);

                _result.CodeValue.ShouldEqual(codeValue);
                _result.Description.ShouldEqual(descripiton);
                _result.ShortDescription.ShouldEqual(shortDescription);
            }
        }

        [TestFixture]
        public class When_setting_a_system_enumeration_item : EmptyDatabaseTestBase
        {
            private const string codeValue = "ENU";
            private const string descripiton = "Enumeration Description";
            private const string shortDescription = "Short Description";
            private const string newCodeValue = "NEW";
            private const string newDescripiton = "Updated enumeration description";
            private const string newShortDescription = "Updated short description";
            private EnumerationItemViewModel _result;
            private Guid _systemItemId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);
                var domain = CreateDomain(dbContext, mappedSystem, domainName, domainDefinition, domainUrl);

                var entity = CreateEntity(dbContext, domain, entityName, entityDefinition);

                var element = CreateElement(dbContext, entity, elementName, elementDefinition);
                _systemItemId = element.SystemItemId;

                var enumerationItem = CreateEnumerationItem(dbContext, element, codeValue, descripiton, shortDescription);

                var enumerationEditModel = new EnumerationItemEditModel
                {
                    CodeValue = newCodeValue,
                    Description = newDescripiton,
                    ShortDescription = newShortDescription
                };

                var enumerationItemService = GetInstance<IEnumerationItemService>();
                _result = enumerationItemService.Put(enumerationItem.SystemItemId, enumerationItem.SystemEnumerationItemId, enumerationEditModel);
            }

            [Test]
            public void Should_return_new_view_model()
            {
                _result.ShouldNotBeNull();
                _result.SystemEnumerationItemId.ShouldNotEqual(Guid.Empty);

                _result.CodeValue.ShouldEqual(newCodeValue);
                _result.Description.ShouldEqual(newDescripiton);
                _result.ShortDescription.ShouldEqual(newShortDescription);
            }

            [Test]
            public void Should_update_enumeration_item()
            {
                var dbContext = CreateDbContext();

                var enumeration = dbContext.SystemEnumerationItems.First(x => x.SystemEnumerationItemId == _result.SystemEnumerationItemId);
                enumeration.ShouldNotBeNull();
                enumeration.SystemEnumerationItemId.ShouldNotEqual(Guid.Empty);
                enumeration.SystemItemId.ShouldEqual(_systemItemId);
                enumeration.CodeValue.ShouldEqual(newCodeValue);
                enumeration.Description.ShouldEqual(newDescripiton);
                enumeration.ShortDescription.ShouldEqual(newShortDescription);
            }
        }

        [TestFixture]
        public class When_deleting_a_system_enumeration_item : EmptyDatabaseTestBase
        {
            private const string codeValue = "ENU";
            private const string descripiton = "Enumeration Description";
            private const string shortDescription = "Short Description";
            private Guid _elementSystemItemId;
            private Guid _systemEnumerationItemId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, systemName, systemVersion);

                var domain = CreateDomain(dbContext, mappedSystem, domainName, domainDefinition, domainUrl);

                var entity = CreateEntity(dbContext, domain, entityName, entityDefinition);

                var element = CreateElement(dbContext, entity, elementName, elementDefinition);
                _elementSystemItemId = element.SystemItemId;

                var enumerationSystemItem = CreateEnumerationItem(dbContext, element, codeValue, descripiton, shortDescription);
                _systemEnumerationItemId = enumerationSystemItem.SystemEnumerationItemId;

                var enumerationItemService = GetInstance<IEnumerationItemService>();
                enumerationItemService.Delete(_elementSystemItemId, _systemEnumerationItemId);
            }

            [Test]
            public void Should_delete_system_enumeration_item()
            {
                var dbContext = CreateDbContext();
                dbContext.SystemEnumerationItems.Find(_systemEnumerationItemId).ShouldBeNull();
                dbContext.SystemItems.Find(_elementSystemItemId).SystemEnumerationItems.Count.ShouldEqual(0);
            }
        }
    }
}