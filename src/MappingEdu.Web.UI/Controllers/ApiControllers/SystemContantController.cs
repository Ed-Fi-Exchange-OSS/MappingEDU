// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Core.Domain;
using MappingEdu.Service.Admin;
using MappingEdu.Service.Model.Admin;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for retrieving system constants
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemConstantController : ApiController
    {
        private readonly ISystemConstantService _systemConstantService;

        public SystemConstantController(ISystemConstantService systemConstantService)
        {
            _systemConstantService = systemConstantService;
        }

        [AllowAnonymous]
        [Route("SystemConstant/{name}")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage Get(string name)
        {
            var constant = _systemConstantService.Get(name);
            return Request.CreateResponse(HttpStatusCode.OK, constant);
        }

        [Route("SystemConstant")]
        [AcceptVerbs("GET")]
        public HttpResponseMessage Get()
        {
            var constants = _systemConstantService.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, constants);
        }

        [Route("SystemConstant/{id}")]
        [AcceptVerbs("Put")]
        public HttpResponseMessage Put(int id, SystemConstantModel constant)
        {
            var result = _systemConstantService.Put(id, constant);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}