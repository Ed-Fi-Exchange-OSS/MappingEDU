// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.SystemItem;
using MappingEdu.Service.Providers;

namespace MappingEdu.Service.SystemItems
{
    public interface IElementSearchService
    {
        ElementDetailsSearchModel[] SearchElements(string searchText, int itemTypeId, Guid dataStandardId);

        SystemItemTypeaheadViewModel[] SearchElements(Guid dataStandardId, Guid? parentSystemItemId = null, string searchText = null, string domainItemPath = null);

        PagedResult<ElementDetailsSearchModel> SearchElementsPaging(Guid targetDataStandardId, SystemItemSearchDatatablesModel model);

    }

    public class ElementSearchService : IElementSearchService
    {
        private readonly ILoggingProvider<ElementSearchService> _logger;
        private readonly IMapper _mapper;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly EntityContext _context;


        public ElementSearchService(ISystemItemRepository systemItemRepository, IMapper mapper, ILoggingProvider<ElementSearchService> logger, IMappedSystemRepository mappedSystemRepository, EntityContext context)
        {
            _systemItemRepository = systemItemRepository;
            _mappedSystemRepository = mappedSystemRepository;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        public ElementDetailsSearchModel[] SearchElements(
            string searchText, int itemTypeId, Guid targetDataStandardId)
        {
            if(!Principal.Current.IsAdministrator && !_mappedSystemRepository.Get(targetDataStandardId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            _logger.LogDebug("Searching for elements");
            var systemItems = _systemItemRepository.GetAllMatchmakerItems(targetDataStandardId, itemTypeId).ToArray();
            return systemItems;
        }

        public SystemItemTypeaheadViewModel[] SearchElements(Guid dataStandardId, Guid? parentSystemItemId = null, string searchText = null, string domainItemPath = null)
        {
            var standard = _mappedSystemRepository.Get(dataStandardId);

            if (!Principal.Current.IsAdministrator && !standard.HasAccess(MappedSystemUser.MappedSystemUserRole.View))
                throw new SecurityException("User needs at least View Access to peform this action");

            if (searchText == null) searchText = "";

            if (!parentSystemItemId.HasValue && domainItemPath != null)
            {
                var splitPath = domainItemPath.ToLower().Split('.');

                var items = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && x.ItemTypeId == 1 && x.MappedSystemId == dataStandardId && !x.MappedSystemExtensionId.HasValue);

                for (var i = 0; i < splitPath.Length - 1; i++)
                {
                    var itemName = splitPath[i];
                    var item = items.FirstOrDefault(x => x.ItemName.ToLower() == itemName);

                    if (item == null && i < splitPath.Length - 1) 
                        throw new NotFoundException("Unable to find System Item with name " + itemName);

                    if(item != null && i < splitPath.Length - 1)
                        items = item.ChildSystemItems.Where(x => !x.MappedSystemExtensionId.HasValue).AsQueryable();

                    if(item != null)
                        parentSystemItemId = item.SystemItemId;
                }
            }

            IQueryable<SystemItem> systemItems;

            if (!parentSystemItemId.HasValue) systemItems = _systemItemRepository.GetAllQueryable().Where(x => x.IsActive && x.ItemTypeId == 1 && x.MappedSystemId == dataStandardId && !x.MappedSystemExtensionId.HasValue);
            else
            {
                var parent = _systemItemRepository.GetAllQueryable().FirstOrDefault(x => x.SystemItemId == parentSystemItemId.Value && !x.MappedSystemExtensionId.HasValue);

                if (parent == null)
                    throw new NotFoundException("Unable to find System Item with Id of " + parentSystemItemId);

                systemItems = parent.ChildSystemItems.Where(x => !x.MappedSystemExtensionId.HasValue).AsQueryable();
            }

            var returnItems = systemItems.Where(x => !x.MappedSystemExtensionId.HasValue && x.ItemName.ToLower().StartsWith(searchText.ToLower()) || x.ItemName.ToLower() == searchText || searchText == "")
                .OrderBy(x => x.ItemName)
                .Select(x => new SystemItemTypeaheadViewModel()
                        {
                            ItemName = x.ItemName,
                            ItemTypeId = x.ItemTypeId,
                            ParentSystemItemId = x.ParentSystemItemId,
                            SystemItemId = x.SystemItemId
                        }).Take(10).ToArray();

            return returnItems;
        }

        public class ListCount
        {
            public int Filtered { get; set; }
            public int Total { get; set; }
        }

        private DataTable CreateDataTable<T>(T[] ids)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(T));

            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var row = dataTable.NewRow();
                    row.SetField("Id", id);
                    dataTable.Rows.Add(row);
                }
            }

            return dataTable; ;
        }

        public PagedResult<ElementDetailsSearchModel> SearchElementsPaging(Guid targetDataStandardId, SystemItemSearchDatatablesModel model)
        {
            if (!Principal.Current.IsAdministrator && !_mappedSystemRepository.Get(targetDataStandardId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            if (model.ItemTypes == null || model.ItemTypes.Length == 0)
                model.ItemTypes = new [] {4, 5};

            var page = new List<ElementDetailsSearchModel>();
            var count = new List<ListCount>();

            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "[GetMatchmakerSearchPage] @MappedSystemId, @SearchText, @ItemTypeIds, @ItemDataTypeIds, @DomainIds, @EntityIds, @Start, @Take, @SortDirection, @OrderBy";
                if (model.IsExtended.HasValue) cmd.CommandText += ", @IsExtended";

                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappedSystemId", Value = targetDataStandardId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SearchText", Value = (model.search.value != null) ? model.search.value : "" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ItemTypes), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemDataTypeIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ItemDataTypes), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "DomainIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ElementGroups), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "EntityIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.Entities), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Start", Value = model.start });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Take", Value = model.length });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SortDirection", Value = model.order[0].dir });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "OrderBy", Value = model.order[0].column });
                if (model.IsExtended.HasValue) cmd.Parameters.Add(new SqlParameter { ParameterName = "IsExtended", Value = model.IsExtended.Value });

                using (var reader = cmd.ExecuteReader())
                {
                    page = ((IObjectContextAdapter)_context).ObjectContext.Translate<ElementDetailsSearchModel>(reader).ToList();
                    reader.NextResult();
                    count = ((IObjectContextAdapter)_context).ObjectContext.Translate<ListCount>(reader).ToList();
                }
            }

            return new PagedResult<ElementDetailsSearchModel>()
            {
                Items = page,
                TotalFiltered = count.First().Filtered,
                TotalRecords = count.First().Total
            };
        }

        private SystemItem[] GetNameMatchItems(
            int itemTypeId, Guid targetDataStandardId, string elementName, string entityName, string domainName)
        {
            if (domainName.Length > 0)
                return _systemItemRepository.GetAllItems()
                    .Where(
                        si => si.MappedSystemId == targetDataStandardId &&
                              si.ItemName.Contains(elementName) && si.ParentSystemItem.ItemName.Contains(entityName) && si.ParentSystemItem.ParentSystemItem != null && si.ParentSystemItem.ParentSystemItem.ItemName.Contains(domainName) && si.ItemTypeId == itemTypeId).ToArray();

            if (entityName.Length > 0)
                return _systemItemRepository.GetAllItems()
                    .Where(
                        si => si.MappedSystemId == targetDataStandardId &&
                              si.ItemName.Contains(elementName) && si.ParentSystemItem.ItemName.Contains(entityName) &&
                              si.ItemTypeId == itemTypeId).ToArray();

            return _systemItemRepository.GetAllItems()
                .Where(
                    si => si.MappedSystemId == targetDataStandardId &&
                          (si.ItemName.Contains(elementName) || si.Definition.Contains(elementName)) &&
                          si.ItemTypeId == itemTypeId).ToArray();
        }

        private SystemItem[] GetSpacesRemovedMatchItems(
            int itemTypeId, Guid targetDataStandardId, string elementName, string entityName)
        {
            var elementNoSpace = elementName.Replace(" ", string.Empty);
            var entityNoSpace = entityName.Replace(" ", string.Empty);
            return _systemItemRepository.GetAllItems()
                .Where(
                    si => si.MappedSystemId == targetDataStandardId &&
                          si.ItemName.Contains(elementNoSpace) &&
                          si.ParentSystemItem.ItemName.Contains(entityNoSpace) &&
                          si.ItemTypeId == itemTypeId).ToArray();
        }
    }
}