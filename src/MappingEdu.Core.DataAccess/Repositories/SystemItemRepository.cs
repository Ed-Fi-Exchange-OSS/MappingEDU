// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class SystemItemRepository : Repository<SystemItem>, ISystemItemRepository
    {
        public SystemItemRepository(EntityContext databaseContext) : base(databaseContext)
        {
        }

        public override SystemItem[] GetAll()
        {
            var entities = _databaseContext.SystemItems.Where(x => x.IsActive);
            return entities.ToArray();
        }

        public IQueryable<SystemItem> GetAllItems()
        {
            return _databaseContext.SystemItems.Where(x => x.IsActive);
        }

        public IEnumerable<ElementDetailsSearchModel> GetAllMatchmakerItems(Guid mappedSystemId, int itemTypeId)
        {
            return _databaseContext.SystemItems.Where(x => x.ItemTypeId == itemTypeId && x.MappedSystemId == mappedSystemId && !x.MappedSystemExtensionId.HasValue)
                .Select(x => new ElementDetailsSearchModel
                {
                    DataTypeSource = x.DataTypeSource,
                    Definition = x.Definition,
                    DomainId = x.ElementGroupSystemItemId.Value,
                    DomainItemPath = x.DomainItemPath,
                    FieldLength = x.FieldLength,
                    IsExtended = x.IsExtended,
                    ItemDataTypeId = x.ItemDataTypeId,
                    ItemName = x.ItemName,
                    ItemUrl = x.ItemUrl,
                    ItemTypeId = x.ItemTypeId,
                    SystemItemId = x.SystemItemId,
                    TechnicalName = x.TechnicalName
                });
        }

        public IEnumerable<ElementDetailsSearchModel> GetAllForComparison(Guid mappedSystemId)
        {
            return _databaseContext.SystemItems.Where(x => x.MappedSystemId == mappedSystemId && !x.MappedSystemExtensionId.HasValue)
                .Select(x => new ElementDetailsSearchModel
                {
                    DataTypeSource = x.DataTypeSource,
                    Definition = x.Definition,
                    DomainId = x.ElementGroupSystemItemId.Value,
                    DomainItemPath = x.DomainItemPath,
                    FieldLength = x.FieldLength,
                    IsExtended = x.IsExtended,
                    ItemDataTypeId = x.ItemDataTypeId,
                    ItemName = x.ItemName,
                    ItemUrl = x.ItemUrl,
                    ItemTypeId = x.ItemTypeId,
                    SystemItemId = x.SystemItemId,
                    TechnicalName = x.TechnicalName
                });
        }

        public SystemItem[] GetWhere(Guid mappedSystemId, Guid? parentSystemItemId)
        {
            return _databaseContext.SystemItems.Where(x => x.MappedSystemId == mappedSystemId
                                                           && x.ParentSystemItemId == parentSystemItemId
                                                           && x.IsActive).ToArray();
        }

        public SystemItem GetWithTreeLoaded(Guid mappedSystemId, Guid id)
        {
            _databaseContext.Configuration.AutoDetectChangesEnabled = false;
            _databaseContext.Configuration.LazyLoadingEnabled = false;
            _databaseContext.SystemItems.Where(x => x.MappedSystemId == mappedSystemId).Load();
            var systemItem = _databaseContext.SystemItems.Local.FirstOrDefault(x => x.SystemItemId == id);
            return systemItem;
        }
    }
}