// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.Mapping;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Tests.Business.Core.Services.Mapping
{
    public class BusinessLogicParserTests
    {
        [TestFixture]
        public class When_updating_a_system_item_map_with_invalid_business_logic_domain : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private const string SystemName = "SystemName";
            private const string SystemVersion = "1.1.1";
            private const string DomainName = @"DomainName!@#$%^&*()`~-_=+{}\|:'"",<>/?";

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                MappedSystem mappedSystem = New.MappedSystem.WithSystemName(SystemName).WithSystemVersion(SystemVersion);
                SystemItem systemItem = New.SystemItem.WithMappedSystem(mappedSystem).WithName("Item").WithType(ItemType.Domain);
                var businessLogic = string.Format("Business Logic [{0}.Entity.Element]", DomainName);
                var businessLogicParser = new BusinessLogicParser(systemItemRepository);
                systemItemRepository.Stub(sir => sir.GetAllItems()).Return(new[] {systemItem}.AsQueryable());

                try
                {
                    businessLogicParser.ParseReferencedSystemItems(businessLogic, false, mappedSystem);
                }
                catch (Exception ex)
                {
                    _exceptionThrown = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("{0} not found as Domain for {1} {2}.", DomainName, SystemName, SystemVersion));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }

        [TestFixture]
        public class When_updating_a_system_item_map_with_invalid_business_logic_element : TestBase
        {
            private bool _exceptionThrown;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();
            private const string SystemName = "SystemName";
            private const string DomainName = @"DomainName!@#$%^&*()`~-_=+{}\|:'"",<>/?";
            private const string EntityName = @"EntityName!@#$%^&*()`~-_=+{}\|:'"",<>/?";
            private const string ElementName = @"ElementName!@#$%^&*()`~-_=+{}\|:'"",<>/?";

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                MappedSystem mappedSystem = New.MappedSystem.WithSystemName(SystemName);
                SystemItem domain = New.SystemItem.AsDomain.WithId(_systemItemId).WithName(DomainName).WithMappedSystem(mappedSystem);
                SystemItem entity = New.SystemItem.AsEntity.WithName(EntityName).WithParentSystemItem(domain).WithMappedSystem(mappedSystem);

                var businessLogic = string.Format("Business Logic [{0}.{1}.{2}]", DomainName, EntityName, ElementName);

                var businessLogicParser = new BusinessLogicParser(systemItemRepository);
                systemItemRepository.Stub(sir => sir.GetAllItems()).Return(new[] {domain, entity}.AsQueryable());

                try
                {
                    businessLogicParser.ParseReferencedSystemItems(businessLogic, false, mappedSystem);
                }
                catch (Exception ex)
                {
                    _exceptionThrown = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_give_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("{0} not found as child of {1}.", ElementName, EntityName));
            }

            [Test]
            public void Should_throw_exception()
            {
                _exceptionThrown.ShouldBeTrue();
                _exception.ShouldNotBeNull();
                _exception.ShouldBeType<Exception>();
            }
        }
    }
}
