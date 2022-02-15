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
using MappingEdu.Core.DataAccess.Migrations;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Util;

namespace MappingEdu.Service.SystemItems
{
    public interface IElementListService
    {
        ElementListViewModel Get(Guid mappedSystemId);
        PagedResult<ElementListViewModel.ElementPathViewModel> GetPaging(Guid mappedSystemId, ElementListDatatablesModel model);
        PagedResult<ElementListViewModel.ElementPathViewModel> GetPagingForDelta(Guid mappedSystemId, ElementListDatatablesModel model);
        MappingProjectElementViewModel[] GetElementIds(Guid mappedSystemId, ElementListDatatablesModel model);
    }

    public class ElementListService : IElementListService
    {
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IMappedSystemRepository _mappedSystemRepository;
        private readonly EntityContext _context;

        public ElementListService(ISystemItemRepository systemItemRepository, EntityContext context, IMappedSystemRepository mappedSystemRepository)
        {
            _systemItemRepository = systemItemRepository;
            _mappedSystemRepository = mappedSystemRepository;
            _context = context;
        }

        public ElementListViewModel Get(Guid mappedSystemId)
        {
            if(!Principal.Current.IsAdministrator && !_mappedSystemRepository.Get(mappedSystemId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var systemItems = _systemItemRepository.GetAllItems()
                .Where(x => x.MappedSystemId == mappedSystemId
                            && new[] {ItemType.Element.Id, ItemType.Enumeration.Id}.Contains(x.ItemTypeId)).ToList();

            return new ElementListViewModel
            {
                Elements =
                    systemItems.Select(i => new ElementListViewModel.ElementPathViewModel
                    {
                        PathSegments = PathSegmentHelper.GetPathSegments(i),
                        Element = new ElementListViewModel.ElementPathViewModel.ElementSegment
                        {
                            SystemItemId = i.SystemItemId,
                            Name = i.ItemName,
                            Definition = i.Definition,
                            TypeName = i.ItemDataType.Name,
                            ItemTypeName = i.ItemType.Name,
                            Length = i.FieldLength
                        }
                    }).ToArray()
            };
        }
        public class ListCount
        {
            public int Filtered { get; set; }
            public int Total { get; set; }
        }

        public class ElementListPage
        {
            public string Definition { get; set; }
            public string DomainItemPath { get; set; }
            public string DomainItemPathIds { get; set; }
            public string ExtensionShortName { get; set; }
            public int? FieldLength { get; set; }
            public bool IsExtended { get; set; }
            public string IsExtendedPath { get; set; }
            public string ItemDataType { get; set; }
            public int ItemTypeId { get; set; }
            public Guid SystemItemId { get; set; }
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

        public PagedResult<ElementListViewModel.ElementPathViewModel> GetPaging(Guid mappedSystemId, ElementListDatatablesModel model)
        {
            if (!Principal.Current.IsAdministrator && !_mappedSystemRepository.Get(mappedSystemId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var page = new List<ElementListPage>();
            var count = new List<ListCount>();
                    
            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "[GetElementListPage] @MappedSystemId, @SearchText, @ItemTypeIds, @DomainIds, @MappedSystemExtensionIds, @Start, @Take, @SortDirection, @OrderBy";
                if (model.IsExtended.HasValue) cmd.CommandText += ", @IsExtended";
                
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappedSystemId", Value = mappedSystemId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SearchText", Value = (model.search.value != null) ? model.search.value : "" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ItemTypes), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "DomainIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ElementGroups), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappedSystemExtensionIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.MappedSystemExtensions), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Start", Value = model.start });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Take", Value = model.length });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SortDirection", Value = model.order[0].dir });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "OrderBy", Value = model.order[0].column });
                if(model.IsExtended.HasValue) cmd.Parameters.Add(new SqlParameter { ParameterName = "IsExtended", Value = model.IsExtended.Value});


                using (var reader = cmd.ExecuteReader())
                {

                    page = ((IObjectContextAdapter)_context).ObjectContext.Translate<ElementListPage>(reader).ToList();

                    reader.NextResult();

                    count = ((IObjectContextAdapter)_context).ObjectContext.Translate<ListCount>(reader).ToList();
                }
            }

            var items = new List<ElementListViewModel.ElementPathViewModel>();
            foreach (var result in page)
            {
                var splitNames = result.DomainItemPath.Split('.');
                var splitIds = result.DomainItemPathIds.Split('/');
                var splitIsExtended = result.IsExtendedPath.Split('/');

                var segments = splitIds.Select((t, i) => new ElementListViewModel.ElementPathViewModel.PathSegment
                {
                    Name = splitNames[i],
                    SystemItemId = new Guid(t),
                    IsExtended = (splitIsExtended[i] == "1"),
                    ExtensionShortName = (splitIsExtended[i] == "1" && result.ExtensionShortName != null) ? result.ExtensionShortName : ""
                }).ToList();

                var last = segments.Last();
                segments.Remove(segments.Last());

                items.Add(new ElementListViewModel.ElementPathViewModel
                {
                    Element = new ElementListViewModel.ElementPathViewModel.ElementSegment
                    {
                        SystemItemId = last.SystemItemId,
                        Definition = result.Definition,
                        Name = last.Name,
                        ItemTypeName = ItemType.GetById(result.ItemTypeId).Name,
                        TypeName = result.ItemDataType,
                        Length = result.FieldLength,
                        IsExtended = result.IsExtended,
                        ExtensionShortName = (result.IsExtended && result.ExtensionShortName != null) ? result.ExtensionShortName : ""
                    },
                    PathSegments = segments.ToArray(),
                    SystemItemId = result.SystemItemId
                });
            }


            return new PagedResult<ElementListViewModel.ElementPathViewModel>()
            {
                Items = items,
                TotalFiltered = count.First().Filtered,
                TotalRecords = count.First().Total
            };
        }

        public class ElementListForDeltaPage
        {
            public string DomainItemPath { get; set; }
            public string DomainItemPathIds { get; set; }
            public string ItemType { get; set; }
            public Guid SystemItemId { get; set; }
        }

        public PagedResult<ElementListViewModel.ElementPathViewModel> GetPagingForDelta(Guid mappedSystemId, ElementListDatatablesModel model)
        {
            if (!Principal.Current.IsAdministrator && !_mappedSystemRepository.Get(mappedSystemId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var page = new List<ElementListForDeltaPage>();
            var count = new List<ListCount>();

            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "[GetElementListForDeltaPage] @MappedSystemId, @SearchText, @ItemTypeIds, @DomainIds, @Start, @Take, @SortDirection, @OrderBy";

                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappedSystemId", Value = mappedSystemId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SearchText", Value = (model.search.value != null) ? model.search.value : "" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ItemTypes), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "DomainIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ElementGroups), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Start", Value = model.start });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Take", Value = model.length });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SortDirection", Value = model.order[0].dir });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "OrderBy", Value = model.order[0].column });

                using (var reader = cmd.ExecuteReader())
                {

                    page = ((IObjectContextAdapter)_context).ObjectContext.Translate<ElementListForDeltaPage>(reader).ToList();

                    reader.NextResult();

                    count = ((IObjectContextAdapter)_context).ObjectContext.Translate<ListCount>(reader).ToList();
                }
            }

            var items = new List<ElementListViewModel.ElementPathViewModel>();
            foreach (var result in page)
            {
                var splitNames = result.DomainItemPath.Split('.');
                var splitIds = result.DomainItemPathIds.Split('/');

                var segments = splitIds.Select((t, i) => new ElementListViewModel.ElementPathViewModel.PathSegment
                {
                    Name = splitNames[i],
                    SystemItemId = new Guid(t)
                }).ToList();

                var last = segments.Last();
                segments.Remove(segments.Last());

                items.Add(new ElementListViewModel.ElementPathViewModel
                {
                    Element = new ElementListViewModel.ElementPathViewModel.ElementSegment
                    {
                        SystemItemId = last.SystemItemId,
                        Name = last.Name,
                        ItemTypeName = result.ItemType
                    },
                    PathSegments = segments.ToArray(),
                    SystemItemId = result.SystemItemId
                });
            }


            return new PagedResult<ElementListViewModel.ElementPathViewModel>()
            {
                Items = items,
                TotalFiltered = count.First().Filtered,
                TotalRecords = count.First().Total
            };
        }

        public MappingProjectElementViewModel[] GetElementIds(Guid mappedSystemId, ElementListDatatablesModel model)
        {
            if (!Principal.Current.IsAdministrator && !_mappedSystemRepository.Get(mappedSystemId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");
            
            var page = new List<ElementListPage>();

            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "[GetElementListPage] @MappedSystemId, @SearchText, @ItemTypeIds, @DomainIds, @MappedSystemExtensionIds, @Start, @Take, @SortDirection, @OrderBy";
                if (model.IsExtended.HasValue) cmd.CommandText += ", @IsExtended";

                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappedSystemId", Value = mappedSystemId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SearchText", Value = (model.search.value != null) ? model.search.value : "" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ItemTypes), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "DomainIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ElementGroups), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappedSystemExtensionIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.MappedSystemExtensions), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Start", Value = 0 });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Take", Value = "All" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SortDirection", Value = model.order[0].dir });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "OrderBy", Value = model.order[0].column });
                if (model.IsExtended.HasValue) cmd.Parameters.Add(new SqlParameter { ParameterName = "IsExtended", Value = model.IsExtended.Value });


                using (var reader = cmd.ExecuteReader())
                {
                    page = ((IObjectContextAdapter)_context).ObjectContext.Translate<ElementListPage>(reader).ToList();
                }
            }

            return page.Select(x => new MappingProjectElementViewModel { ElementId = x.SystemItemId }).ToArray();
        }
    }
}