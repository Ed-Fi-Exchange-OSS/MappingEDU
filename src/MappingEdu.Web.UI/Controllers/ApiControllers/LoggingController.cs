// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Common.Exceptions;
using MappingEdu.Service.Logging;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.Logging;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing logging
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class LoggingController : ControllerBase
    {
        private readonly ILoggingService _loggingService;
        private static MemoryCache _logCache;

        public LoggingController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
            _logCache = MemoryCache.Default;
        }

        [AllowAnonymous]
        [Route("Logging")]
        [AcceptVerbs("POST")]
        public LoggingViewModel Post(LoggingCreateModel model)
        {
            return _loggingService.Post(model);
        }

        [Route("Logging/paging")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetPaging(LoggingDataTablesModel model)
        {
            var result = _loggingService.GetPaging(model);
            var returnPage = new DatatablesReturnModel<LoggingViewModel>()
            {
                data = result.Items.ToList(),
                draw = model.draw,
                recordsFiltered = result.TotalFiltered,
                recordsTotal = result.TotalRecords
            };

            return Request.CreateResponse(HttpStatusCode.OK, returnPage);
        }

        [Route("Logging/Export")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage Export(LoggingDataTablesModel model)
        {
            var result = _loggingService.Export(model);

            var token = Guid.NewGuid();
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1) //TODO Add to Configuration (App Settings)
            };
            _logCache.Add(token.ToString(), result, policy);

            return Request.CreateResponse(HttpStatusCode.OK, token);
        }

        [Route("Logging/Export-Count")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage ExportCount(LoggingDataTablesModel model)
        {
            var result = _loggingService.ExportCount(model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [AllowAnonymous]
        [Route("Logging/export/{exportId:guid}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage GetExport(Guid exportId)
        {
            var result = (HttpResponseMessage)_logCache.Get(exportId.ToString());
            if (result == null)
                throw new NotFoundException("Error locating report id");

            _logCache.Remove(exportId.ToString());

            return result;
        }

        [Route("Logging/clear")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage ClearLogs(LoggingDeleteModel model)
        {
            _loggingService.ClearLogs(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}