// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services.Mapping;
using MappingEdu.Service.Model.Membership;
using MappingEdu.Service.Model.SystemItemMapping;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Service.SystemItems
{
    public interface ISystemItemMappingService
    {
        SystemItemMappingViewModel[] GetSourceMappings(Guid sourceSystemItemId, bool getAll = false);

        UserNameViewModel[] GetUniqueCreateBy(Guid mappingProjectId);

        UserNameViewModel[] GetUniqueUpdateBy(Guid mappingProjectId);

        SystemItemMappingViewModel[] GetItemMappingsByProject(Guid sourceSystemItemId, Guid mappingProjectId);

        SystemItemMappingViewModel GetMappingByProject(Guid sourceSytemItemId, Guid mappingProjectId);

        SystemItemMappingViewModel Get(Guid sourceSystemItemId, Guid systemItemMapId);

        SystemItemMappingViewModel Post(Guid sourceSystemItemId, SystemItemMappingCreateModel model);

        SystemItemMappingViewModel Put(Guid sourceSystemItemId, Guid systemItemMapId, SystemItemMappingEditModel model);

        void Delete(Guid sourceSystemItemId, Guid systemItemMapId);
    }

    public class SystemItemMappingService : ISystemItemMappingService
    {
        private readonly IBusinessLogicParser _businessLogicParser;
        private readonly IMapper _mapper;
        private readonly IRepository<MappingProject> _mappingProjectRepository;
        private readonly IRepository<SystemEnumerationItemMap> _systemEnumerationItemMapRepository;
        private readonly ISystemItemMapRepository _systemItemMappingRepository;
        private readonly IRepository<SystemItem> _systemItemRepository;
        private readonly IUserRepository _userRepository;

        public SystemItemMappingService(ISystemItemMapRepository systemItemMappingRepository,
            IRepository<SystemItem> systemItemRepository,
            IRepository<MappingProject> mappingProjectRepository, IRepository<SystemEnumerationItemMap>
                systemEnumerationItemMapRepository, IMapper mapper, IBusinessLogicParser businessLogicParser, IUserRepository userRepository)
        {
            _systemItemMappingRepository = systemItemMappingRepository;
            _systemItemRepository = systemItemRepository;
            _mappingProjectRepository = mappingProjectRepository;
            _systemEnumerationItemMapRepository = systemEnumerationItemMapRepository;
            _mapper = mapper;
            _businessLogicParser = businessLogicParser;
            _userRepository = userRepository;
        }

        public UserNameViewModel[] GetUniqueCreateBy(Guid mappingProjectId)
        {
            if (!Principal.Current.IsAdministrator && !_mappingProjectRepository.Get(mappingProjectId).HasAccess(MappingProjectUser.MappingProjectUserRole.Edit))
                return new UserNameViewModel[] {};

            var createByIds = _systemItemMappingRepository.GetAllMaps().Where(x => x.MappingProjectId == mappingProjectId).Select(x => x.CreateById.ToString()).Distinct();

            return _userRepository.GetAllUsers().Where(x => createByIds.Contains(x.Id))
                .Select(x => new UserNameViewModel
                {
                    FirstName = x.FirstName,
                    Id = x.Id,
                    LastName = x.LastName
                }).ToArray();
        }

        public UserNameViewModel[] GetUniqueUpdateBy(Guid mappingProjectId)
        {
            if (!Principal.Current.IsAdministrator && !_mappingProjectRepository.Get(mappingProjectId).HasAccess(MappingProjectUser.MappingProjectUserRole.Edit))
                return new UserNameViewModel[] { };

            var updateByIds = _systemItemMappingRepository.GetAllMaps().Where(x => x.MappingProjectId == mappingProjectId).Select(x => x.UpdateById.ToString()).Distinct();

            return _userRepository.GetAllUsers().Where(x => updateByIds.Contains(x.Id))
                .Select(x => new UserNameViewModel
                {
                    FirstName = x.FirstName,
                    Id = x.Id,
                    LastName = x.LastName
                }).ToArray();
        }

        public SystemItemMappingViewModel[] GetItemMappingsByProject(Guid sourceSystemItemId, Guid mappingProjectId)
        {
            if(!Principal.Current.IsAdministrator && !_mappingProjectRepository.Get(mappingProjectId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var hasEditAccess = Principal.Current.IsAdministrator || _mappingProjectRepository.Get(mappingProjectId).HasAccess(MappingProjectUser.MappingProjectUserRole.Edit);

            var maps = _systemItemMappingRepository.GetAllMaps()
                .Where(sim => sim.SourceSystemItemId == sourceSystemItemId &&
                              sim.MappingProjectId == mappingProjectId).ToArray();

            foreach (var notification in maps.SelectMany(x => x.UserNotifications.Where(y => y.UserId == Principal.Current.UserId)))
                notification.HasSeen = true;

            _systemItemMappingRepository.SaveChanges();

            var models = _mapper.Map<SystemItemMappingViewModel[]>(maps);

            foreach (var model in models)
            {
                if (hasEditAccess)
                {
                    if (model.CreateById.HasValue)
                    {
                        var createUser = _userRepository.FindById(model.CreateById.Value);
                        model.CreateBy = createUser.FirstName[0] + ". " + createUser.LastName;
                    }

                    if (model.UpdateById.HasValue)
                    {
                        var updateUser = _userRepository.FindById(model.UpdateById.Value);
                        model.UpdateBy = updateUser.FirstName[0] + ". " + updateUser.LastName;
                    }

                    if (model.MapNotes != null)
                        foreach (var note in model.MapNotes)
                        {
                            note.CreateBy = note.CreateById.HasValue ? GetUserName(note.CreateById) : null;
                            note.CreateById = null;
                        }
                }
                else
                {
                    model.UpdateBy = null;
                    model.UpdateDate = null;
                    model.CreateDate = null;
                    model.CreateBy = null;

                    if (model.MapNotes != null)
                        foreach (var note in model.MapNotes)
                        {
                            note.CreateBy = null;
                            note.CreateById = null;
                        }
                }

                model.CreateById = null;
                model.UpdateById = null;
            }

            return models;
        }

        public SystemItemMappingViewModel GetMappingByProject(Guid sourceSystemItemId, Guid mappingProjectId)
        {
            if (!Principal.Current.IsAdministrator && !_mappingProjectRepository.Get(mappingProjectId).HasAccess())
                throw new SecurityException("User needs at least Guest Access to peform this action");

            var hasEditAccess = Principal.Current.IsAdministrator || _mappingProjectRepository.Get(mappingProjectId).HasAccess(MappingProjectUser.MappingProjectUserRole.Edit);

            var map = _systemItemMappingRepository.GetAllMaps()
                .FirstOrDefault(sim => sim.SourceSystemItemId == sourceSystemItemId &&
                              sim.MappingProjectId == mappingProjectId);

            if (map == null) return null;

            foreach (var notification in map.UserNotifications.Where(x => x.UserId == Principal.Current.UserId))
                notification.HasSeen = true;

            _systemItemMappingRepository.SaveChanges();

            var model = _mapper.Map<SystemItemMappingViewModel>(map);

            if (hasEditAccess)
            {
                if (model.CreateById.HasValue)
                {
                    var createUser = _userRepository.FindById(model.CreateById.Value);
                    model.CreateBy = createUser.FirstName[0] + ". " + createUser.LastName;
                }

                if (model.UpdateById.HasValue)
                {
                    var updateUser = _userRepository.FindById(model.UpdateById.Value);
                    model.UpdateBy = updateUser.FirstName[0] + ". " + updateUser.LastName;
                }

                if (model.MapNotes != null)
                    foreach (var note in model.MapNotes)
                    {
                        note.CreateBy = note.CreateById.HasValue ? GetUserName(note.CreateById) : null;
                    }
            }
            else
            {
                model.UpdateBy = null;
                model.UpdateDate = null;
                model.CreateDate = null;
                model.CreateBy = null;

                if (model.MapNotes != null)
                    foreach (var note in model.MapNotes)
                    {
                        note.CreateBy = null;
                        note.CreateById = null;
                    }
            }

            model.CreateById = null;
            model.UpdateById = null;

            return model;
        }

        public SystemItemMappingViewModel[] GetSourceMappings(Guid sourceSystemItemId, bool getAll = false)
        {
            var maps = _systemItemMappingRepository.GetAllMaps()
                .Where(sim => sim.SourceSystemItemId == sourceSystemItemId).ToArray();

            if (!Principal.Current.IsAdministrator && !getAll)
                maps = maps.Where(x => x.MappingProject.HasAccess() && x.MappingProject.IsActive)
                           .Select(x =>{
                                if (Principal.Current.IsGuest) {
                                    x.WorkflowStatusTypeId = 0;
                                    x.StatusNote = "";
                                }
                                return x;
                            }).ToArray();

            var model = _mapper.Map<SystemItemMappingViewModel[]>(maps);
            return model;
        }

        public SystemItemMappingViewModel Get(Guid sourceSystemItemId, Guid systemItemMapId)
        {
            var systemItemMap = GetSystemItemMap(systemItemMapId, sourceSystemItemId);
            var model = _mapper.Map<SystemItemMappingViewModel>(systemItemMap);

            if (model == null || model.MapNotes == null) return model;

            foreach (var note in model.MapNotes)
            {
                note.CreateBy = note.CreateById.HasValue ? GetUserName(note.CreateById) : null;
                note.CreateById = null;
            }
            return model;
        }

        public SystemItemMappingViewModel Post(Guid sourceSystemItemId, SystemItemMappingCreateModel model)
        {
            var sourceSystemItem = GetSystemItem(sourceSystemItemId);
            var mappingProject = GetMappingProject(model.MappingProjectId, MappingProjectUser.MappingProjectUserRole.Edit);

            var systemItemMap = new SystemItemMap
            {
                MappingProjectId = mappingProject.MappingProjectId,
                MappingProject = mappingProject,
                SourceSystemItemId = sourceSystemItemId,
                SourceSystemItem = sourceSystemItem,
                BusinessLogic = model.BusinessLogic,
                DeferredMapping = model.DeferredMapping,
                OmissionReason = model.OmissionReason,
                MappingStatusTypeId = model.MappingStatusTypeId,
                CompleteStatusTypeId = model.CompleteStatusTypeId,
                MappingStatusReasonTypeId = model.MappingStatusReasonTypeId,
                ExcludeInExternalReports = model.ExcludeInExternalReports,
                WorkflowStatusTypeId = model.WorkflowStatusTypeId,
                MappingMethodTypeId = model.MappingMethodTypeId,
                Flagged = model.Flagged
            };

            LoadTargetSystemItems(mappingProject, systemItemMap, model.BusinessLogic);

            _systemItemMappingRepository.Add(systemItemMap);
            _systemItemMappingRepository.SaveChanges();

            return Get(systemItemMap.SourceSystemItemId, systemItemMap.SystemItemMapId);
        }

        public SystemItemMappingViewModel Put(
            Guid sourceSystemItemId, Guid systemItemMapId, SystemItemMappingEditModel model)
        {
            var systemItemMap = GetSystemItemMap(systemItemMapId, sourceSystemItemId, MappingProjectUser.MappingProjectUserRole.Edit);
            var mappingProject = GetMappingProject(model.MappingProjectId);

            systemItemMap.BusinessLogic = model.BusinessLogic;
            systemItemMap.DeferredMapping = model.DeferredMapping;
            systemItemMap.OmissionReason = model.OmissionReason;
            systemItemMap.MappingStatusTypeId = model.MappingStatusTypeId;
            systemItemMap.CompleteStatusTypeId = model.CompleteStatusTypeId;
            systemItemMap.MappingStatusReasonTypeId = model.MappingStatusReasonTypeId;
            systemItemMap.ExcludeInExternalReports = model.ExcludeInExternalReports;
            systemItemMap.WorkflowStatusTypeId = model.WorkflowStatusTypeId;
            systemItemMap.MappingMethodTypeId = model.MappingMethodTypeId;
            systemItemMap.Flagged = model.Flagged;
            systemItemMap.StatusNote = model.StatusNote;

            LoadTargetSystemItems(mappingProject, systemItemMap, model.BusinessLogic);

            CleanEnumerationMappings(systemItemMap);

            _systemItemMappingRepository.SaveChanges();

            return Get(systemItemMap.SourceSystemItemId, systemItemMap.SystemItemMapId);
        }

        public void Delete(Guid sourceSystemItemId, Guid systemItemMapId)
        {
            GetSystemItemMap(systemItemMapId, sourceSystemItemId, MappingProjectUser.MappingProjectUserRole.Edit);

            _systemItemMappingRepository.Delete(systemItemMapId);
            _systemItemMappingRepository.SaveChanges();
        }

        private string GetUserName(Guid? createdById = null)
        {
            if (!createdById.HasValue) return null;

            var user = _userRepository.GetAllUsers().FirstOrDefault(x => x.Id == createdById.Value.ToString());
            if (user != null) return user.FirstName + " " + user.LastName;

            return null;
        }

        private void LoadTargetSystemItems(
            MappingProject mappingProject, SystemItemMap systemItemMap, string businessLogic)
        {
            var targetSystemItems = _businessLogicParser.ParseReferencedSystemItems(
                businessLogic, systemItemMap.SourceSystemItem.ItemType.Id == ItemType.Enumeration.Id,
                mappingProject.TargetDataStandard);
            systemItemMap.TargetSystemItems.Clear();
            targetSystemItems.Do(x => systemItemMap.TargetSystemItems.Add(x));
        }

        private void CleanEnumerationMappings(SystemItemMap systemItemMap)
        {
            var missingTargets = systemItemMap.SystemEnumerationItemMaps.Where(
                x => x.TargetSystemEnumerationItem != null &&
                     !systemItemMap.TargetSystemItems.Contains(x.TargetSystemEnumerationItem.SystemItem)).ToArray();

            foreach (var missingTarget in missingTargets)
                _systemEnumerationItemMapRepository.Delete(missingTarget);
        }

        private SystemItem GetSystemItem(Guid sourceSystemItemId)
        {
            var systemItem = _systemItemRepository.Get(sourceSystemItemId);
            if (null == systemItem)
                throw new Exception(string.Format("System Item with id '{0}' does not exist.", sourceSystemItemId));
            return systemItem;
        }

        private MappingProject GetMappingProject(Guid mappingProjectId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var mappingProject = _mappingProjectRepository.Get(mappingProjectId);
            if (null == mappingProject)
                throw new Exception(string.Format("Mapping Project with id '{0}' does not exist.", mappingProjectId));
            if (!Principal.Current.IsAdministrator && !mappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return mappingProject;
        }

        private SystemItemMap GetSystemItemMap(Guid systemItemMapId, Guid sourceSystemItemId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var systemItemMap = _systemItemMappingRepository.Get(systemItemMapId);
            if (null == systemItemMap)
                throw new Exception(string.Format("System Item Map with id '{0}' does not exist.", systemItemMapId));
            if (systemItemMap.SourceSystemItemId != sourceSystemItemId)
                throw new Exception(
                    string.Format(
                        "System Item Map with id '{0}' does not have System Item id of '{1}'.", systemItemMapId, sourceSystemItemId));
            if (!Principal.Current.IsAdministrator && !systemItemMap.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));

            return systemItemMap;
        }
    }
}