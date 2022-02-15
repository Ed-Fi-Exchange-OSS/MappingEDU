// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectElementsService
    {
        MappingProjectElementViewModel[] GetElementIds(Guid mappingProjectId, string filter);
    }

    public class MappingProjectElementsService : IMappingProjectElementsService
    {
        private readonly IRepository<MappingProject> _mappingProjectRepository;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly EntityContext _context;

        public MappingProjectElementsService(ISystemItemRepository systemItemRepository,
            IRepository<MappingProject> mappingProjectRepository,
            EntityContext context)
        {
            _systemItemRepository = systemItemRepository;
            _mappingProjectRepository = mappingProjectRepository;
            _context = context;
        }

        public MappingProjectElementViewModel[] GetElementIds(Guid mappingProjectId, string filter)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if(!Principal.Current.IsAdministrator && !mappingProject.HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            // Mappings Ready for Review, Mappings Ready for Approval, Incomplete, Complete, Reviewed, Approved
            int workflowStatusId;
            if (int.TryParse(filter, out workflowStatusId))
            {
                return _systemItemRepository.GetAllItems()
                    .Where(si => si.MappedSystemId == mappingProject.SourceDataStandardMappedSystemId &&
                                 si.SourceSystemItemMaps.Any(
                        sim => sim.MappingProjectId == mappingProjectId) &&
                            si.SourceSystemItemMaps.FirstOrDefault(sim => sim.MappingProjectId == mappingProjectId).WorkflowStatusTypeId == workflowStatusId)
                    .Select(si => new MappingProjectElementViewModel
                    {
                        ElementId = si.SystemItemId,
                        UpdateDate = si.SourceSystemItemMaps.Any() ? si.SourceSystemItemMaps.Max(sim => sim.UpdateDate) : default(DateTime)
                    }).OrderBy(mpe => mpe.ElementId).ToArray();
            }

            var incompleteMappings = _systemItemRepository.GetAllItems()
                .Where(si => si.MappedSystemId == mappingProject.SourceDataStandardMappedSystemId &&
                             si.SourceSystemItemMaps.Any(sim => sim.MappingProjectId == mappingProjectId) &&
                             si.SourceSystemItemMaps.FirstOrDefault(sim => sim.MappingProjectId == mappingProjectId).WorkflowStatusTypeId == WorkflowStatusType.Incomplete.Id);

            var unmappedItems = _systemItemRepository.GetAllItems()
                .Where(si => si.MappedSystemId == mappingProject.SourceDataStandardMappedSystemId &&
                             (si.ItemTypeId == ItemType.Element.Id || si.ItemTypeId == ItemType.Enumeration.Id) &&
                             si.SourceSystemItemMaps.All(sim => sim.MappingProjectId != mappingProjectId));

            // Element Groups
            Guid entityId;
            if (Guid.TryParse(filter, out entityId))
            {
                return
                    _context.Database.SqlQuery<MappingProjectElementViewModel>("[GetUnmappedAndIncompleteByMappingProjectAndDomain] @MappingProjectId, @DomainId", 
                    new SqlParameter("@MappingProjectId", mappingProjectId), 
                    new SqlParameter("@DomainId", entityId)).ToArray();
            }

            switch (filter)
            {
                case "All":
                    return _systemItemRepository.GetAllItems()
                        .Where(si => si.MappedSystemId == mappingProject.SourceDataStandardMappedSystemId &&
                                     (si.ItemTypeId == ItemType.Element.Id ||
                                      si.ItemTypeId == ItemType.Enumeration.Id))
                        .Select(si => new MappingProjectElementViewModel
                        {
                            ElementId = si.SystemItemId,
                            UpdateDate = si.SourceSystemItemMaps.Any() ? si.SourceSystemItemMaps.Max(sim => sim.UpdateDate) : default(DateTime)
                        }).OrderBy(mpe => mpe.ElementId).ToArray();
                case "Unmapped":
                    return unmappedItems
                        .Select(si => new MappingProjectElementViewModel
                        {
                            ElementId = si.SystemItemId,
                            UpdateDate = si.SourceSystemItemMaps.Any() ? si.SourceSystemItemMaps.Max(sim => sim.UpdateDate) : default(DateTime)
                        }).OrderBy(mpe => mpe.ElementId).ToArray();
                case "AllIncomplete":
                    return unmappedItems
                        .Select(si => new MappingProjectElementViewModel
                        {
                            ElementId = si.SystemItemId,
                            UpdateDate = si.SourceSystemItemMaps.Any() ? si.SourceSystemItemMaps.Max(sim => sim.UpdateDate) : default(DateTime)
                        })
                        .Union(incompleteMappings.Select(si => new MappingProjectElementViewModel
                        {
                            ElementId = si.SystemItemId,
                            UpdateDate = si.SourceSystemItemMaps.Any() ? si.SourceSystemItemMaps.Max(sim => sim.UpdateDate) : default(DateTime)
                        }))
                        .OrderBy(mpe => mpe.ElementId).ToArray();
            }

            throw new NotImplementedException(string.Format("No Get Element Ids implemented for filter: {0}.", filter));
        }
    }
}