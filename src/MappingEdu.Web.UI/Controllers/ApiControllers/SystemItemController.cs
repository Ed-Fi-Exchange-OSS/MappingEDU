// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItem;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemController : ControllerBase
    {
        private readonly ISystemItemService _systemItemService;

        public SystemItemController(ISystemItemService systemItemService)
        {
            _systemItemService = systemItemService;
        }

        [Route("SystemItem/{systemItemId:guid}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage Get(Guid systemItemId)
        {
            var systemItem = _systemItemService.Get(systemItemId);
            return Request.CreateResponse(HttpStatusCode.OK, systemItem);
        }

        [Route("SystemItem/{systemItemId:guid}/detail")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetDetail(Guid systemItemId, Guid? mappingProjectId = null)
        {
            var systemItem = _systemItemService.GetDetail(systemItemId, mappingProjectId);
            return Request.CreateResponse(HttpStatusCode.OK, systemItem);
        }


        [Route("SystemItem/{systemItemId:guid}/usage")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetUsage(Guid systemItemId)
        {
            var systemItem = _systemItemService.GetUsage(systemItemId);
            return Request.CreateResponse(HttpStatusCode.OK, systemItem);
        }


        [Route("SystemItem")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage Post(SystemItemCreateModel model)
        {
            var systemItem = _systemItemService.Post(model);
            return Request.CreateResponse(HttpStatusCode.OK, systemItem);
        }

        [Route("SystemItem/{systemItemId:guid}")]
        [AcceptVerbs("PUT")]
        public HttpResponseMessage Put(Guid systemItemId, SystemItemEditModel model)
        {
            var systemItem = _systemItemService.Put(systemItemId, model);
            return Request.CreateResponse(HttpStatusCode.OK, systemItem);
        }

        [Route("SystemItem/{systemItemId:guid}")]
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage Put(Guid systemItemId)
        {
            _systemItemService.Delete(systemItemId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}