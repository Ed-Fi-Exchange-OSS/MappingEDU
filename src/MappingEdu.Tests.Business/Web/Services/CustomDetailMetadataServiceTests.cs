// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
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
    public class CustomDetailMetadataServiceTests
    {
        [TestFixture]
        public class When_getting_custom_detail_metadata_for_mapped_system : TestBase
        {
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private CustomDetailMetadataViewModel[] _result;
            private CustomDetailMetadataViewModel[] _expected;

            [OneTimeSetUp]
            public void Setup()
            {
                var customDetailMetadataRepository = GenerateStub<IRepository<CustomDetailMetadata>>();
                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();

                MappedSystem mappedSystem = New.MappedSystem;
                mappedSystemRepository.Expect(x => x.Get(_mappedSystemId)).Return(mappedSystem);

                var mapper = GenerateStub<IMapper>();
                _expected = new[]
                {
                    new CustomDetailMetadataViewModel
                    {
                        DisplayName = "Display Name",
                        IsBoolean = false,
                        IsCoreDetail = false,
                        MappedSystemId = _mappedSystemId
                    }
                };
                mapper.Expect(x => x.Map<CustomDetailMetadataViewModel[]>(mappedSystem.CustomDetailMetadata)).Return(_expected);

                ICustomDetailMetadataService customDetailMetadataService =
                    new CustomDetailMetadataService(customDetailMetadataRepository, mappedSystemRepository, mapper, null);

                _result = customDetailMetadataService.Get(_mappedSystemId);
            }

            [Test]
            public void Should_populate_view_models()
            {
                _result.ShouldNotBeNull();
                _result.ShouldEqual(_expected);
            }
        }

        [TestFixture]
        public class When_getting_custom_detail_metadata_for_non_existant_mapped_system : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _mappedSystemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var customDetailMetadataRepository = GenerateStub<IRepository<CustomDetailMetadata>>();
                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();

                mappedSystemRepository.Expect(x => x.Get(_mappedSystemId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                ICustomDetailMetadataService customDetailMetadataService =
                    new CustomDetailMetadataService(customDetailMetadataRepository, mappedSystemRepository, mapper, null);

                try
                {
                    customDetailMetadataService.Get(_mappedSystemId);
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
                _exception.Message.ShouldEqual(string.Format("The mapped system with id '{0}' does not exist.", _mappedSystemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_custom_detail_metadata_that_does_not_exist : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private readonly Guid _customDetailMetadataId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var customDetailMetadataRepository = GenerateStub<IRepository<CustomDetailMetadata>>();
                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();

                customDetailMetadataRepository.Expect(x => x.Get(_customDetailMetadataId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                ICustomDetailMetadataService customDetailMetadataService =
                    new CustomDetailMetadataService(customDetailMetadataRepository, mappedSystemRepository, mapper, null);

                try
                {
                    customDetailMetadataService.Get(_mappedSystemId, _customDetailMetadataId);
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
                _exception.Message.ShouldEqual(string.Format("The custom detail metadata with id '{0}' does not exist.", _customDetailMetadataId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_getting_custom_detail_metadata_with_invalid_mapped_system : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _mappedSystemId = Guid.NewGuid();
            private readonly Guid _customDetailMetadataId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var customDetailMetadataRepository = GenerateStub<IRepository<CustomDetailMetadata>>();
                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();

                customDetailMetadataRepository.Expect(x => x.Get(_customDetailMetadataId)).Return(New.CustomDetailMetadata.WithId(_customDetailMetadataId));

                var mapper = GenerateStub<IMapper>();

                ICustomDetailMetadataService customDetailMetadataService =
                    new CustomDetailMetadataService(customDetailMetadataRepository, mappedSystemRepository, mapper, null);

                try
                {
                    customDetailMetadataService.Get(_mappedSystemId, _customDetailMetadataId);
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
                    string.Format("The custom detail metadata with id '{0}' does not have a mapped system id of '{1}'.", _customDetailMetadataId, _mappedSystemId));
            }

            [Test]
            public void Should_throw_an_exception()
            {
                _hasException.ShouldBeTrue();
                _exception.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_creating_a_custom_detail_metadata_for_non_existant_mapped_system : TestBase
        {
            private bool _hasException;
            private Exception _exception;
            private readonly Guid _mappedSystemId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                var customDetailMetadataRepository = GenerateStub<IRepository<CustomDetailMetadata>>();
                var mappedSystemRepository = GenerateStub<IRepository<MappedSystem>>();

                mappedSystemRepository.Expect(x => x.Get(_mappedSystemId)).Return(null);

                var mapper = GenerateStub<IMapper>();

                ICustomDetailMetadataService customDetailMetadataService =
                    new CustomDetailMetadataService(customDetailMetadataRepository, mappedSystemRepository, mapper, null);

                try
                {
                    customDetailMetadataService.Post(_mappedSystemId, new CustomDetailMetadataCreateModel());
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
                _exception.Message.ShouldEqual(string.Format("The mapped system with id '{0}' does not exist.", _mappedSystemId));
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