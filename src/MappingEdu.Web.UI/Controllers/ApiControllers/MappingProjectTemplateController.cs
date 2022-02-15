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
using MappingEdu.Service.EntityHints;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.EntityHints;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for searching elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappingProjectTemplateController : ControllerBase
    {
        private readonly IMappingProjectTemplateService _templateService;

        public MappingProjectTemplateController(IMappingProjectTemplateService templateService)
        {
            _templateService = templateService;
        }

        [Route("MappingProjectTemplate/{mappingProjectId:guid}")]
        [AcceptVerbs("GET")]
        public ICollection<MappingProjectTemplateModel> Get(Guid mappingProjectId)
        {
            return _templateService.Get(mappingProjectId);
        }

        [Route("MappingProjectTemplate/{mappingProjectId:guid}")]
        [AcceptVerbs("POST")]
        public MappingProjectTemplateModel Post(Guid mappingProjectId, MappingProjectTemplateModel model)
        {
            return _templateService.Post(mappingProjectId, model);
        }

        [Route("MappingProjectTemplate/{mappingProjectId:guid}/{templateId:guid}")]
        [AcceptVerbs("PUT")]
        public MappingProjectTemplateModel Put(Guid mappingProjectId, Guid templateId, MappingProjectTemplateModel model)
        {
            return _templateService.Put(mappingProjectId, templateId, model);
        }

        [Route("MappingProjectTemplate/{mappingProjectId:guid}/{templateId:guid}")]
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage Delete(Guid mappingProjectId, Guid templateId)
        {
            _templateService.Delete(mappingProjectId, templateId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}