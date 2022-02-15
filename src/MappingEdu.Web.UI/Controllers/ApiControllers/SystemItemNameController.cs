// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItemName;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing system item names
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemNameController : ControllerBase
    {
        private readonly ISystemItemNameService _systemItemNameService;

        public SystemItemNameController(ISystemItemNameService systemItemNameService)
        {
            _systemItemNameService = systemItemNameService;
        }

        [Route("SystemItemName/{id:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemNameViewModel Get(Guid id)
        {
            return _systemItemNameService.Get(id);
        }

        [Route("SystemItemName/{id:guid}")]
        [AcceptVerbs("PUT")]
        public SystemItemNameViewModel Put(Guid id, SystemItemNameEditModel model)
        {
            return _systemItemNameService.Put(model);
        }
    }
}