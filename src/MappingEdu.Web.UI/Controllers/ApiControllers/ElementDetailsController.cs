// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing element details
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class ElementDetailsController : ControllerBase
    {
        private readonly IElementDetailsService _elementDetailsService;

        public ElementDetailsController(IElementDetailsService elementDetailsService)
        {
            _elementDetailsService = elementDetailsService;
        }

        [Route("ElementDetails/{id:guid}")]
        [AcceptVerbs("GET")]
        public ElementDetailsViewModel Get(Guid id)
        {
            return _elementDetailsService.Get(id);
        }

        [Route("ElementDetails")]
        [AcceptVerbs("POST")]
        public ElementDetailsViewModel Post(ElementDetailsCreateModel model)
        {
            return _elementDetailsService.Post(model);
        }

        [Route("ElementDetails/{id:guid}")]
        [AcceptVerbs("PUT")]
        public ElementDetailsViewModel Put(Guid id, ElementDetailsEditModel editModel)
        {
            return _elementDetailsService.Put(id, editModel);
        }
    }
}