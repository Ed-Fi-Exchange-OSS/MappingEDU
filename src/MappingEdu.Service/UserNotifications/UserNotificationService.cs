// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.Model.UserNotification;

namespace MappingEdu.Service.UserNotifications
{
    public interface IUserNotificationService
    {
        void Dismiss(ICollection<Guid> notificationIds);

        PagedResult<UserNotificationViewModel> GetNotificationsPaging(Guid? mappingProjectId, DatatablesModel model);

        UserNotificationElementModel[] GetNotificationElements(Guid? mappingProjectId, DatatablesModel model);

        int GetUnreadNotificationCount(Guid? mappingProjectId);
    }

    public class UserNotificationService : IUserNotificationService
    {
        private readonly IRepository<UserNotification> _userNotificationRepository;

        public UserNotificationService(IRepository<UserNotification> userNotificationRepository)
        {
            _userNotificationRepository = userNotificationRepository;
        }

        public PagedResult<UserNotificationViewModel> GetNotificationsPaging(Guid? mappingProjectId, DatatablesModel model)
        {
            var notifications = _userNotificationRepository.GetAllQueryable()
                .Where(x => (!x.IsDismissed) && 
                            (x.UserId == Principal.Current.UserId) &&
                            (mappingProjectId.HasValue ? x.MappingProjectId == mappingProjectId : x.MappingProjectId.HasValue))
                .ToList()
                .GroupBy(x => x.SystemItemMapId)
                .Select(x => new UserNotificationViewModel
                {
                    SystemItemId = x.First().SystemItemMap.SourceSystemItemId,
                    Element = GetElementSegment(x.First().SystemItemMap.SourceSystemItem),
                    PathSegments = GetPathSegments(x.First().SystemItemMap.SourceSystemItem).ToArray(),
                    MappingProjectId = x.First().MappingProjectId.Value,
                    Notification = GetNotficationText(x),
                    NotificationDate = x.Max(y => y.CreateDate),
                    HasSeen = x.All(y => y.HasSeen),
                    UserNotificationIds = x.Select(y => y.UserNotificationId).ToList()
                }).ToList();

            var total = notifications.Count();

            if (model.search != null && model.search.value != null)
            {
                notifications = notifications.Where(x => x.Notification.Contains(model.search.value) ||
                                                        GetDomainItemPath(x.PathSegments, x.Element).ToLower().Contains(model.search.value.ToLower()))
                                             .ToList();
            }

            var filtered = notifications.Count();

            var order = model.order[0];
            switch (order.column)
            {
                case 0:
                    notifications = order.dir == "desc" ?
                        notifications.OrderByDescending(x => GetDomainItemPath(x.PathSegments, x.Element)).ToList() :
                        notifications.OrderBy(x => GetDomainItemPath(x.PathSegments, x.Element)).ToList();
                    break;
                case 1:
                    notifications = order.dir == "desc" ?
                        notifications.OrderByDescending(x => x.Notification).ToList() :
                        notifications.OrderBy(x => x.Notification).ToList();
                    break;
                default:
                    notifications = order.dir == "desc" ?
                        notifications.OrderByDescending(x => x.NotificationDate).ToList() :
                        notifications.OrderBy(x => x.NotificationDate).ToList();
                    break;
            }

            notifications = notifications.Skip(model.start).Take(model.length).ToList();


            return new PagedResult<UserNotificationViewModel>
            {
                Items = notifications,
                TotalFiltered = filtered,
                TotalRecords = total
            };
        }

        public UserNotificationElementModel[] GetNotificationElements(Guid? mappingProjectId, DatatablesModel model)
        {
            var notifications = _userNotificationRepository.GetAllQueryable()
                .Where(x => (!x.IsDismissed) &&
                            (x.UserId == Principal.Current.UserId) &&
                            (mappingProjectId.HasValue ? x.MappingProjectId == mappingProjectId : x.MappingProjectId.HasValue))
                .ToList()
                .GroupBy(x => x.SystemItemMapId)
                .Select(x => new UserNotificationViewModel
                {
                    SystemItemId = x.First().SystemItemMap.SourceSystemItemId,
                    Element = GetElementSegment(x.First().SystemItemMap.SourceSystemItem),
                    PathSegments = GetPathSegments(x.First().SystemItemMap.SourceSystemItem).ToArray(),
                    MappingProjectId = x.First().MappingProjectId.Value,
                    Notification = GetNotficationText(x),
                    NotificationDate = x.Max(y => y.CreateDate),
                    HasSeen = x.All(y => y.HasSeen),
                    UserNotificationIds = x.Select(y => y.UserNotificationId).ToList()
                }).ToList();

            if (model.search != null && model.search.value != null)
            {
                notifications = notifications.Where(x => (x.Notification != null && x.Notification.Contains(model.search.value)) ||
                                                        GetDomainItemPath(x.PathSegments, x.Element).ToLower().Contains(model.search.value.ToLower()))
                                             .ToList();
            }

            var order = model.order[0];
            switch (order.column)
            {
                case 0:
                    notifications = order.dir == "desc" ?
                        notifications.OrderByDescending(x => GetDomainItemPath(x.PathSegments, x.Element)).ToList() :
                        notifications.OrderBy(x => GetDomainItemPath(x.PathSegments, x.Element)).ToList();
                    break;
                case 1:
                    notifications = order.dir == "desc" ?
                        notifications.OrderByDescending(x => x.Notification).ToList() :
                        notifications.OrderBy(x => x.Notification).ToList();
                    break;
                default:
                    notifications = order.dir == "desc" ?
                        notifications.OrderByDescending(x => x.NotificationDate).ToList() :
                        notifications.OrderBy(x => x.NotificationDate).ToList();
                    break;
            }

            return notifications.Select(x => new UserNotificationElementModel {MappingProjectId = x.MappingProjectId, ElementId = x.SystemItemId}).ToArray();
        }

        public void Dismiss(ICollection<Guid> notificationIds)
        {
            var notifications = _userNotificationRepository.GetAll().Where(x => notificationIds.Contains(x.UserNotificationId) && Principal.Current.UserId == x.UserId);
            foreach (var notification in notifications)
                notification.IsDismissed = true;
            
            _userNotificationRepository.SaveChanges();
        }

        public int GetUnreadNotificationCount(Guid? mappingProjectId)
        {
            var count = _userNotificationRepository
                .GetAllQueryable()
                .Count(x => (!x.IsDismissed) &&
                            (!x.HasSeen) &&
                            (x.UserId == Principal.Current.UserId) &&
                            (mappingProjectId.HasValue ? x.MappingProjectId == mappingProjectId : x.MappingProjectId.HasValue));

            return count;
        }

        private string GetNotficationText(IGrouping<Guid?, UserNotification> notifications)
        {
            if (notifications.Count(x => !x.HasSeen) > 1)
                return string.Format("{0} New Notifications", notifications.Count(x => !x.HasSeen));

            if(notifications.Count(x => !x.HasSeen) == 1)
                return notifications.First(x => !x.HasSeen).MapNoteId.HasValue ? notifications.First(x => !x.HasSeen).MapNote.Notes : notifications.First(x => !x.HasSeen).SystemItemMap.StatusNote;

            if (notifications.Count() > 1)
                return string.Format("{0} Read Notifications", notifications.Count());
            
            return notifications.First().MapNoteId.HasValue ? notifications.First().MapNote.Notes : notifications.First().SystemItemMap.StatusNote;
        }

        private ICollection<ElementListViewModel.ElementPathViewModel.PathSegment> GetPathSegments(SystemItem systemItem)
        {
            var item = systemItem.ParentSystemItem;

            var segments = new List<ElementListViewModel.ElementPathViewModel.PathSegment>();
            while (item != null)
            {
                var segment = new ElementListViewModel.ElementPathViewModel.PathSegment
                {
                    Definition = item.Definition,
                    Name = item.ItemName,
                    SystemItemId = item.SystemItemId,
                    IsExtended = item.IsExtended
                };
                segments.Add(segment);
                item = item.ParentSystemItem;
            }
            segments.Reverse();
            return segments;
        }

        private ElementListViewModel.ElementPathViewModel.ElementSegment GetElementSegment(SystemItem systemItem)
        {
            return new ElementListViewModel.ElementPathViewModel.ElementSegment
            {
                IsExtended = systemItem.IsExtended,
                Definition = systemItem.Definition,
                Name = systemItem.ItemName,
                ItemTypeName = systemItem.ItemType.Name,
                SystemItemId = systemItem.SystemItemId
            };
        }

        private string GetDomainItemPath(ICollection<ElementListViewModel.ElementPathViewModel.PathSegment> segments, ElementListViewModel.ElementPathViewModel.ElementSegment element)
        {
            return segments.Aggregate("", (current, segment) => current + (segment.Name + '.')) + element.Name;
        }
    }
}
