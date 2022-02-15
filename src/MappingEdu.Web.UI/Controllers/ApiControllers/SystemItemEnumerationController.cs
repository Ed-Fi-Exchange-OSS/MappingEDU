// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItemEnumeration;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing system item enumerations
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemEnumerationController : ControllerBase
    {
        private readonly ISystemItemEnumerationService _systemItemEnumerationService;

        public SystemItemEnumerationController(ISystemItemEnumerationService systemItemEnumerationService)
        {
            _systemItemEnumerationService = systemItemEnumerationService;
        }

        [Route("SystemItemEnumeration/{id:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemEnumerationViewModel[] Get(Guid id)
        {
            return _systemItemEnumerationService.Get(id);
        }
    }
}