// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;
using MappingEdu.Core.Services.Import;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Rhino.Mocks;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    public class SerializedToMappedSystemMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_mapped_system_to_mapped_system : TestBase
        {
            private SerializedMappedSystem _serializedMappedSystem;
            private SerializedCustomDetailMetadata _customDetails1, _customDetails2;
            private SerializedDomain _domain1, _domain2;
            private MappedSystem _mappedSystem;
            private ImportOptions _importOptions;
            private ISerializedDomainToSystemItemMapper _serializedDomainToSystemItemMapper;
            private ISerializedToCustomDetailMetadataMapper _serializedToCustomDetailMetadataMapper;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;

                _serializedDomainToSystemItemMapper = GenerateStub<ISerializedDomainToSystemItemMapper>();
                _serializedToCustomDetailMetadataMapper = GenerateStub<ISerializedToCustomDetailMetadataMapper>();

                _customDetails1 = new SerializedCustomDetailMetadata();
                _customDetails2 = new SerializedCustomDetailMetadata();
                _domain1 = new SerializedDomain();
                _domain2 = new SerializedDomain();
                _importOptions = new ImportOptions();

                _serializedMappedSystem = new SerializedMappedSystem
                {
                    CustomDetails = new[] {_customDetails1, _customDetails2},
                    Domains = new[] {_domain1, _domain2}
                };

                var mapper = new SerializedToMappedSystemMapper(_serializedDomainToSystemItemMapper, _serializedToCustomDetailMetadataMapper);
                mapper.Map(_serializedMappedSystem, _mappedSystem, _importOptions);
            }

            [Test]
            public void Should_map_custom_details()
            {
                _serializedToCustomDetailMetadataMapper.AssertWasCalled(x => x.Map(_customDetails1, _mappedSystem, _importOptions));
                _serializedToCustomDetailMetadataMapper.AssertWasCalled(x => x.Map(_customDetails2, _mappedSystem, _importOptions));
            }

            [Test]
            public void Should_map_domains()
            {
                _serializedDomainToSystemItemMapper.AssertWasCalled(x => x.Map(_domain1, _mappedSystem, _importOptions));
                _serializedDomainToSystemItemMapper.AssertWasCalled(x => x.Map(_domain2, _mappedSystem, _importOptions));
            }
        }
    }
}