// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.Domain.System;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Core.Repositories
{
    public interface ISystemItemMapRepository : IRepository<SystemItemMap>
    {
        SystemItemMap[] GetByMappingProject(Guid mappingProjectId);

        IQueryable<SystemItemMap> GetAllMaps();

        void SaveChangesWithoutValidation();

        SystemItemMap[] GetAllForComparison(Guid mappingProjectId);
    }
}