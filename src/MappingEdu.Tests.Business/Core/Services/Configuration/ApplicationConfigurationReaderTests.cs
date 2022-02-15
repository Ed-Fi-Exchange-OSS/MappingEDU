// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Services.Configuration;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Services.Configuration
{
    public class ApplicationConfigurationReaderTests
    {
        [TestFixture]
        public class When_setting_exists_in_the_configuraiton_file
        {
            private readonly string _key = "settingForTestingApplicationConfigurationReader";
            private readonly ApplicationConfigurationReader _reader = new ApplicationConfigurationReader();

            [Test]
            public void Should_indicate_that_setting_exists()
            {
                _reader.HasSetting(_key).ShouldBeTrue();
            }

            [Test]
            public void Should_indicate_that_setting_exists_when_getting_optional_setting()
            {
                _reader.GetOptionalSetting(_key).IsSet.ShouldBeTrue();
            }

            [Test]
            public void Should_return_value_of_setting()
            {
                _reader.GetSetting(_key).ShouldEqual("I Am Expected");
            }

            [Test]
            public void Should_return_value_of_setting_when_getting_optional_setting()
            {
                _reader.GetOptionalSetting(_key).Value.ShouldEqual("I Am Expected");
            }
        }

        [TestFixture]
        public class When_setting_does_not_exist_in_the_cofiguration_file
        {
            private readonly string _key = "doesNotExist";
            private readonly ApplicationConfigurationReader _reader = new ApplicationConfigurationReader();

            [Test]
            public void Should_indicate_setting_does_not_exist()
            {
                _reader.HasSetting(_key).ShouldBeFalse();
            }

            [Test]
            public void Should_indicate_setting_does_not_exist_when_getting_optional_setting()
            {
                _reader.GetOptionalSetting(_key).IsSet.ShouldBeFalse();
            }

            [Test]
            public void Should_throw_exception_for_trying_to_retrieve_a_setting_that_doesnt_exist()
            {
                try
                {
                    _reader.GetSetting(_key);
                }
                catch
                {
                    Assert.Pass();
                }
                Assert.Fail("This should have thrown an exception");
            }
        }

        [TestFixture]
        public class When_connectionString_exists_in_the_configuraiton_file
        {
            private readonly string _key = "connectionStringForTestingApplicationConfigurationReader";
            private readonly ApplicationConfigurationReader _reader = new ApplicationConfigurationReader();

            [Test]
            public void Should_indicate_that_connectionString_exists()
            {
                _reader.HasConnectionString(_key).ShouldBeTrue();
            }

            [Test]
            public void Should_return_value_of_connectionString()
            {
                _reader.GetConnectionString(_key).ShouldEqual("I Am A Connection String");
            }
        }

        [TestFixture]
        public class When_connectionString_does_not_exist_in_the_configuration_file
        {
            private readonly string _key = "doesNotExist";
            private readonly ApplicationConfigurationReader _reader = new ApplicationConfigurationReader();

            [Test]
            public void Should_indicate_connectionString_does_not_exist()
            {
                _reader.HasConnectionString(_key).ShouldBeFalse();
            }

            [Test]
            public void Should_throw_exception_for_trying_to_retrieve_a_connectionString_that_doesnt_exist()
            {
                try
                {
                    _reader.GetConnectionString(_key);
                }
                catch
                {
                    Assert.Pass();
                }
                Assert.Fail("This should have thrown an exception");
            }
        }
    }
}