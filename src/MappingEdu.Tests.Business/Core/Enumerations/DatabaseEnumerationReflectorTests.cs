// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Enumerations
{
    public class DatabaseEnumerationReflectorTests
    {
        [Test]
        public void Should_provide_all_database_values()
        {
            var reflector = new DatabaseEnumerationReflector();

            var testType = typeof (ItemDataType);

            var declaredValues = ItemDataType.GetDatabaseValues();
            var reflectedValues = reflector.GetDatabaseValues(testType.Name);

            foreach (var declaredValue in declaredValues)
            {
                var id = declaredValue.DatabaseId;
                var reflectedValue = reflectedValues.SingleOrDefault(x => x.DatabaseId.Equals(id));
                if (reflectedValue == null)
                    Assert.Fail("Could not find {0} with id {1}", testType, id);
                reflectedValue.DatabaseText.ShouldEqual(declaredValue.DatabaseText);
            }
        }
    }
}