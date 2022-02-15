// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.SystemItemTree;
using MappingEdu.Service.Model.UserNotification;
using MappingEdu.Service.SystemItemTree;
using MappingEdu.Service.UserNotifications;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for notifications
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class UserNotificationsController : ControllerBase
    {
        private readonly IUserNotificationService _userNotificationService;

        public UserNotificationsController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }

        [Route("Notifications/Paging")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetPaging(DatatablesModel model, Guid? mappingProjectId = null)
        {
            var result = _userNotificationService.GetNotificationsPaging(mappingProjectId, model);
            var returnPage = new DatatablesReturnModel<UserNotificationViewModel>()
            {
                data = result.Items.ToList(),
                draw = model.draw,
                recordsFiltered = result.TotalFiltered,
                recordsTotal = result.TotalRecords
            };

            return Request.CreateResponse(HttpStatusCode.OK, returnPage);
        }

        [Route("Notifications/Elements")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetElementIds(DatatablesModel model, Guid? mappingProjectId = null)
        {
            var elements = _userNotificationService.GetNotificationElements(mappingProjectId, model);
            return Request.CreateResponse(HttpStatusCode.OK, elements);
        }

        [Route("Notifications/Unread")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetUnreadCount(Guid? mappingProjectId = null)
        {
            var count = _userNotificationService.GetUnreadNotificationCount(mappingProjectId);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }


        [Route("Notifications/{notificationId:guid}")]
        [AcceptVerbs("DElETE")]
        public HttpResponseMessage DismissNotification(Guid notificationId)
        {
            var ids = new List<Guid>();
            ids.Add(notificationId);
            _userNotificationService.Dismiss(ids);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [Route("Notifications/Dismiss")]
        [AcceptVerbs("DElETE")]
        public HttpResponseMessage DismissNotifications([FromUri] Guid[] notificationIds )
        {
            _userNotificationService.Dismiss(notificationIds);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}