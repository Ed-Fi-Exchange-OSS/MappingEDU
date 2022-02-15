// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Core.Repositories
{
    public interface IMappingProjectRepository : IRepository<MappingProject>
    {
        MappingProject[] GetSourceMappingProjects(Guid dataStandardId);

        MappingProject[] GetTargetMappingProjects(Guid dataStandardId);

        MappingProjectSummaryViewModel[] GetSummary(Guid mappingProjectId, int? itemTypeId = null, Guid? parentSystemItemId = null);

        MappingProjectSummaryDetailViewModel[] GetSummaryDetail(Guid mappingProjectId, int? itemTypeId = null, Guid? systemItemId = null);
    }
}