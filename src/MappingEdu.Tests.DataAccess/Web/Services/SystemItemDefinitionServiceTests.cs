// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Service.Model.SystemItemDefinition;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class SystemItemDefinitionServiceTests
    {
        [TestFixture]
        public class When_saving_system_item_definition : EmptyDatabaseTestBase
        {
            private Guid _systemItemId;
            private const string ExpectedDefinition = "New Definition";
            private SystemItemDefinitionViewModel _viewModel;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "Test", "Test");
                var domain = CreateDomain(dbContext, mappedSystem, "domain", string.Empty, string.Empty);
                var systemItem = CreateElement(dbContext, domain, "System Item", "old description");
                _systemItemId = systemItem.SystemItemId;

                var entityEditModel = new SystemItemDefinitionEditModel
                {
                    SystemItemId = _systemItemId,
                    Definition = ExpectedDefinition
                };

                var entityNameService = GetInstance<ISystemItemDefinitionService>();
                _viewModel = entityNameService.Put(entityEditModel);
            }

            [Test]
            public void Should_get_system_item_description()
            {
                _viewModel.Definition.ShouldEqual(ExpectedDefinition);
            }

            [Test]
            public void Should_save_system_item_description()
            {
                var dbContext = CreateDbContext();
                var systemItem = dbContext.SystemItems.Single(x => x.SystemItemId == _systemItemId);
                systemItem.Definition.ShouldEqual(ExpectedDefinition);
            }
        }
    }
}