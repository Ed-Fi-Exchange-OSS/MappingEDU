// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Common.Exceptions;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing mapping project reports
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappingProjectReportsController : ControllerBase
    {
        private static MemoryCache _reportCache;
        private readonly IMappingProjectReportsService _mappingProjectReportsService;

        public MappingProjectReportsController(IMappingProjectReportsService mappingProjectReportsService)
        {
            _mappingProjectReportsService = mappingProjectReportsService;
            _reportCache = MemoryCache.Default;
        }

        [AllowAnonymous]
        [Route("MappingProjectReports/{projectId}/ceds/{reportId:guid}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetCedsReport(Guid projectId, Guid reportId)
        {
            var result = (HttpResponseMessage) _reportCache.Get(reportId.ToString());
            if (result == null)
                throw new NotFoundException("Error locating report id");

            return result;
        }

        [Route("MappingProjectReports/{projectId:guid}/ceds-token")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetCedsReportToken(Guid projectId)
        {
            var result = _mappingProjectReportsService.CreateCedsReport(projectId);

            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) //TODO Add to Configuration (App Settings)
            };
            _reportCache.Add(token.ToString(), result, policy);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [AllowAnonymous]
        [Route("MappingProjectReports/{projectId}/report/{reportId:guid}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetReport(Guid projectId, Guid reportId)
        {
            var result = (HttpResponseMessage) _reportCache.Get(reportId.ToString());
            if (result == null)
                throw new NotFoundException("Error locating report id");

            return result;
        }
        
        [Route("MappingProjectReports/{projectId:guid}/token")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetReportToken(Guid projectId, MappingProjectCreateReportModel reportModel)
        {
            var result = _mappingProjectReportsService.CreateReport(projectId, reportModel);

            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) //TODO Add to Configuration (App Settings)
            };
            _reportCache.Add(token.ToString(), result, policy);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [Route("MappingProjectReports/{projectId:guid}/target-token")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetTargetReportToken(Guid projectId, MappingProjectCreateReportModel reportModel)
        {
            var result = _mappingProjectReportsService.CreateTargetReport(projectId, reportModel);

            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) //TODO Add to Configuration (App Settings)
            };
            _reportCache.Add(token.ToString(), result, policy);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }
    }
}