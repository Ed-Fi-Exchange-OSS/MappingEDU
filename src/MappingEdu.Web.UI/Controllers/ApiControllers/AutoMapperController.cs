// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.AutoMap;
using MappingEdu.Service.Model.AutoMap;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing projects
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class AutoMapperController : ControllerBase
    {
        private readonly IAutoMapService _autoMapService;

        public AutoMapperController(IAutoMapService autoMapService)
        {
            _autoMapService = autoMapService;
        }

        [Route("AutoMapper/mapping-suggestions")]
        [AcceptVerbs("POST")]
        public ICollection<AutoMap> MappingSuggestions(MappingProjectCreateModel model, Guid? mappingProjectId = null)
        {
            var projectId = (mappingProjectId == null) ? new Guid() : mappingProjectId.Value; 
            return _autoMapService.GetAutoMapResults(model.SourceDataStandardId, model.TargetDataStandardId, projectId);
        }

        [Route("AutoMapper/add-results/{projectId:guid}")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddResults(MappingProjectCreateModel model, Guid projectId)
        {
           _autoMapService.CreateAutoMappings(model.SourceDataStandardId, model.TargetDataStandardId, projectId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("AutoMapper/delta-copy/{standardId:guid}")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage DeltaCopy(Guid standardId)
        {
            _autoMapService.DeltaCopy(standardId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}