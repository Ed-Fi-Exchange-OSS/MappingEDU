// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MapNote;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MappingEdu.Service.SystemItems
{
    public interface IMapNoteService
    {
        MapNoteViewModel[] Get(Guid systemItemMapId);

        MapNoteViewModel Get(Guid systemItemMapId, Guid mapNoteId);

        MapNoteViewModel Post(Guid systemItemMapId, MapNoteCreateModel model);

        MapNoteViewModel Put(Guid systemItemMapId, Guid mapNoteId, MapNoteEditModel model);

        void Delete(Guid systemItemMapId, Guid mapNoteId);
    }

    public class MapNoteService : IMapNoteService
    {
        private readonly IRepository<MapNote> _mapNoteRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<SystemItemMap> _systemItemMapRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<UserNotification> _userNotificationRepository;

        public MapNoteService(IRepository<MapNote> mapNoteRepository, IRepository<SystemItemMap> systemItemMapRepository, IMapper mapper, IUserRepository userRepository,
            IRepository<UserNotification> userNotificationRepository)
        {
            _mapNoteRepository = mapNoteRepository;
            _systemItemMapRepository = systemItemMapRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _userNotificationRepository = userNotificationRepository;
        }

        public MapNoteViewModel[] Get(Guid systemItemMapId)
        {
            var systemItemMap = GetSystemItemMap(systemItemMapId);
            var mapNoteViewModels = _mapper.Map<MapNoteViewModel[]>(systemItemMap.MapNotes);
            return mapNoteViewModels;
        }

        public MapNoteViewModel Get(Guid systemItemMapId, Guid mapNoteId)
        {
            var mapNote = GetMapNote(systemItemMapId, mapNoteId, MappingProjectUser.MappingProjectUserRole.View);
            var mapNoteViewModel = _mapper.Map<MapNoteViewModel>(mapNote);
            return mapNoteViewModel;
        }

        public MapNoteViewModel Post(Guid systemItemMapId, MapNoteCreateModel model)
        {
            var systemItemMap = GetSystemItemMap(systemItemMapId, MappingProjectUser.MappingProjectUserRole.Edit);

            var users = GetUsersFromNote(model.Notes);

            var notifications = users.Select(user => new UserNotification {
                HasSeen = false,
                IsDismissed = false,
                MappingProjectId = systemItemMap.MappingProjectId,
                SystemItemMapId = systemItemMapId,
                UserId = user.Id
            }).ToList();

            var mapNote = new MapNote
            {
                SystemItemMap = systemItemMap,
                SystemItemMapId = systemItemMap.SystemItemMapId,
                Title = model.Title != null ? model.Title : "",
                Notes = model.Notes,
                UserNotifications = notifications
            };

            _mapNoteRepository.Add(mapNote);
            _mapNoteRepository.SaveChanges();

            return Get(systemItemMapId, mapNote.MapNoteId);
        }

        public MapNoteViewModel Put(Guid systemItemMapId, Guid mapNoteId, MapNoteEditModel model)
        {
            var mapNote = GetMapNote(systemItemMapId, mapNoteId, MappingProjectUser.MappingProjectUserRole.Edit);

            if (!mapNote.CreateById.HasValue || mapNote.CreateById.Value.ToString() != Principal.Current.UserId)
                throw new SecurityException("Cannot modify other users notes");

            mapNote.Title = model.Title != null ? model.Title : "";
            mapNote.Notes = model.Notes;

            var notifiedUserIds = mapNote.UserNotifications.Select(x => x.UserId).ToList();

            var users = GetUsersFromNote(model.Notes).ToList().Where(x => !notifiedUserIds.Contains(x.Id));

            var notifications = users.Select(user => new UserNotification
            {
                HasSeen = false,
                IsDismissed = false,
                MappingProjectId = mapNote.SystemItemMap.MappingProjectId,
                SystemItemMapId = systemItemMapId,
                UserId = user.Id
            }).ToList();

            foreach (var notification in notifications)
                mapNote.UserNotifications.Add(notification);

            foreach (var notification in mapNote.UserNotifications)
                notification.IsDismissed = notification.HasSeen = false;

            _mapNoteRepository.SaveChanges();

            return Get(systemItemMapId, mapNoteId);
        }

        public void Delete(Guid systemItemMapId, Guid mapNoteId)
        {
            var mapNote = GetMapNote(systemItemMapId, mapNoteId, MappingProjectUser.MappingProjectUserRole.Edit);

            if (!mapNote.CreateById.HasValue || mapNote.CreateById.Value.ToString() != Principal.Current.UserId)
                throw new SecurityException("Cannot delete other users notes");

            var notifications = mapNote.UserNotifications;

            foreach (var notification in mapNote.UserNotifications)
            {
                notification.MapNote = null;
                notification.MapNoteId = null;
            }
            mapNote.UserNotifications = null;
            _mapNoteRepository.Delete(mapNote);

            foreach (var notification in notifications)
                _userNotificationRepository.Delete(notification);

            _mapNoteRepository.SaveChanges();
        }

        private IEnumerable<ApplicationUser> GetUsersFromNote(string note)
        {
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

        private MapNote GetMapNote(Guid systemItemMapId, Guid mapNoteId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var mapNote = _mapNoteRepository.Get(mapNoteId);

            if (mapNote == null)
                throw new Exception(string.Format("The map note with id '{0}' does not exist.", mapNoteId));
            if (!Principal.Current.IsAdministrator && !mapNote.SystemItemMap.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            if (mapNote.SystemItemMapId != systemItemMapId)
                throw new Exception(string.Format("The map note with id '{0}' does not have a parent system item map id of '{1}'.", mapNoteId, systemItemMapId));

            return mapNote;
        }

        private SystemItemMap GetSystemItemMap(Guid systemItemMapId, MappingProjectUser.MappingProjectUserRole role = MappingProjectUser.MappingProjectUserRole.Guest)
        {
            var systemItemMap = _systemItemMapRepository.Get(systemItemMapId);
            if (null == systemItemMap)
                throw new Exception(string.Format("The system item map with id '{0}' does not exist.", systemItemMapId));
            if (!Principal.Current.IsAdministrator && !systemItemMap.MappingProject.HasAccess(role))
                throw new SecurityException(String.Format("User needs at least {0} Access to peform this action", role));
            return systemItemMap;
        }
    }
}