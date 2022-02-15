// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing mapping project elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappingProjectElementsController : ControllerBase
    {
        private readonly IMappingProjectElementsService _mappingProjectElementsService;

        public MappingProjectElementsController(IMappingProjectElementsService mappingProjectElementsService)
        {
            _mappingProjectElementsService = mappingProjectElementsService;
        }

        [Route("MappingProjectElements/{id:guid}")]
        [AcceptVerbs("GET")]
        public IEnumerable<MappingProjectElementViewModel> Get(Guid id, string filter)
        {
            return _mappingProjectElementsService.GetElementIds(id, filter);
        }
    }
}