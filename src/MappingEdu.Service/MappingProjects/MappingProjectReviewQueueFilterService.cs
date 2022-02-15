// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using MappingEdu.Common.Exceptions;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Service.MappingProjects
{
    public interface IMappingProjectQueueFilterService
    {
        List<MappingProjectQueueFilterViewModel> GetAll(Guid? mappingProjectId = null);

        MappingProjectQueueFilterViewModel Get(Guid mappingProjetQueueFilterId);

        MappingProjectQueueFilterDashboardModel[] GetDashboard(Guid mappingProjectId);

        MappingProjectQueueFilterViewModel Post(Guid mappingProjectId, MappingProjectQueueFilterCreateModel model);

        MappingProjectQueueFilterViewModel Put(Guid mappingProjectQueueFilterId, Guid mappingProjectId, MappingProjectQueueFilterEditModel model);

        void Delete(Guid mappingProjectQueueFilterId);
    }

    public class MappingProjectQueueFilterService : IMappingProjectQueueFilterService
    {
        private readonly IRepository<MappingProjectQueueFilter> _mappingProjectQueueFilterRepository;
        private readonly EntityContext _context;
        private readonly ISystemItemRepository _systemItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MappingProjectQueueFilterService(IRepository<MappingProjectQueueFilter> mappingProjectQueueFilterRepository, EntityContext context, IMapper mapper, ISystemItemRepository systemItemRepository, IUserRepository userRepository)
        {
            _mappingProjectQueueFilterRepository = mappingProjectQueueFilterRepository;
            _context = context;
            _mapper = mapper;
            _systemItemRepository = systemItemRepository;
            _userRepository = userRepository;
        }

        public List<MappingProjectQueueFilterViewModel> GetAll(Guid? mappingProjectId = null)
        {
            var currentUserId = new Guid(Principal.Current.UserId);
            var query = _mappingProjectQueueFilterRepository.GetAllQueryable().Where(x => x.UserId == currentUserId);
            if (mappingProjectId != null)
                query = query.Where(x => x.MappingProjectId == mappingProjectId.Value);

            var filters = query.ToList().Select(filter => new MappingProjectQueueFilterViewModel
            {
                AutoMapped = filter.AutoMapped,
                Base = filter.Base,
                CreatedByColumn = filter.CreatedByColumn,
                CreationDateColumn = filter.CreationDateColumn,
                CreatedByUserIds = filter.CreatedByUsers.Select(x => new Guid(x.Id)).ToArray(),
                Extended = filter.Extended,
                Flagged = filter.Flagged,
                ItemTypes = filter.ItemTypes.Select(x => x.ItemTypeId).ToArray(),
                Length = filter.Length,
                MappingProjectId = filter.MappingProjectId,
                MappedByColumn = filter.MappedByColumn,
                MappingMethods = filter.MappingMethodTypes.Select(x => x.MappingMethodTypeId).ToArray(),
                MappingProjectQueueFilterId = filter.MappingProjectQueueFilterId,
                Name = filter.Name,
                OrderColumn = filter.OrderColumn,
                OrderDirection = filter.OrderDirection,
                ElementGroups = filter.ParentSystemItems.Select(x => x.SystemItemId).ToArray(),
                Search = filter.Search,
                ShowInDashboard = filter.ShowInDashboard,
                Unflagged = filter.Unflagged,
                UpdatedByUserIds = filter.UpdatedByUsers.Select(x => new Guid(x.Id)).ToArray(),
                UpdatedByColumn = filter.UpdatedByColumn,
                UpdateDateColumn = filter.UpdateDateColumn,
                WorkflowStatuses = filter.WorkflowStatusTypes.Select(x => x.WorkflowStatusTypeId).ToArray(),
            }).ToList();

            return filters;
        }

        public MappingProjectQueueFilterViewModel Get(Guid mappingProjectQueueFilterId)
        {
            var queueFilter = _mappingProjectQueueFilterRepository.GetAllQueryable().FirstOrDefault(x => x.MappingProjectQueueFilterId == mappingProjectQueueFilterId);

            if(queueFilter == null)
                throw new NotFoundException("Unable to find Mapping Project Queue Filter with Id " + mappingProjectQueueFilterId);

            var viewModel = new MappingProjectQueueFilterViewModel
            {
                AutoMapped = queueFilter.AutoMapped,
                Base = queueFilter.Base,
                CreatedByColumn = queueFilter.CreatedByColumn,
                CreationDateColumn = queueFilter.CreationDateColumn,
                CreatedByUserIds = queueFilter.CreatedByUsers.Select(x => new Guid(x.Id)).ToArray(),
                Extended = queueFilter.Extended,
                Flagged = queueFilter.Flagged,
                ItemTypes = queueFilter.ItemTypes.Select(x => x.ItemTypeId).ToArray(),
                Length = queueFilter.Length,
                MappingProjectId = queueFilter.MappingProjectId,
                MappedByColumn = queueFilter.MappedByColumn,
                MappingMethods = queueFilter.MappingMethodTypes.Select(x => x.MappingMethodTypeId).ToArray(),
                MappingProjectQueueFilterId = queueFilter.MappingProjectQueueFilterId,
                Name = queueFilter.Name,
                OrderColumn = queueFilter.OrderColumn,
                OrderDirection = queueFilter.OrderDirection,
                ElementGroups = queueFilter.ParentSystemItems.Select(x => x.SystemItemId).ToArray(),
                Search = queueFilter.Search,
                ShowInDashboard = queueFilter.ShowInDashboard,
                Unflagged = queueFilter.Unflagged,
                UpdatedByUserIds = queueFilter.UpdatedByUsers.Select(x => new Guid(x.Id)).ToArray(),
                UpdatedByColumn = queueFilter.UpdatedByColumn,
                UpdateDateColumn = queueFilter.UpdateDateColumn,
                WorkflowStatuses = queueFilter.WorkflowStatusTypes.Select(x => x.WorkflowStatusTypeId).ToArray(),
            };

            return viewModel;
        }

        public MappingProjectQueueFilterDashboardModel[] GetDashboard(Guid mappingProjectId)
        {
            var filters = new List<MappingProjectQueueFilterDashboardModel>();

            using (var connection = _context.Database.Connection)
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "[GetDashboardQueueFilters] @MappingProjectId";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingProjectId", Value = mappingProjectId });

                using (var reader = cmd.ExecuteReader())
                {
                    filters = ((IObjectContextAdapter)_context).ObjectContext.Translate<MappingProjectQueueFilterDashboardModel>(reader).ToList();
                }
            }

            return filters.ToArray();
        }

        public MappingProjectQueueFilterViewModel Post(Guid mappingProjectId, MappingProjectQueueFilterCreateModel model)
        {
            var createdByUsers = model.CreatedByUserIds.Select(userId => _userRepository.FindById(userId)).Where(user => user != null).ToList();
            var updatedByUsers = model.UpdatedByUserIds.Select(userId => _userRepository.FindById(userId)).Where(user => user != null).ToList();
            var elementGroups = model.ElementGroups.Select(systemItemId => _systemItemRepository.Get(systemItemId)).Where(systemItem => systemItem != null).ToList();
            var itemTypes = model.ItemTypes.Select(itemTypeId => _context.ItemTypes.FirstOrDefault(x => x.ItemTypeId == itemTypeId)).ToList();
            var mappingMethods = model.MappingMethods.Select(mappingMethodId => _context.MappingMethodTypes.FirstOrDefault(x => x.MappingMethodTypeId == mappingMethodId)).ToList();
            var workflowStatuses = model.WorkflowStatuses.Select(workflowStatusId => _context.WorkflowStatusTypes.FirstOrDefault(x => x.WorkflowStatusTypeId == workflowStatusId)).ToList();

            var currentUser = _userRepository.FindById(Principal.Current.UserId);

            var filter = new MappingProjectQueueFilter
            {
                AutoMapped = model.AutoMapped,
                Base = model.Base,
                CreatedByColumn = model.CreatedByColumn,
                CreationDateColumn = model.CreationDateColumn,
                CreatedByUsers = createdByUsers,
                Extended = model.Extended,
                Flagged = model.Flagged,
                ItemTypes = _mapper.Map<ItemType[]>(itemTypes),
                Length = model.Length,
                MappingProjectId = mappingProjectId,
                MappedByColumn = model.MappedByColumn,
                MappingMethodTypes = _mapper.Map<MappingMethodType[]>(mappingMethods),
                Name = model.Name,
                OrderColumn = model.OrderColumn,
                OrderDirection = model.OrderDirection,
                ParentSystemItems = elementGroups,
                Search = model.Search,
                ShowInDashboard = model.ShowInDashboard,
                Unflagged = model.Unflagged,
                UserId = new Guid(Principal.Current.UserId),
                User = currentUser,
                UpdatedByUsers = updatedByUsers,
                UpdatedByColumn = model.UpdatedByColumn,
                UpdateDateColumn = model.UpdateDateColumn,
                WorkflowStatusTypes = _mapper.Map<WorkflowStatusType[]>(workflowStatuses),
            };

            _mappingProjectQueueFilterRepository.Add(filter);
            _mappingProjectQueueFilterRepository.SaveChanges();
            return Get(filter.MappingProjectQueueFilterId);
        }

        public MappingProjectQueueFilterViewModel Put(Guid mappingProjectQueueFilterId, Guid mappingProjectId, MappingProjectQueueFilterEditModel model)
        {
            var queueFilter = _mappingProjectQueueFilterRepository.GetAllQueryable().FirstOrDefault(x => x.MappingProjectQueueFilterId == mappingProjectQueueFilterId);

            if (queueFilter == null)
                throw new NotFoundException("Unable to find Mapping Project Queue Filter with Id " + mappingProjectQueueFilterId);

            var createdByUsers = model.CreatedByUserIds.Select(userId => _userRepository.FindById(userId)).Where(user => user != null).ToList();
            var updatedByUsers = model.UpdatedByUserIds.Select(userId => _userRepository.FindById(userId)).Where(user => user != null).ToList();
            var elementGroups = model.ElementGroups.Select(systemItemId => _systemItemRepository.Get(systemItemId)).Where(systemItem => systemItem != null).ToList();
            var itemTypes = model.ItemTypes.Select(itemTypeId => _context.ItemTypes.FirstOrDefault(x => x.ItemTypeId == itemTypeId)).ToList();
            var mappingMethods = model.MappingMethods.Select(mappingMethodId => _context.MappingMethodTypes.FirstOrDefault(x => x.MappingMethodTypeId == mappingMethodId)).ToList();
            var workflowStatuses = model.WorkflowStatuses.Select(workflowStatusId => _context.WorkflowStatusTypes.FirstOrDefault(x => x.WorkflowStatusTypeId == workflowStatusId)).ToList();

            var currentUser = _userRepository.FindById(Principal.Current.UserId);

            queueFilter.CreatedByUsers.Clear();
            queueFilter.ItemTypes.Clear();
            queueFilter.MappingMethodTypes.Clear();
            queueFilter.ParentSystemItems.Clear();
            queueFilter.UpdatedByUsers.Clear();
            queueFilter.WorkflowStatusTypes.Clear();
            _mappingProjectQueueFilterRepository.SaveChanges();

            queueFilter.AutoMapped = model.AutoMapped;
            queueFilter.Base = model.Base;
            queueFilter.CreatedByColumn = model.CreatedByColumn;
            queueFilter.CreationDateColumn = model.CreationDateColumn;
            queueFilter.CreatedByUsers = createdByUsers;
            queueFilter.Extended = model.Extended;
            queueFilter.Flagged = model.Flagged;
            queueFilter.ItemTypes = _mapper.Map<ItemType[]>(itemTypes);
            queueFilter.Length = model.Length;
            queueFilter.MappingProjectId = mappingProjectId;
            queueFilter.MappedByColumn = model.MappedByColumn;
            queueFilter.MappingMethodTypes = _mapper.Map<MappingMethodType[]>(mappingMethods);
            queueFilter.Name = model.Name;
            queueFilter.OrderColumn = model.OrderColumn;
            queueFilter.OrderDirection = model.OrderDirection;
            queueFilter.ParentSystemItems = elementGroups;
            queueFilter.Search = model.Search;
            queueFilter.ShowInDashboard = model.ShowInDashboard;
            queueFilter.Unflagged = model.Unflagged;
            queueFilter.UserId = new Guid(Principal.Current.UserId);
            queueFilter.User = currentUser;
            queueFilter.UpdatedByUsers = updatedByUsers;
            queueFilter.UpdatedByColumn = model.UpdatedByColumn;
            queueFilter.UpdateDateColumn = model.UpdateDateColumn;
            queueFilter.WorkflowStatusTypes = _mapper.Map<WorkflowStatusType[]>(workflowStatuses);

            _mappingProjectQueueFilterRepository.SaveChanges();
            return Get(mappingProjectQueueFilterId);
        }

        public void Delete(Guid mappingProjectQueueFilterId)
        {
            var queueFilter = _mappingProjectQueueFilterRepository.GetAllQueryable().FirstOrDefault(x => x.MappingProjectQueueFilterId == mappingProjectQueueFilterId && x.UserId.ToString() == Principal.Current.UserId);

            if (queueFilter == null)
                throw new NotFoundException("Unable to find Mapping Project Queue Filter with Id " + mappingProjectQueueFilterId);

            _mappingProjectQueueFilterRepository.Delete(queueFilter);
            _mappingProjectQueueFilterRepository.SaveChanges();
        }
    }
}