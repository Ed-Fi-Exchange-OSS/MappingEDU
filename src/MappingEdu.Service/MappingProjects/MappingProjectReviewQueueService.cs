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
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Model.SystemItemMapping;
using ItemDataType = MappingEdu.Core.Domain.Enumerations.ItemDataType;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectReviewQueueService
    {
        MappingProjectReviewQueueViewModel Get(Guid mappingProjectId);
        PagedResult<MappingProjectReviewQueueViewModel.ReviewItemViewModel> GetPaging(Guid mappingProjectId, ReviewQueueDatatablesModel model);
        MappingProjectElementViewModel[] GetElementIds(Guid mappingProjectId, ReviewQueueDatatablesModel model);
    }

    public class MappingProjectReviewQueueService : IMappingProjectReviewQueueService
    {
        private readonly IMapper _mapper;
        private readonly IMappingProjectRepository _mappingProjectRepository;
        private readonly ISystemItemMapRepository _systemItemMapRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly EntityContext _context;

        public MappingProjectReviewQueueService(IMappingProjectRepository mappingProjectRepository,
            ISystemItemRepository systemItemRepository, ISystemItemMapRepository systemItemMapRepository,
            IMapper mapper, EntityContext context)
        {
            _mappingProjectRepository = mappingProjectRepository;
            _systemItemRepository = systemItemRepository;
            _systemItemMapRepository = systemItemMapRepository;
            _mapper = mapper;
            _context = context;
        }

        private ElementListViewModel.ElementPathViewModel.PathSegment[] GetPathSegments(ICollection<ElementDetailsSearchModel> systemItems, string domainItemPath)
        {
            var segments = domainItemPath.Split('.');
            var pathSegments = new List<ElementListViewModel.ElementPathViewModel.PathSegment>();
            var searchString = "";
            for (var i = 0; i < segments.Length - 1; i++)
            {
                searchString += segments[i];
                while (systemItems.FirstOrDefault(x => x.DomainItemPath == searchString.ToString()) == null)
                {
                    i++;
                    if (i == segments.Length - 1) return pathSegments.ToArray();
                    searchString += ".";
                    searchString += segments[i];

                }
                var systemItem = systemItems.FirstOrDefault(x => x.DomainItemPath == searchString.ToString());
                pathSegments.Add(new ElementListViewModel.ElementPathViewModel.PathSegment
                {
                    Name = segments[i],
                    SystemItemId = systemItem.SystemItemId,
                    Definition = systemItem.Definition
                });
                searchString += ".";
            }
            return pathSegments.ToArray();
        }

        public MappingProjectReviewQueueViewModel Get(Guid mappingProjectId)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var hasViewAccess = Principal.Current.IsAdministrator || mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View);

            var systemItems = _systemItemRepository.GetAllForComparison(mappingProject.SourceDataStandardMappedSystemId).ToArray();
            var elements = systemItems.Where(x => new[] { ItemType.Enumeration.Id, ItemType.Element.Id }.Contains(x.ItemTypeId));
            var systemItemMaps = _systemItemMapRepository.GetAllForComparison(mappingProjectId).ToList();

            return new MappingProjectReviewQueueViewModel
            {
                MappingProjectId = mappingProjectId,
                ReviewItems = elements.Select(i => new MappingProjectReviewQueueViewModel.ReviewItemViewModel
                {
                    PathSegments = GetPathSegments(systemItems, i.DomainItemPath),
                    SystemItemId = i.SystemItemId,
                    Element = new ElementListViewModel.ElementPathViewModel.ElementSegment
                    {
                        SystemItemId = i.SystemItemId,
                        Name = i.DomainItemPath.Split('.').Last(),
                        Definition = i.Definition,
                        TypeName = ItemDataType.GetById(i.ItemDataTypeId).Name,
                        ItemTypeName = ItemType.GetById(i.ItemTypeId).Name,
                        Length = i.FieldLength
                    },
                    Mapping = _mapper.Map<SystemItemMappingBriefViewModel>
                        (systemItemMaps.Where(x => x.SourceSystemItemId == i.SystemItemId)
                                       .Select(x =>{x.WorkflowStatusTypeId = (hasViewAccess) ? x.WorkflowStatusTypeId : 0;
                                                    x.StatusNote = (hasViewAccess) ? x.StatusNote: null;
                                                    return x; })
                                       .FirstOrDefault())
                }).ToArray()
            };
        }

        public class QueueCount
        {
            public int Filtered { get; set; }
            public int Total { get; set; }
        }

        public class ReviewQueuePage
        {
            public string CreateBy { get; set; }
            public DateTime? CreateDate { get; set; }
            public string Definition { get; set; }
            public string DomainItemPath { get; set; }
            public string DomainItemPathIds { get; set; }
            public bool IsExtended { get; set; }
            public bool? IsAutoMapped { get; set; }
            public string IsExtendedPath { get; set; }
            public int ItemTypeId { get; set; }
            public Guid SystemItemId { get; set; }
            public string Logic { get; set; }
            public bool? Flagged { get; set; }
            public int WorkflowStatusTypeId { get; set; }
            public int? MappingMethodTypeId { get; set; }
            public Guid? SystemItemMapId { get; set; }
            public int MappedEnumerations { get; set; }
            public int TotalEnumerations { get; set; }
            public string UpdateBy { get; set; }
            public DateTime? UpdateDate { get; set; }
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

        public PagedResult<MappingProjectReviewQueueViewModel.ReviewItemViewModel> GetPaging(Guid mappingProjectId, ReviewQueueDatatablesModel model)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var hasViewAccess = Principal.Current.IsAdministrator || mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.View);
            var hasEditAccess = Principal.Current.IsAdministrator || mappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit);

            var page = new List<ReviewQueuePage>();
            var count = new List<QueueCount>();

            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"[GetReviewQueuePage] @MappingProjectId,
                                                         @SearchText,
                                                         @ItemTypeIds,
                                                         @WorkflowStatusIds,
                                                         @Flagged,
                                                         @Unflagged,
			                                             @AutoMapped,
			                                             @UserMapped,
			                                             @Extended,
			                                             @Base,
                                                         @DomainIds,
                                                         @CreateByIds, 
                                                         @UpdateByIds,
                                                         @MethodIds,
                                                         @OrderBy,
                                                         @SortDirection,
                                                         @Start,
                                                         @Take";
                

                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingProjectId", Value = mappingProjectId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SearchText", Value = (model.search.value != null) ? model.search.value : "" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ItemTypes), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "WorkflowStatusIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.WorkflowStatuses), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Flagged", Value = model.Flagged });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Unflagged", Value = model.Unflagged });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "AutoMapped", Value = model.AutoMapped });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "UserMapped", Value = model.UserMapped });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Extended", Value = model.Extended });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Base", Value = model.Base });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "DomainIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ElementGroups), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "CreateByIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.CreatedByUserIds), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "UpdateByIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.UpdatedByUserIds), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MethodIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.MappingMethods), TypeName = "dbo.IntId"});
                cmd.Parameters.Add(new SqlParameter { ParameterName = "OrderBy", Value = model.order[0].column });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SortDirection", Value = model.order[0].dir });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Start", Value = model.start });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Take", Value = model.length });

                using (var reader = cmd.ExecuteReader())
                {

                    page = ((IObjectContextAdapter)_context).ObjectContext.Translate<ReviewQueuePage>(reader).ToList();

                    reader.NextResult();

                    count = ((IObjectContextAdapter)_context).ObjectContext.Translate<QueueCount>(reader).ToList();
                }
            }

            var items = new List<MappingProjectReviewQueueViewModel.ReviewItemViewModel>();
            foreach (var result in page)
            {
                var splitNames = result.DomainItemPath.Split('.');
                var splitIds = result.DomainItemPathIds.Split('/');
                var splitExtendedPath = result.IsExtendedPath.Split('/');

                var segments = splitIds.Select((t, i) => new ElementListViewModel.ElementPathViewModel.PathSegment
                {
                    Name = splitNames[i],
                    SystemItemId = new Guid(t),
                    IsExtended = (splitExtendedPath[i] == "1")
                }).ToList();

                var last = segments.Last();
                segments.Remove(segments.Last());

                items.Add(new MappingProjectReviewQueueViewModel.ReviewItemViewModel
                {
                    Mapping = (result.MappingMethodTypeId.HasValue) ? new SystemItemMappingBriefViewModel
                    {
                        BusinessLogic = (result.MappingMethodTypeId != 4) ? result.Logic : null,
                        CreateDate = (hasEditAccess) ? result.CreateDate : null,
                        CreateBy = (hasEditAccess) ? result.CreateBy : null,
                        OmissionReason = (result.MappingMethodTypeId == 4) ? result.Logic : null,
                        MappingMethodTypeId = result.MappingMethodTypeId.Value,
                        WorkflowStatusTypeId = (hasViewAccess) ? result.WorkflowStatusTypeId : 0,
                        Flagged = result.Flagged,
                        IsAutoMapped = (result.IsAutoMapped.HasValue) ? result.IsAutoMapped.Value : false,
                        SystemItemMapId = result.SystemItemMapId.Value,
                        SourceSystemItemId = result.SystemItemId,
                        UpdateDate = (hasEditAccess) ? result.UpdateDate : null,
                        UpdateBy = (hasEditAccess) ? result.UpdateBy : null
                    } : null,
                    Element = new ElementListViewModel.ElementPathViewModel.ElementSegment
                    {
                        SystemItemId = last.SystemItemId,
                        Definition = result.Definition,
                        Name = last.Name,
                        IsExtended = last.IsExtended,
                        ItemTypeName = ItemType.GetById(result.ItemTypeId).Name,
                    },
                    PathSegments = segments.ToArray(),
                    SystemItemId = result.SystemItemId,
                    MappedEnumerations = result.MappedEnumerations,
                    TotalEnumerations = result.TotalEnumerations,
                });
            }


            return new PagedResult<MappingProjectReviewQueueViewModel.ReviewItemViewModel>()
            {
                Items = items,
                TotalFiltered = count.First().Filtered,
                TotalRecords = count.First().Total
            };
        }

        public MappingProjectElementViewModel[] GetElementIds(Guid mappingProjectId, ReviewQueueDatatablesModel model)
        {
            if (!Principal.Current.IsAdministrator) {
                var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
                if (!mappingProject.HasAccess())
                    throw new SecurityException("User needs at least Guest Access to perform this action");
            }

            var page = new List<ReviewQueuePage>();

            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"[GetReviewQueuePage] @MappingProjectId,
                                                         @SearchText,
                                                         @ItemTypeIds,
                                                         @WorkflowStatusIds,
                                                         @Flagged,
                                                         @Unflagged,
			                                             @AutoMapped,
			                                             @UserMapped,
			                                             @Extended,
			                                             @Base,
                                                         @DomainIds,
                                                         @CreateByIds, 
                                                         @UpdateByIds,
                                                         @MethodIds,
                                                         @OrderBy,
                                                         @SortDirection,
                                                         @Start,
                                                         @Take";


                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingProjectId", Value = mappingProjectId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SearchText", Value = (model.search.value != null) ? model.search.value : "" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "ItemTypeIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ItemTypes), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "WorkflowStatusIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.WorkflowStatuses), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Flagged", Value = model.Flagged });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Unflagged", Value = model.Unflagged });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "AutoMapped", Value = model.AutoMapped });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "UserMapped", Value = model.UserMapped });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Extended", Value = model.Extended });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Base", Value = model.Base });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "DomainIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.ElementGroups), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "CreateByIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.CreatedByUserIds), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "UpdateByIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.UpdatedByUserIds), TypeName = "dbo.UniqueIdentiferId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MethodIds", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(model.MappingMethods), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "OrderBy", Value = model.order[0].column });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "SortDirection", Value = model.order[0].dir });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Start", Value = 0 });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "Take", Value = "All" });

                using (var reader = cmd.ExecuteReader())
                {
                    page = ((IObjectContextAdapter)_context).ObjectContext.Translate<ReviewQueuePage>(reader).ToList();
                }
            }

            return page.Select(x => new MappingProjectElementViewModel { ElementId = x.SystemItemId }).ToArray();
        }
    }
}