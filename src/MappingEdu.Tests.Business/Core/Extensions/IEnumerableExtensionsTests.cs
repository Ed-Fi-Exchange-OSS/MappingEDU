// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Common.Extensions;
using MappingEdu.Tests.Business.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.Business.Core.Extensions
{
    public class IEnumerableExtensionsTests
    {
        [TestFixture]
        public class When_checking_none_and_collection_has_matching_element : TestBase
        {
            [Test]
            public void Should_be_false()
            {
                var collection = new[] {1, 2, 3};
                var result = collection.None(x => x == 2);
                result.ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_checking_none_and_collection_does_not_have_matching_element : TestBase
        {
            [Test]
            public void Should_be_true()
            {
                var collection = new[] {1, 2, 3};
                var result = collection.None(x => x == 4);
                result.ShouldBeTrue();
            }
        }
    }
}