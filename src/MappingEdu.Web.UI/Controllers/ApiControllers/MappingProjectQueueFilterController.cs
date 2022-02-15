// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing mapping project review queue
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappingProjectQueueFilterController : ControllerBase
    {
        private readonly IMappingProjectQueueFilterService _mappingProjectQueueFilterService;

        public MappingProjectQueueFilterController(IMappingProjectQueueFilterService mappingProjectQueueFilterService)
        {
            _mappingProjectQueueFilterService = mappingProjectQueueFilterService;
        }

        [Route("MappingProjectQueueFilter")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetAll(Guid? mappingProjectId)
        {
            var results = _mappingProjectQueueFilterService.GetAll(mappingProjectId);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        [Route("MappingProjectQueueFilter/{mappingProjectId:guid}/dashboard")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetDashboard(Guid mappingProjectId)
        {
            var results = _mappingProjectQueueFilterService.GetDashboard(mappingProjectId);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        [Route("MappingProjectQueueFilter/{mappingProjectId:guid}")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage Post(Guid mappingProjectId, MappingProjectQueueFilterCreateModel model)
        {
            var result = _mappingProjectQueueFilterService.Post(mappingProjectId, model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("MappingProjectQueueFilter/{mappingProjectQueueFilterId:guid}/MappingProject/{mappingProjectId:guid}")]
        [AcceptVerbs("PUT")]
        public HttpResponseMessage Put(Guid mappingProjectQueueFilterId, Guid mappingProjectId, MappingProjectQueueFilterEditModel model)
        {
            var result = _mappingProjectQueueFilterService.Put(mappingProjectQueueFilterId, mappingProjectId, model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("MappingProjectQueueFilter/{mappingProjectQueueFilterId:guid}")]
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage Delete(Guid mappingProjectQueueFilterId)
        {
            _mappingProjectQueueFilterService.Delete(mappingProjectQueueFilterId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}