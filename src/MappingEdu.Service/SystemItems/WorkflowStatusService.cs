// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.SystemItemMapping;

namespace MappingEdu.Service.SystemItems
{
    public interface IWorkflowStatusService
    {
        SystemItemMappingViewModel Put(Guid sourceSystemItemId, Guid systemItemMapId, SystemItemMappingEditModel model);
    }

    public class WorkflowStatusService : IWorkflowStatusService
    {
        private readonly ISystemItemMapRepository _systemItemMappingRepository;
        private readonly ISystemItemMappingService _systemItemMappingService;
        private readonly IUserRepository _userRepository;

        public WorkflowStatusService(ISystemItemMapRepository systemItemMappingRepository, ISystemItemMappingService systemItemMappingService, IUserRepository userRepository)
        {
            _systemItemMappingRepository = systemItemMappingRepository;
            _systemItemMappingService = systemItemMappingService;
            _userRepository = userRepository;
        }

        public SystemItemMappingViewModel Put(Guid sourceSystemItemId, Guid systemItemMapId, SystemItemMappingEditModel model)
        {
            var systemItemMap = GetSystemItemMap(systemItemMapId, sourceSystemItemId);

            if(!Principal.Current.IsAdministrator && !systemItemMap.MappingProject.HasAccess(MappingProjectUser.MappingProjectUserRole.Edit))
                throw new SecurityException("User needs at least Edit Access to peform this action");

            var notifiedUserIds = systemItemMap.UserNotifications.Where(x => x.MapNoteId == null).Select(x => x.UserId).ToList();

            var users = GetUsersFromNote(model.StatusNote).ToList().Where(x => !notifiedUserIds.Contains(x.Id));

            var notifications = users.Select(user => new UserNotification
            {
                HasSeen = false,
                IsDismissed = false,
                MappingProjectId = systemItemMap.MappingProjectId,
                UserId = user.Id
            }).ToList();

            foreach (var notification in notifications)
                systemItemMap.UserNotifications.Add(notification);

            foreach (var notification in systemItemMap.UserNotifications.Where(x => x.MapNoteId == null))
                notification.IsDismissed = notification.HasSeen = false;

            systemItemMap.WorkflowStatusTypeId = model.WorkflowStatusTypeId;
            systemItemMap.Flagged = model.Flagged;
            systemItemMap.StatusNote = model.StatusNote;
            _systemItemMappingRepository.SaveChanges();

            return _systemItemMappingService.Get(systemItemMap.SourceSystemItemId, systemItemMap.SystemItemMapId);
        }

        private IEnumerable<ApplicationUser> GetUsersFromNote(string note)
        {
            if(note == null) return new List<ApplicationUser>();

            var split = note.Split(new[] { "[~" }, StringSplitOptions.None);

            var names = new List<String>();
            for (var i = 1; i < split.Length; i++)
            {
                var split2 = split[i].Split(']');
                if (split2.Length > 1)
                    names.Add(split2[0]);
            }

            var users = names
                .Select(name => new
                {
                    FirstName = name.ToLower().Split(' ')[0],
                    LastName = name.ToLower().Split(' ')[1]
                })
                .Select(name => _userRepository.GetAllUsers()
                    .FirstOrDefault(x => x.FirstName.ToLower() == name.FirstName &&
                                         x.LastName.ToLower() == name.LastName))
                .Where(user => user != null)
                .ToList();

            return users.Distinct();
        }

        private SystemItemMap GetSystemItemMap(Guid systemItemMapId, Guid sourceSystemItemId)
        {
            var systemItemMap = _systemItemMappingRepository.Get(systemItemMapId);
            if (null == systemItemMap)
                throw new Exception(string.Format("System Item Map with id '{0}' does not exist.", systemItemMapId));
            if (systemItemMap.SourceSystemItemId != sourceSystemItemId)
                throw new Exception(
                    string.Format(
                        "System Item Map with id '{0}' does not have System Item id of '{1}'.", systemItemMapId, sourceSystemItemId));
            return systemItemMap;
        }
    }
}