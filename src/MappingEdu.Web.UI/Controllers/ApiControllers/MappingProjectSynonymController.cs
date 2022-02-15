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
    public class MappingProjectSynonymController : ControllerBase
    {
        private readonly IMappingProjectSynonymService _synonymService;

        public MappingProjectSynonymController(IMappingProjectSynonymService synonymService)
        {
            _synonymService = synonymService;
        }

        [Route("MappingProjectSynonym/{mappingProjectId:guid}")]
        [AcceptVerbs("GET")]
        public ICollection<MappingProjectSynonymModel> Get(Guid mappingProjectId)
        {
            return _synonymService.Get(mappingProjectId);
        }

        [Route("MappingProjectSynonym/{mappingProjectId:guid}")]
        [AcceptVerbs("POST")]
        public MappingProjectSynonymModel Post(Guid mappingProjectId, MappingProjectSynonymModel model)
        {
            return _synonymService.Post(mappingProjectId, model);
        }

        [Route("MappingProjectSynonym/{mappingProjectId:guid}/{synonymId:guid}")]
        [AcceptVerbs("PUT")]
        public MappingProjectSynonymModel Put(Guid mappingProjectId, Guid synonymId, MappingProjectSynonymModel model)
        {
            return _synonymService.Put(mappingProjectId, synonymId, model);
        }

        [Route("MappingProjectSynonym/{mappingProjectId:guid}/{synonymId:guid}")]
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage Delete(Guid mappingProjectId, Guid synonymId)
        {
            _synonymService.Delete(mappingProjectId, synonymId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}