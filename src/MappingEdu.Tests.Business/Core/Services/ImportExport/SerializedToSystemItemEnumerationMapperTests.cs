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
    public class SerializedToSystemEnumerationItemMapperTests
    {
        [TestFixture]
        public class When_mapping_serialized_enumeration_value_to_system_enumeration_item : TestBase
        {
            private SerializedEnumerationValue _serializedEnumerationValue;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _enumeration;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _enumeration = New.SystemItem.AsEnumeration.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);
                _importOptions = new ImportOptions();

                _serializedEnumerationValue = new SerializedEnumerationValue
                {
                    CodeValue = "ABC Data",
                    ShortDescription = "Value of ABC",
                    Description = "Some description text"
                };

                var mapper = new SerializedToSystemItemEnumerationMapper();
                mapper.Map(_serializedEnumerationValue, _enumeration, _importOptions);
            }

            [Test]
            public void Should_create_new_enumeration_value_for_enumeration()
            {
                var enumerationValue = _enumeration.SystemEnumerationItems.FirstOrDefault();
                enumerationValue.ShouldNotBeNull();
                enumerationValue.CodeValue.ShouldEqual("ABC Data");
                enumerationValue.ShortDescription.ShouldEqual("Value of ABC");
                enumerationValue.Description.ShouldEqual("Some description text");
                enumerationValue.SystemItem.ShouldEqual(_enumeration);
            }
        }

        [TestFixture]
        public class When_mapping_serialized_enumeration_value_to_system_enumeration_item_and_matching_on_name : TestBase
        {
            private SerializedEnumerationValue _serializedEnumerationValue;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _enumeration;
            private SystemEnumerationItem _enumerationItem;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _enumeration = New.SystemItem.AsEnumeration.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);
                _enumerationItem = New.SystemEnumerationItem.WithCodeValue("ABC Data").WithSystemItem(_enumeration);
                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedEnumerationValue = new SerializedEnumerationValue
                {
                    CodeValue = "ABC Data",
                    ShortDescription = "Value of ABC",
                    Description = "Some description text"
                };

                var mapper = new SerializedToSystemItemEnumerationMapper();
                mapper.Map(_serializedEnumerationValue, _enumeration, _importOptions);
            }

            [Test]
            public void Should_not_create_new_enumeration_item()
            {
                _enumeration.SystemEnumerationItems.Count.ShouldEqual(1);
            }

            [Test]
            public void Should_update_matching_enumeration_item()
            {
                _enumerationItem.CodeValue.ShouldEqual("ABC Data");
                _enumerationItem.ShortDescription.ShouldEqual("Value of ABC");
                _enumerationItem.Description.ShouldEqual("Some description text");
            }
        }

        [TestFixture]
        public class When_mapping_serialized_element_to_system_item_and_matching_on_name_with_no_match : TestBase
        {
            private SerializedEnumerationValue _serializedEnumerationValue;
            private MappedSystem _mappedSystem;
            private SystemItem _domain;
            private SystemItem _enumeration;
            private ImportOptions _importOptions;

            [OneTimeSetUp]
            public void Setup()
            {
                _mappedSystem = New.MappedSystem;
                _domain = New.SystemItem.AsDomain.WithMappedSystem(_mappedSystem);
                _enumeration = New.SystemItem.AsEnumeration.WithMappedSystem(_mappedSystem).WithParentSystemItem(_domain);
                New.SystemEnumerationItem.WithCodeValue("ABC Data").WithSystemItem(_enumeration);
                _importOptions = new ImportOptions
                {
                    UpsertBasedOnName = true
                };

                _serializedEnumerationValue = new SerializedEnumerationValue
                {
                    CodeValue = "New ABC Data",
                    ShortDescription = "Value of ABC",
                    Description = "Some description text"
                };

                var mapper = new SerializedToSystemItemEnumerationMapper();
                mapper.Map(_serializedEnumerationValue, _enumeration, _importOptions);
            }

            [Test]
            public void Should_create_new_domain_system_item_for_mapped_system()
            {
                var enumerationItem = _enumeration.SystemEnumerationItems.FirstOrDefault(x => x.CodeValue == "New ABC Data");
                enumerationItem.ShouldNotBeNull();
                enumerationItem.CodeValue.ShouldEqual("New ABC Data");
                enumerationItem.ShortDescription.ShouldEqual("Value of ABC");
                enumerationItem.Description.ShouldEqual("Some description text");
                enumerationItem.SystemItem.ShouldEqual(_enumeration);
            }

            [Test]
            public void Should_create_new_element()
            {
                _enumeration.SystemEnumerationItems.Count.ShouldEqual(2);
            }
        }
    }
}