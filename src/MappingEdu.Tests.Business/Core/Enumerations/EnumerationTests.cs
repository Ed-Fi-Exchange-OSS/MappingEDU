// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Enumerations
{
    public class EnumerationTests
    {
        [TestFixture]
        public class When_enumeration_has_values : TestBase
        {
            private TestEnumeration[] _results;

            private class TestEnumeration : Enumeration<TestEnumeration, string>
            {
                public string MyId { get; private set; }
                public override string Id { get { return MyId; } }
                public string Name { get; private set; }

                public static TestEnumeration One = new TestEnumeration("1", "One");
                public static TestEnumeration Two = new TestEnumeration("2", "Two", true);

                private static TestEnumeration Problematic = new TestEnumeration("3", "I Cause Problems");

                public static string Detractor = "foo";

                public TestEnumeration(string id, string name, bool isDefault = false) : base(isDefault)
                {
                    MyId = id;
                    Name = name;
                }
            }

            [OneTimeSetUp]
            public void Setup()
            {
                _results = TestEnumeration.GetValues();
            }

            [Test]
            public void Should_retrieve_all_values()
            {
                _results.Length.ShouldEqual(2);
                _results.ShouldContain(TestEnumeration.One);
                _results.ShouldContain(TestEnumeration.Two);
            }

            [Test]
            public void Should_retrieve_value_by_id()
            {
                TestEnumeration.GetById("1").ShouldEqual(TestEnumeration.One);
            }

            [Test]
            public void Should_return_null_when_searching_by_nonexistent_id()
            {
                TestEnumeration.GetById("foo").ShouldBeNull();
            }

            [Test]
            public void Should_return_default_value()
            {
                TestEnumeration.GetDefault().ShouldEqual(TestEnumeration.Two);
            }
        }

        [TestFixture]
        public class When_enumeration_has_value_with_null_id : TestBase
        {
            private TestEnumeration[] _results;

            private class TestEnumeration : Enumeration<TestEnumeration, string>
            {
                public string MyId { get; private set; }
                public override string Id { get { return MyId; } }
                public string Name { get; private set; }

                public static TestEnumeration Null = new TestEnumeration(null, "Null");
                public static TestEnumeration One = new TestEnumeration("1", "One");
                public static TestEnumeration Two = new TestEnumeration("2", "Two", true);

                private static TestEnumeration Problematic = new TestEnumeration("3", "I Cause Problems");

                public static string Detractor = "foo";

                public TestEnumeration(string id, string name, bool isDefault = false) : base(isDefault)
                {
                    MyId = id;
                    Name = name;
                }
            }

            [OneTimeSetUp]
            public void Setup()
            {
                _results = TestEnumeration.GetValues();
            }

            [Test]
            public void Should_retrieve_all_values()
            {
                _results.Length.ShouldEqual(3);
                _results.ShouldContain(TestEnumeration.Null);
                _results.ShouldContain(TestEnumeration.One);
                _results.ShouldContain(TestEnumeration.Two);
            }

            [Test]
            public void Should_retrieve_value_by_id()
            {
                TestEnumeration.GetById("1").ShouldEqual(TestEnumeration.One);
            }

            [Test]
            public void Should_retrieve_value_with_null_id()
            {
                TestEnumeration.GetById(null).ShouldEqual(TestEnumeration.Null);
            }

            [Test]
            public void Should_return_null_when_searching_by_nonexistent_id()
            {
                TestEnumeration.GetById("foo").ShouldBeNull();
            }

            [Test]
            public void Should_return_default_value()
            {
                TestEnumeration.GetDefault().ShouldEqual(TestEnumeration.Two);
            }
        }

        [TestFixture]
        public class When_ids_are_equal : TestBase
        {
            private class EqualsEnumeration : Enumeration<EqualsEnumeration, int>
            {
                public override int Id
                {
                    get { return MyId; }
                }

                public int MyId { get; private set; }
                public string Name { get; private set; }

                public static EqualsEnumeration One = new EqualsEnumeration(1, "One");
                public static EqualsEnumeration Two = new EqualsEnumeration(1, "Two");

                public EqualsEnumeration(int id, string name)
                {
                    MyId = id;
                    Name = name;
                }
            }

            [Test]
            public void Should_be_equal()
            {
                EqualsEnumeration.One.ShouldEqual(EqualsEnumeration.Two);
            }

            [Test]
            public void Should_throw_exception_when_searching_by_duplicated_id()
            {
                try
                {
                    EqualsEnumeration.GetById(1);
                }
                catch
                {
                    Assert.Pass("Nothing to see here.... These are not the tests you're looking for.");
                }
                Assert.Fail("Should have thrown an exception");
            }
        }

        [TestFixture]
        public class When_ids_are_not_equal : TestBase
        {
            private class EqualsEnumeration : Enumeration<EqualsEnumeration, int>
            {
                public override int Id
                {
                    get { return MyId; }
                }

                public int MyId { get; private set; }
                public string Name { get; private set; }

                public static EqualsEnumeration One = new EqualsEnumeration(1, "One");
                public static EqualsEnumeration Two = new EqualsEnumeration(2, "Two");

                public EqualsEnumeration(int id, string name)
                {
                    MyId = id;
                    Name = name;
                }
            }

            [Test]
            public void Should_not_be_equal()
            {
                EqualsEnumeration.One.ShouldNotEqual(EqualsEnumeration.Two);
            }
        }

        [TestFixture]
        public class When_more_than_one_default_value_is_defined : TestBase
        {
            private class TestEnumeration : Enumeration<TestEnumeration, string>
            {
                public string MyId { get; private set; }
                public override string Id { get { return MyId; } }
                public string Name { get; private set; }

                public static TestEnumeration One = new TestEnumeration("1", "One", true);
                public static TestEnumeration Two = new TestEnumeration("2", "Two", true);

                public TestEnumeration(string id, string name, bool isDefault = false)
                    : base(isDefault)
                {
                    MyId = id;
                    Name = name;
                }
            }

            [Test]
            public void Should_throw_exception()
            {
                try
                {
                    TestEnumeration.GetDefault();
                }
                catch (Exception)
                {
                    Assert.Pass("I totally expected that exception.");
                }
                Assert.Fail("TestEnumeration.GetDefault should have thrown exception.");
            }
        }

        [TestFixture]
        public class When_no_default_value_is_defined : TestBase
        {
            private class TestEnumeration : Enumeration<TestEnumeration, string>
            {
                public string MyId { get; private set; }
                public override string Id { get { return MyId; } }
                public string Name { get; private set; }

                public static TestEnumeration One = new TestEnumeration("1", "One");
                public static TestEnumeration Two = new TestEnumeration("2", "Two");

                public TestEnumeration(string id, string name, bool isDefault = false)
                    : base(isDefault)
                {
                    MyId = id;
                    Name = name;
                }
            }

            [Test]
            public void Should_throw_exception()
            {
                try
                {
                    TestEnumeration.GetDefault();
                }
                catch (Exception)
                {
                    Assert.Pass("I totally expected that exception.");
                }
                Assert.Fail("TestEnumeration.GetDefault should have thrown exception.");
            }

        }
    }
}
