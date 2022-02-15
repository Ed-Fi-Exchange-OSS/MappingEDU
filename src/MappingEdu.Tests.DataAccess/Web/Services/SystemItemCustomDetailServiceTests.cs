// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.Web.Services
{
    public class SystemItemCustomDetailServiceTests
    {
        [TestFixture]
        public class When_adding_system_item_custom_detail : EmptyDatabaseTestBase
        {
            private Guid _systemItemId;
            private Guid _customDetailMetadataId;
            private SystemItemCustomDetailViewModel _systemItemCustomDetailViewModel;
            private const string Value = "Value";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Domain Name", "Domain Definition");
                _systemItemId = domain.SystemItemId;

                var customDetailMetadata = CreateCustomDetailMetadata(dbContext, mappedSystem, "Display Name", false, true);
                _customDetailMetadataId = customDetailMetadata.CustomDetailMetadataId;

                var systemItemCustomDetailService = GetInstance<ISystemItemCustomDetailService>();
                var systemItemCustomDetailCreateModel = new SystemItemCustomDetailCreateModel
                {
                    CustomDetailMetadataId = customDetailMetadata.CustomDetailMetadataId,
                    Value = Value
                };

                _systemItemCustomDetailViewModel = systemItemCustomDetailService.Post(_systemItemId, systemItemCustomDetailCreateModel);
            }

            [Test]
            public void Should_create_a_new_system_item_custom_detail()
            {
                var dbContext = CreateDbContext();
                var systemItem = dbContext.SystemItems.Find(_systemItemId);
                var systemItemCustomDetail = systemItem.SystemItemCustomDetails.Single();
                systemItemCustomDetail.ShouldNotBeNull();
                systemItemCustomDetail.CustomDetailMetadataId.ShouldEqual(_customDetailMetadataId);
                systemItemCustomDetail.Value.ShouldEqual(Value);
            }

            [Test]
            public void Should_return_a_valid_view_model()
            {
                _systemItemCustomDetailViewModel.ShouldNotBeNull();
                _systemItemCustomDetailViewModel.CustomDetailMetadataId.ShouldEqual(_customDetailMetadataId);
                _systemItemCustomDetailViewModel.Value.ShouldEqual(Value);
            }
        }

        [TestFixture]
        public class When_updating_system_item_custom_detail : EmptyDatabaseTestBase
        {
            private Guid _systemItemId;
            private Guid _customDetailMetadataId;
            private Guid _systemItemCustomDetailId;
            private SystemItemCustomDetailViewModel _systemItemCustomDetailViewModel;
            private const string Value = "Value";
            private const string NewValue = "New Value";

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Domain Name", "Domain Definition");
                _systemItemId = domain.SystemItemId;

                var customDetailMetadata = CreateCustomDetailMetadata(dbContext, mappedSystem, "Display Name", false, true);
                _customDetailMetadataId = customDetailMetadata.CustomDetailMetadataId;

                var systemItemCustomDetail = CreateSystemItemCustomDetail(dbContext, domain, customDetailMetadata, Value);
                _systemItemCustomDetailId = systemItemCustomDetail.SystemItemCustomDetailId;

                var systemItemCustomDetailService = GetInstance<ISystemItemCustomDetailService>();
                var systemItemCustomDetailCreateModel = new SystemItemCustomDetailEditModel
                {
                    SystemItemCustomDetailId = _systemItemCustomDetailId,
                    CustomDetailMetadataId = _customDetailMetadataId,
                    Value = NewValue
                };

                _systemItemCustomDetailViewModel = systemItemCustomDetailService.Put(_systemItemId, systemItemCustomDetailCreateModel);
            }

            [Test]
            public void Should_return_a_valid_view_model()
            {
                _systemItemCustomDetailViewModel.ShouldNotBeNull();
                _systemItemCustomDetailViewModel.CustomDetailMetadataId.ShouldEqual(_customDetailMetadataId);
                _systemItemCustomDetailViewModel.Value.ShouldEqual(NewValue);
            }

            [Test]
            public void Should_update_the_system_item_custom_detail()
            {
                var dbContext = CreateDbContext();
                var systemItem = dbContext.SystemItems.Find(_systemItemId);
                var systemItemCustomDetail = systemItem.SystemItemCustomDetails.Single();
                systemItemCustomDetail.ShouldNotBeNull();
                systemItemCustomDetail.CustomDetailMetadataId.ShouldEqual(_customDetailMetadataId);
                systemItemCustomDetail.Value.ShouldEqual(NewValue);
            }
        }

        [TestFixture]
        public class When_deleting_system_item_custom_detail : EmptyDatabaseTestBase
        {
            private Guid _systemItemId;
            private Guid _systemItemCustomDetailId;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                var domain = CreateDomain(dbContext, mappedSystem, "Domain Name", "Domain Definition");
                _systemItemId = domain.SystemItemId;

                var customDetailMetadata = CreateCustomDetailMetadata(dbContext, mappedSystem, "Display Name", false, true);

                var systemItemCustomDetail = CreateSystemItemCustomDetail(dbContext, domain, customDetailMetadata, "Value");
                _systemItemCustomDetailId = systemItemCustomDetail.SystemItemCustomDetailId;

                var systemItemCustomDetailService = GetInstance<ISystemItemCustomDetailService>();

                systemItemCustomDetailService.Delete(_systemItemId, _systemItemCustomDetailId);
            }

            [Test]
            public void Should_update_the_system_item_custom_detail()
            {
                var dbContext = CreateDbContext();
                var systemItem = dbContext.SystemItems.Find(_systemItemId);
                var systemItemCustomDetail = systemItem.SystemItemCustomDetails.SingleOrDefault();
                systemItemCustomDetail.ShouldBeNull();
            }
        }
    }
}