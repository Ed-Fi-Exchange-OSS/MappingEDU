// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain.Enumerations;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Enumerations
{
    public class DatabaseEnumerationTests
    {
        private class TestDbEnumeration : DatabaseEnumeration<TestDbEnumeration, string>
        {
            private readonly bool _isInDatabase;
            public static TestDbEnumeration One = new TestDbEnumeration("111", "One", true);
            public static TestDbEnumeration Two = new TestDbEnumeration("222", "Two", true);
            public static TestDbEnumeration Detractor = new TestDbEnumeration("333", "Three", false);

            public TestDbEnumeration(string myId, string myName, bool isInDatabase)
            {
                _isInDatabase = isInDatabase;
                MyId = myId;
                MyName = myName;
            }

            public string MyId { get; private set; }
            public string MyName { get; private set; }
            public override string Id
            {
                get { return MyId; }
            }

            public override string Name
            {
                get { return MyName; }
            }

            protected override bool IsInDatabase { get { return _isInDatabase; } }
        }

        [Test]
        public void Should_get_all_database_values()
        {
            TestDbEnumeration.GetValues().Length.ShouldEqual(3);  //Sanity Check

            TestDbEnumeration[] databaseValues = TestDbEnumeration.GetDatabaseValues();
            databaseValues.Length.ShouldEqual(2);
            databaseValues.ShouldContain(TestDbEnumeration.One);
            databaseValues.ShouldContain(TestDbEnumeration.Two);
        }

        [Test]
        public void Should_use_id_for_databaseId()
        {
            TestDbEnumeration.One.DatabaseId.ShouldEqual(TestDbEnumeration.One.Id);
        }

        [Test]
        public void Should_use_name_for_databaseText()
        {
            TestDbEnumeration.One.DatabaseText.ShouldEqual(TestDbEnumeration.One.Name);
        }
    }
}