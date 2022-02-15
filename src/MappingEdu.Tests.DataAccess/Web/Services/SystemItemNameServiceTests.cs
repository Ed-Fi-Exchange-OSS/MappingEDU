// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Service.Model.SystemItemName;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class SystemItemNameServiceTests
    {
        [TestFixture]
        public class When_saving_system_item_name : EmptyDatabaseTestBase
        {
            private SystemItemNameViewModel _viewModel;
            private const string ExpectedName = "New Name";
            private Guid _systemItemId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "Test", "Test");
                var domain = CreateDomain(dbContext, mappedSystem, "domain", string.Empty, string.Empty);
                var systemItem = CreateElement(dbContext, domain, "System Item", string.Empty);
                _systemItemId = systemItem.SystemItemId;

                var entityEditModel = new SystemItemNameEditModel
                {
                    SystemItemId = systemItem.SystemItemId,
                    ItemName = ExpectedName
                };

                var entityNameService = GetInstance<ISystemItemNameService>();
                _viewModel = entityNameService.Put(entityEditModel);
            }

            [Test]
            public void Should_get_system_item_name()
            {
                _viewModel.ItemName.ShouldEqual(ExpectedName);
            }

            [Test]
            public void Should_save_system_item_name()
            {
                var dbContext = CreateDbContext();
                var systemItem = dbContext.SystemItems.Single(x => x.SystemItemId == _systemItemId);
                systemItem.ItemName.ShouldEqual(ExpectedName);
            }
        }
    }
}