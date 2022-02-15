// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MappingEdu.Core.Repositories
{
    public interface IMappingProjectReportRepository
    {
        List<dynamic> GetElementList(Guid dataStandardId);

        List<dynamic> GetEnumerationItems(Guid dataStandardId);

        List<dynamic> GetSourceEnumerationItemMaps(Guid mappingProjectId, ICollection<int> enumerationStatuses, ICollection<int> enumerationStatusReasons, bool includeCustomDetails);

        List<dynamic> GetTargetEnumerationItemMaps(Guid mappingProjectId, ICollection<int> enumerationStatuses, ICollection<int> enumerationStatusReasons, bool includeCustomDetails);

        List<dynamic> GetSourceMappings(Guid mappingProjectId, ICollection<int> mappingMethods, ICollection<int> workflowStatuses, bool includeCustomDetails, bool includeTargetItems);

        List<dynamic> GetTargetMappings(Guid mappingProjectId, ICollection<int> mappingMethods, ICollection<int> workflowStatuses, bool includeCustomDetails);
    }
}