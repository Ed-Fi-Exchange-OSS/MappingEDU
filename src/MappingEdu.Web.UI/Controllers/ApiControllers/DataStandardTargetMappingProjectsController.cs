// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing data target mappings
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class DataStandardTargetMappingProjectsController : ControllerBase
    {
        private readonly IMappingProjectService _mappingProjectService;

        public DataStandardTargetMappingProjectsController(IMappingProjectService mappingProjectService)
        {
            _mappingProjectService = mappingProjectService;
        }

        [Route("DataStandardTargetMappingProjects/{id:guid}")]
        [AcceptVerbs("GET")]
        public DataStandardMappingProjectsViewModel[] Get(Guid id)
        {
            return _mappingProjectService.GetTargetMappingProjects(id);
        }
    }
}