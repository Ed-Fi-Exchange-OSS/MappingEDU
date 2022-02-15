// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.Domain.System;
using MappingEdu.Service.Model.ElementDetails;

namespace MappingEdu.Core.Repositories
{
    public interface ISystemItemRepository : IRepository<SystemItem>
    {
        SystemItem[] GetWhere(Guid mappedSystemId, Guid? parentSystemItemId);

        SystemItem GetWithTreeLoaded(Guid mappedSystemId, Guid id);

        IQueryable<SystemItem> GetAllItems();

        IEnumerable<ElementDetailsSearchModel> GetAllMatchmakerItems(Guid mappedSystemId, int itemTypeId);

        IEnumerable<ElementDetailsSearchModel> GetAllForComparison(Guid mappedSystemId);
    }
}