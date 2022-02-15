// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Services.Import;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    public class SerializedToCustomDetailMetadataMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_custom_detail_to_custom_detail_metadata : TestBase
        {
            private SerializedCustomDetailMetadata _serializedCustomDetail;
            private MappedSystem _mappedSystem;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _importOptions = new ImportOptions();

                _serializedCustomDetail = new SerializedCustomDetailMetadata
                {
                    Name = "ABC Data",
                    IsBoolean = true
                };

                var mapper = new SerializedToCustomDetailMetadataMapper();
                mapper.Map(_serializedCustomDetail, _mappedSystem, _importOptions);
            }

            [Test]
            public void Should_create_new_custom_detail_metadata_for_mapped_system()
            {
                var customDetailMetadata = _mappedSystem.CustomDetailMetadata.FirstOrDefault();
                customDetailMetadata.ShouldNotBeNull();
                customDetailMetadata.DisplayName.ShouldEqual("ABC Data");
                customDetailMetadata.IsBoolean.ShouldBeTrue();
                customDetailMetadata.MappedSystem.ShouldEqual(_mappedSystem);
            }
        }

        [TestFixture]
        public class When_mapping_serialized_custom_detail_and_matching_on_name : TestBase
        {
            private SerializedCustomDetailMetadata _serializedCustomDetail;
            private MappedSystem _mappedSystem;
            private CustomDetailMetadata _customDetailMetadata;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _customDetailMetadata = New.CustomDetailMetadata.WithDisplayName("ABC Data")
                    .WithIsBoolean(false)
                    .WithMappedSystem(_mappedSystem);
                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedCustomDetail = new SerializedCustomDetailMetadata
                {
                    Name = "ABC Data",
                    IsBoolean = true
                };

                var mapper = new SerializedToCustomDetailMetadataMapper();
                mapper.Map(_serializedCustomDetail, _mappedSystem, _importOptions);
            }

            [Test]
            public void Should_not_create_new_custom_detail_metadata()
            {
                _mappedSystem.CustomDetailMetadata.Count.ShouldEqual(1);
                _customDetailMetadata.CustomDetailMetadataId.ShouldNotEqual(Guid.Empty);
            }

            [Test]
            public void Should_update_matching_custom_detail_metadata()
            {
                _customDetailMetadata.DisplayName.ShouldEqual("ABC Data");
                _customDetailMetadata.IsBoolean.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_mapping_serialized_custom_detail_and_matching_on_name_with_no_match : TestBase
        {
            private SerializedCustomDetailMetadata _serializedCustomDetail;
            private MappedSystem _mappedSystem;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                New.CustomDetailMetadata.WithDisplayName("ABC Data").WithMappedSystem(_mappedSystem);
                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedCustomDetail = new SerializedCustomDetailMetadata
                {
                    Name = "New ABC Data",
                    IsBoolean = true
                };

                var mapper = new SerializedToCustomDetailMetadataMapper();
                mapper.Map(_serializedCustomDetail, _mappedSystem, _importOptions);
            }

            [Test]
            public void Should_create_new_custom_detail_metadata()
            {
                _mappedSystem.CustomDetailMetadata.Count.ShouldEqual(2);
            }

            [Test]
            public void Should_create_new_domain_system_item_for_mapped_system()
            {
                var customDetailMetadata = _mappedSystem.CustomDetailMetadata.FirstOrDefault(x => x.DisplayName == "New ABC Data");
                customDetailMetadata.ShouldNotBeNull();
                customDetailMetadata.DisplayName.ShouldEqual("New ABC Data");
                customDetailMetadata.IsBoolean.ShouldBeTrue();
                customDetailMetadata.MappedSystem.ShouldEqual(_mappedSystem);
            }
        }
    }
}