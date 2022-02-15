// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.SystemItems;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Should;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class SystemItemCustomDetailServiceTests
    {
        [TestFixture]
        public class When_getting_system_item_custom_details_for_system_item : TestBase
        {
            private readonly Guid _systemItemId = Guid.NewGuid();
            private SystemItemCustomDetailViewModel[] _result;
            private SystemItemCustomDetailViewModel[] _expected;

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemCustomDetailId1 = Guid.NewGuid();
                var systemItemCustomDetailId2 = Guid.NewGuid();
                var customDetailMetadataId1 = Guid.NewGuid();
                var customDetailMetadataId2 = Guid.NewGuid();
                var mappedSystemId = Guid.NewGuid();

                _expected = new[]
                {
                    new SystemItemCustomDetailViewModel
                    {
                        SystemItemId = _systemItemId,
                        SystemItemCustomDetailId = systemItemCustomDetailId1,
                        CustomDetailMetadataId = customDetailMetadataId1,
                        CustomDetailMetadata = new CustomDetailMetadataViewModel
                        {
                            CustomDetailMetadataId = customDetailMetadataId1,
                            DisplayName = "Display Name",
                            IsBoolean = false,
                            IsCoreDetail = false,
                            MappedSystemId = mappedSystemId
                        },
                        Value = "Value"
                    },
                    new SystemItemCustomDetailViewModel
                    {
                        SystemItemId = _systemItemId,
                        SystemItemCustomDetailId = systemItemCustomDetailId2,
                        CustomDetailMetadataId = customDetailMetadataId2,
                        CustomDetailMetadata = new CustomDetailMetadataViewModel
                        {
                            CustomDetailMetadataId = customDetailMetadataId2,
                            DisplayName = "Boolean Name",
                            IsBoolean = true,
                            IsCoreDetail = true,
                            MappedSystemId = mappedSystemId
                        },
                        Value = "1"
                    }
                };

                var systemItemCustomDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();

                MappedSystem mappedSystem = New.MappedSystem.IsActive(true).WithId(mappedSystemId);
                CustomDetailMetadata customDetailMetadata1 = New.CustomDetailMetadata.WithDisplayName("Display Name").WithIsBoolean(false).WithIsCoreDetail(false).WithId(customDetailMetadataId1).WithMappedSystem(mappedSystem);
                CustomDetailMetadata customDetailMetadata2 = New.CustomDetailMetadata.WithDisplayName("Boolean Name").WithIsBoolean(true).WithIsCoreDetail(true).WithId(customDetailMetadataId2).WithMappedSystem(mappedSystem);
                SystemItem systemItem = New.SystemItem.WithId(_systemItemId).WithMappedSystem(mappedSystem);
                New.SystemItemCustomDetail.WithCustomDetailMetadata(customDetailMetadata1).WithId(systemItemCustomDetailId1).WithSystemItem(systemItem).WithValue("Value");
                New.SystemItemCustomDetail.WithCustomDetailMetadata(customDetailMetadata2).WithId(systemItemCustomDetailId2).WithSystemItem(systemItem).WithValue("1");


                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Expect(x => x.Get(_systemItemId)).Return(systemItem);

                var mapper = GenerateStub<IMapper>();
                mapper.Expect(
                    x => x.Map<SystemItemCustomDetailViewModel[]>(systemItem.SystemItemCustomDetails)).Return(_expected);

                ISystemItemCustomDetailService systemItemCustomDetailService =
                    new SystemItemCustomDetailService(systemItemCustomDetailRepository, systemItemRepository, mapper, null);
                _result = systemItemCustomDetailService.Get(_systemItemId);
            }

            [Test]
            public void Should_populate_view_models()
            {
                _result.ShouldNotBeNull();
                _result[0].SystemItemCustomDetailId.ShouldEqual(_expected[0].SystemItemCustomDetailId);
                _result[1].SystemItemCustomDetailId.ShouldEqual(_expected[1].SystemItemCustomDetailId);
            }
        }

        [TestFixture]
        public class When_getting_system_item_custom_details_for_non_existant_system_item : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemCustomDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Expect(x => x.Get(_systemItemId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                ISystemItemCustomDetailService systemItemCustomDetailService =
                    new SystemItemCustomDetailService(systemItemCustomDetailRepository, systemItemRepository, mapper, null);

                try
                {
                    systemItemCustomDetailService.Get(_systemItemId);
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The system item with id '{0}' does not exist.", _systemItemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_system_item_custom_detail_that_does_not_exist : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemItemCustomDetailId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemCustomDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();

                systemItemCustomDetailRepository.Expect(x => x.Get(_systemItemCustomDetailId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                ISystemItemCustomDetailService systemItemCustomDetailService =
                    new SystemItemCustomDetailService(systemItemCustomDetailRepository, systemItemRepository, mapper, null);

                try
                {
                    systemItemCustomDetailService.Get(_systemItemId, _systemItemCustomDetailId);
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The system item custom detail with id '{0}' does not exist.", _systemItemCustomDetailId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_system_item_custom_detail_with_invalid_system_item : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();
            private readonly Guid _systemItemCustomDetailId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemCustomDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();

                systemItemCustomDetailRepository.Expect(x => x.Get(_systemItemCustomDetailId)).Return(New.SystemItemCustomDetail);

                var mapper = GenerateStub<IMapper>();

                ISystemItemCustomDetailService systemItemCustomDetailService =
                    new SystemItemCustomDetailService(systemItemCustomDetailRepository, systemItemRepository, mapper, null);

                try
                {
                    systemItemCustomDetailService.Get(_systemItemId, _systemItemCustomDetailId);
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(
                    string.Format("The system item custom detail with id '{0}' does not have a system item id of '{1}'.", _systemItemCustomDetailId, _systemItemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_creating_a_system_item_custom_detail_for_non_existant_system_item : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _systemItemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var systemItemCustomDetailRepository = GenerateStub<IRepository<SystemItemCustomDetail>>();
                var systemItemRepository = GenerateStub<IRepository<SystemItem>>();
                systemItemRepository.Expect(x => x.Get(_systemItemId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                ISystemItemCustomDetailService systemItemCustomDetailService =
                    new SystemItemCustomDetailService(systemItemCustomDetailRepository, systemItemRepository, mapper, null);

                try
                {
                    systemItemCustomDetailService.Post(_systemItemId, new SystemItemCustomDetailCreateModel());
                }
                catch (Exception ex)
                {
                    _hasException = true;
                    _exception = ex;
                }
            }

            [Test]
            public void Should_have_a_meaningful_error_message()
            {
                _exception.Message.ShouldEqual(string.Format("The system item with id '{0}' does not exist.", _systemItemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }
    }
}