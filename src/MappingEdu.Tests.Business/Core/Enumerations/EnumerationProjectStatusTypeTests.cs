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
    [TestFixture]
    public class EnumerationProjectStatusTypeTests
    {
        private ProjectStatusType[] _testResults;

        [OneTimeSetUp]
        public void Setup()
        {
            _testResults = new[]
            {
                ProjectStatusType.Unknown,
                ProjectStatusType.Active,
                ProjectStatusType.Closed
            };
        }

        [Test]
        public void Active_enum_should_return_correct_name()
        {
            var result = _testResults.First(x => x.Name == "Active");
            result.ShouldNotBeNull();
            result.Id.ShouldEqual(1);
        }

        [Test]
        public void Active_enum_value_should_return_correct_id()
        {
            ProjectStatusType.Active.Id.ShouldEqual(1);
        }

        [Test]
        public void Closed_enum_should_return_correct_name()
        {
            var result = _testResults.First(x => x.Name == "Closed");
            result.ShouldNotBeNull();
            result.Id.ShouldEqual(2);
        }

        [Test]
        public void Closed_enum_value_should_return_correct_id()
        {
            ProjectStatusType.Closed.Id.ShouldEqual(2);
        }

        [Test]
        public void Should_retrieve_all_values()
        {
            _testResults.Length.ShouldEqual(3);
            _testResults.ShouldContain(ProjectStatusType.Unknown);
            _testResults.ShouldContain(ProjectStatusType.Active);
            _testResults.ShouldContain(ProjectStatusType.Closed);
        }

        [Test]
        public void Unknown_enum_should_return_correct_name()
        {
            var result = _testResults.First(x => x.Name == "Unknown");
            result.ShouldNotBeNull();
            result.Id.ShouldEqual(0);
        }

        [Test]
        public void Unknown_enum_value_should_return_correct_id()
        {
            ProjectStatusType.Unknown.Id.ShouldEqual(0);
        }
    }
}