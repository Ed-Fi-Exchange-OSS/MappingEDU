// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.Entity;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class ElementController : ControllerBase
    {
        private readonly INewElementService _elementService;

        public ElementController(INewElementService elementService)
        {
            _elementService = elementService;
        }

        [Route("Element/{id:guid}")]
        [AcceptVerbs("GET")]
        public NewElementViewModel Get(Guid id)
        {
            return _elementService.Get(id);
        }

        [Route("Element/{id:guid}/children")]
        [AcceptVerbs("GET")]
        public IEnumerable<ElementDetailsViewModel> GetChildren(Guid id)
        {
            return _elementService.GetChildren(id);
        }


        [Route("Element/{mappingProjectId:guid}/{systemItemId:guid}")]
        [AcceptVerbs("GET")]
        public NewElementViewModel Get(Guid mappingProjectId, Guid systemItemId)
        {
            return _elementService.Get(mappingProjectId, systemItemId);
        }

        [Route("Element/{id:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id)
        {
            _elementService.Delete(id);
        }
    }
}