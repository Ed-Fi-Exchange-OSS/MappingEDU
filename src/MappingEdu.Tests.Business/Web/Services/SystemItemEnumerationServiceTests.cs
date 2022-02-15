// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemEnumeration;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SystemItemEnumerationServiceTests
    {
        [TestFixture]
        // ReSharper disable once InconsistentNaming
        public class When_getting_System_Item_Enumerations : TestBase
        {
            private readonly Guid _dataStandardId = Guid.NewGuid();
            private readonly Guid _otherDataStandardId = Guid.NewGuid();
            private readonly Guid _systemItemEnumerationId = Guid.NewGuid();
            private const string SystemItemEnumerationName = "Enumeration Name";
            private const string SystemItemEnumerationDefinition = "Enumeration Definition";

            private SystemItemEnumerationViewModel[] results;

            [OneTimeSetUp]
            public void Setup()
            {
                MappedSystem mappedSystem = New.MappedSystem.WithId(_dataStandardId);
                MappedSystem otherMappedSystem = New.MappedSystem.WithId(_otherDataStandardId);

                SystemItem systemItemEnumeration =
                    New.SystemItem.AsEnumeration.WithId(_systemItemEnumerationId)
                        .WithName(SystemItemEnumerationName)
                        .WithParentSystemItem(New.SystemItem.WithName("Domain"))
                        .WithDefinition(SystemItemEnumerationDefinition)
                        .WithMappedSystem(mappedSystem);
                SystemItem otherSystemItemEnumeration =
                    New.SystemItem.AsEnumeration
                        .WithName("Should not be returned because wrong data standard.")
                        .WithMappedSystem(otherMappedSystem);
                SystemItem systemItemElement =
                    New.SystemItem.AsElement
                        .WithName("Should not be returned because not an enumeration.")
                        .WithMappedSystem(mappedSystem);

                var systemItemEnumerations = new List<SystemItem>
                {
                    systemItemEnumeration,
                    otherSystemItemEnumeration,
                    systemItemElement
                }.AsQueryable();

                var systemItemRepository = GenerateStub<ISystemItemRepository>();
                systemItemRepository.Stub(sir => sir.GetAllItems()).Return(systemItemEnumerations);

                ISystemItemEnumerationService systemItemEnumerationService =
                    new SystemItemEnumerationService(systemItemRepository);

                results = systemItemEnumerationService.Get(_dataStandardId);
            }

            [Test]
            public void Should_return_an_array_of_System_Item_Enumeration_View_Models()
            {
                results.ShouldNotBeNull();
                results.Length.ShouldEqual(1);
                results[0].SystemItemId.ShouldEqual(_systemItemEnumerationId);
                results[0].ItemName.ShouldEqual(SystemItemEnumerationName);
                results[0].Definition.ShouldEqual(SystemItemEnumerationDefinition);
            }
        }
    }
}