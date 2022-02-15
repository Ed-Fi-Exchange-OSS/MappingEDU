// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItemDefinition;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing system item definitions
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemDefinitionController : ControllerBase
    {
        private readonly ISystemItemDefinitionService _systemItemDefinitionService;

        public SystemItemDefinitionController(ISystemItemDefinitionService systemItemDefinitionService)
        {
            _systemItemDefinitionService = systemItemDefinitionService;
        }

        [Route("SystemItemDefinition/{id:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemDefinitionViewModel Get(Guid id)
        {
            return _systemItemDefinitionService.Get(id);
        }

        [Route("SystemItemDefinition/{id:guid}")]
        [AcceptVerbs("PUT")]
        public SystemItemDefinitionViewModel Put(Guid id, SystemItemDefinitionEditModel value)
        {
            return _systemItemDefinitionService.Put(value);
        }
    }
}