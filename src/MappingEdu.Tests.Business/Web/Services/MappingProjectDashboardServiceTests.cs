// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;
using MappingEdu.Tests.Business.Bases;
using MappingEdu.Tests.Business.Builders;
using NUnit.Framework;

namespace MappingEdu.Tests.Business.Web.Services
{
    public class MappingProjectDashboardServiceTests
    {
        [TestFixture]
        public class when_getting_a_mapping_project_dashboard : TestBase
        {
            private readonly Guid _mappingProjectId = Guid.NewGuid();

            [OneTimeSetUp]
            public void Setup()
            {
                SystemItemMap systemItemMap = New.SystemItemMap.WithMappingProject(
                    New.MappingProject.IsActive(true).WithId(_mappingProjectId)
                        .WithSource(
                            New.MappedSystem.IsActive(true)
                                .WithSystemName("Source Test")
                                .WithSystemVersion("1.0"))
                        .WithTarget(
                            New.MappedSystem.IsActive(true)
                                .WithSystemName("Target Test")
                                .WithSystemVersion("1.0")
                                .WithSystemItem(
                                    New.SystemItem.IsActive(true)
                                        .WithName("Test Target Element"))))
                    .WithCompleteStatusType(CompleteStatusType.Incomplete.Id.Value);
            }
        }
    }
}