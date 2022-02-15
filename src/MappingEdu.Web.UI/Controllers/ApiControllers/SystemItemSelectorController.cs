// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.MappedSystem;
using MappingEdu.Service.SystemItemSelector;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing system item selectors
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemSelectorController : ControllerBase
    {
        private readonly ISystemItemSelectorService _service;

        public SystemItemSelectorController(ISystemItemSelectorService service)
        {
            _service = service;
        }

        [Route("SystemItemSelector")]
        [AcceptVerbs("GET")]
        public MappedSystemViewModel[] Get()
        {
            return _service.Get();
        }

        [Route("SystemItemSelector/{id:guid}")]
        [AcceptVerbs("GET")]
        public MappedSystemViewModel Get(Guid id)
        {
            return _service.Get(id);
        }
    }
}