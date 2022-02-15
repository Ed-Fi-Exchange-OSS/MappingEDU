// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class ElementListServiceTests
    {
        public class When_getting_an_element_list : TestBase
        {
            private Guid _mappedSystemId;
            private ElementListViewModel _result;
            private SystemItem[] _systemItems;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystemId = Guid.NewGuid();

                var mappedSystem = New.MappedSystem;

                _systemItems = new SystemItem[]
                {
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element").WithType(ItemType.Element).WithDefinition("I am an element"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Enumeration").WithType(ItemType.Enumeration).WithDefinition("I am an enumeration"),
                    New.SystemItem.WithId(Guid.NewGuid()).WithMappedSystem(mappedSystem).WithName("Element Group").WithType(ItemType.Domain).WithDefinition("I am an element group")
                };

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Expect(x => x.GetAllItems()).Return(_systemItems.AsQueryable());

                IElementListService service = new ElementListService(systemItemRepository, null, null);
                _result = service.Get(_mappedSystemId);
            }

            [Test]
            public void ShouldPopulateViewModel()
            {
                _result.ShouldNotBeNull();
            }

            [Test]
            public void ShouldOnlyReturnElementsAndEnumerations()
            {
                Assert.That(!_result.Elements.Any(i => !new[] {"Enumeration", "Element"}.Contains(i.Element.ItemTypeName)));
            }
        }
    }
}