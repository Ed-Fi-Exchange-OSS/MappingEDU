// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Services.Validation;
using MappingEdu.Tests.DataAccess.Bases;
using NUnit.Framework;
using Should;

namespace MappingEdu.Tests.DataAccess.DependencyResolution
{
    [TestFixture]
    public class IoCTests : EmptyDatabaseTestBase
    {
        [Test]
        public void Should_not_double_register_validation_rules()
        {
            var rules = GetInstances<IValidationRule<CustomDetailMetadata>>();
            rules.ShouldNotBeNull();
            rules.Length.ShouldBeLessThanOrEqualTo(1);
        }

        [Test]
        public void Should_use_database_context_as_a_singleton()
        {
            var one = GetInstance<EntityContext>();
            var two = GetInstance<EntityContext>();

            one.ShouldBeSameAs(two);
        }
    }
}