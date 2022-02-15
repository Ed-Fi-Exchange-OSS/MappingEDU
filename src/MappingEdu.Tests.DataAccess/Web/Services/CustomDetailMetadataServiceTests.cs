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
    public class CustomDetailMetadataServiceTests
    {
        [TestFixture]
        public class When_adding_custom_detail_metadata : EmptyDatabaseTestBase
        {
            private CustomDetailMetadataViewModel _customDetailMetadataViewModel;
            private Guid _mappedSystemId;
            private const string DisplayName = "Display Name";
            private const bool IsBoolean = true;
            private const bool IsCoreDetail = true;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                _mappedSystemId = mappedSystem.MappedSystemId;

                var model = new CustomDetailMetadataCreateModel
                {
                    DisplayName = DisplayName,
                    IsBoolean = IsBoolean,
                    IsCoreDetail = IsCoreDetail
                };

                var customDetailMetadataService = GetInstance<ICustomDetailMetadataService>();
                _customDetailMetadataViewModel = customDetailMetadataService.Post(_mappedSystemId, model);
            }

            [Test]
            public void Should_create_a_new_custom_detail_metadata()
            {
                var dbContext = CreateDbContext();
                var mappedSystem = dbContext.MappedSystems.Find(_mappedSystemId);
                var customDetailMetadata = mappedSystem.CustomDetailMetadata.Single();
                customDetailMetadata.CustomDetailMetadataId.ShouldNotEqual(Guid.Empty);
                customDetailMetadata.DisplayName.ShouldEqual(DisplayName);
                customDetailMetadata.IsBoolean.ShouldEqual(IsBoolean);
                customDetailMetadata.IsCoreDetail.ShouldEqual(IsCoreDetail);
                customDetailMetadata.MappedSystemId.ShouldEqual(_mappedSystemId);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _customDetailMetadataViewModel.ShouldNotBeNull();
                _customDetailMetadataViewModel.DisplayName.ShouldEqual(DisplayName);
                _customDetailMetadataViewModel.IsBoolean.ShouldEqual(IsBoolean);
                _customDetailMetadataViewModel.IsCoreDetail.ShouldEqual(IsCoreDetail);
                _customDetailMetadataViewModel.MappedSystemId.ShouldEqual(_mappedSystemId);
            }
        }

        [TestFixture]
        public class When_updating_custom_detail_metadata : EmptyDatabaseTestBase
        {
            private CustomDetailMetadataViewModel _customDetailMetadataViewModel;
            private Guid _mappedSystemId;
            private Guid _customDetailMetadataId;
            private const string DisplayName = "Display Name";
            private const bool IsBoolean = false;
            private const bool IsCoreDetail = true;
            private const string NewDisplayName = "New Display Name";
            private const bool NewIsBoolean = true;
            private const bool NewIsCoreDetail = false;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                _mappedSystemId = mappedSystem.MappedSystemId;

                var customDetail = CreateCustomDetailMetadata(dbContext, mappedSystem, DisplayName, IsBoolean, IsCoreDetail);
                _customDetailMetadataId = customDetail.CustomDetailMetadataId;

                var customDetailMetadataService = GetInstance<ICustomDetailMetadataService>();
                var model = new CustomDetailMetadataEditModel
                {
                    DisplayName = NewDisplayName,
                    IsBoolean = NewIsBoolean,
                    IsCoreDetail = NewIsCoreDetail
                };

                _customDetailMetadataViewModel = customDetailMetadataService.Put(_mappedSystemId, _customDetailMetadataId, model);
            }

            [Test]
            public void Should_create_a_new_custom_detail_metadata()
            {
                var dbContext = CreateDbContext();
                var mappedSystem = dbContext.MappedSystems.Find(_mappedSystemId);
                var customDetailMetadata = mappedSystem.CustomDetailMetadata.Single();
                customDetailMetadata.CustomDetailMetadataId.ShouldNotEqual(Guid.Empty);
                customDetailMetadata.DisplayName.ShouldEqual(NewDisplayName);
                customDetailMetadata.IsBoolean.ShouldEqual(NewIsBoolean);
                customDetailMetadata.IsCoreDetail.ShouldEqual(NewIsCoreDetail);
                customDetailMetadata.MappedSystemId.ShouldEqual(_mappedSystemId);
            }

            [Test]
            public void Should_return_valid_view_model()
            {
                _customDetailMetadataViewModel.ShouldNotBeNull();
                _customDetailMetadataViewModel.DisplayName.ShouldEqual(NewDisplayName);
                _customDetailMetadataViewModel.IsBoolean.ShouldEqual(NewIsBoolean);
                _customDetailMetadataViewModel.IsCoreDetail.ShouldEqual(NewIsCoreDetail);
                _customDetailMetadataViewModel.MappedSystemId.ShouldEqual(_mappedSystemId);
            }
        }

        [TestFixture]
        public class When_deleting_custom_detail_metadata : EmptyDatabaseTestBase
        {
            private Guid _mappedSystemId;
            private Guid _customDetailMetadataId;
            private const string DisplayName = "Display Name";
            private const bool IsBoolean = false;
            private const bool IsCoreDetail = true;

            protected override void EstablishContext()
            {
                base.EstablishContext();

                var dbContext = CreateDbContext();
                var mappedSystem = CreateMappedSystem(dbContext, "System Name", "1.0.0");
                _mappedSystemId = mappedSystem.MappedSystemId;

                var customDetail = CreateCustomDetailMetadata(dbContext, mappedSystem, DisplayName, IsBoolean, IsCoreDetail);
                _customDetailMetadataId = customDetail.CustomDetailMetadataId;

                var customDetailMetadataService = GetInstance<ICustomDetailMetadataService>();
                customDetailMetadataService.Delete(_mappedSystemId, _customDetailMetadataId);
            }

            [Test]
            public void Should_create_a_new_custom_detail_metadata()
            {
                var dbContext = CreateDbContext();
                var mappedSystem = dbContext.MappedSystems.Find(_mappedSystemId);
                var customDetailMetadata = mappedSystem.CustomDetailMetadata.SingleOrDefault();
                customDetailMetadata.ShouldBeNull();
            }
        }
    }
}