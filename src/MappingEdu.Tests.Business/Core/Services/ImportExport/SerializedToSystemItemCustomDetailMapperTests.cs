// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Import;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    public class SerializedToSystemItemCustomDetailMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_element_custom_detail_to_system_item_custom_detail : TestBase
        {
            private SerializedElementCustomDetail _serializedElementCustomDetail;
            private MappedSystem _mappedSystem;
            private CustomDetailMetadata _customDetailMetadata;
            private SystemItem _domain;
            private SystemItem _entity;
            private SystemItem _element;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _customDetailMetadata = New.CustomDetailMetadata.WithDisplayName("ABC Data").WithMappedSystem(_mappedSystem);
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _entity = New.SystemItem.AsEntity.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);
                _element = New.SystemItem.AsElement.WithMappedSystem(_mappedSystem).WithParentSystemItem(_entity);
                _importOptions = new ImportOptions();

                _serializedElementCustomDetail = new SerializedElementCustomDetail
                {
                    Name = "ABC Data",
                    Value = "Value of ABC"
                };

                var mapper = new SerializedToSystemItemCustomDetailMapper();
                mapper.Map(_serializedElementCustomDetail, _element, _importOptions);
            }

            [Test]
            public void Should_create_new_custom_detail_for_element()
            {
                var itemCustomDetail = _element.SystemItemCustomDetails.FirstOrDefault();
                itemCustomDetail.ShouldNotBeNull();
                itemCustomDetail.Value.ShouldEqual("Value of ABC");
                itemCustomDetail.SystemItem.ShouldEqual(_element);
                itemCustomDetail.CustomDetailMetadata.ShouldEqual(_customDetailMetadata);
            }
        }

        // probably want to revisit this in the future 
        [TestFixture]
        public class When_mapping_serialized_element_custom_detail_to_system_item_custom_detail_and_not_matching_on_name : TestBase
        {
            private SerializedElementCustomDetail _serializedElementCustomDetail;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _entity;
            private SystemItem _element;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _entity = New.SystemItem.AsEntity.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);
                _element = New.SystemItem.AsElement.WithMappedSystem(_mappedSystem).WithParentSystemItem(_entity);
                _importOptions = new ImportOptions();

                _serializedElementCustomDetail = new SerializedElementCustomDetail
                {
                    Name = "ABC Data",
                    Value = "Value of ABC"
                };

                var mapper = new SerializedToSystemItemCustomDetailMapper();
                mapper.Map(_serializedElementCustomDetail, _element, _importOptions);
            }

            [Test]
            public void Should_not_create_custom_detail()
            {
                _element.SystemItemCustomDetails.Count.ShouldEqual(0);
            }
        }
    }
}