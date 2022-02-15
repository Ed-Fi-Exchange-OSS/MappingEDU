// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;

namespace MappingEdu.Core.Repositories
{
    public interface IDataStandardExportRepository
    {
        List<dynamic> GetSystemItems(Guid dataStandardId);

        List<dynamic> GetEnumerationItems(Guid dataStandardId);
    }
}