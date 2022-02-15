// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItemTree;
using MappingEdu.Service.SystemItemTree;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing system item tree
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemTreeController : ControllerBase
    {
        private readonly SystemItemTreeService _treeService;

        public SystemItemTreeController(SystemItemTreeService treeService)
        {
            _treeService = treeService;
        }

        [Route("SystemItemTree/{id:guid}")]
        [AcceptVerbs("GET")]
        public IEnumerable<SystemItemTypeViewModel> Get(Guid id)
        {
            var treeModel = _treeService.Get(id);
            return treeModel;
        }

        [Route("SystemItemTree/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemTreeViewModel Get(Guid id, Guid id2)
        {
            var treeModel = _treeService.Get(id, id2);
            return treeModel;
        }
    }
}