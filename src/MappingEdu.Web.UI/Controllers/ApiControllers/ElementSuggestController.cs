// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for suggesting elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class ElementSuggestController : ControllerBase
    {
        private readonly IElementSuggestService _elementSuggestService;

        public ElementSuggestController(IElementSuggestService elementSuggestService)
        {
            _elementSuggestService = elementSuggestService;
        }

        [Route("ElementSuggest/{mappingProjectId:guid}/{systemItemId:guid}")]
        [AcceptVerbs("GET")]
        public SuggestModel Get(Guid mappingProjectId, Guid systemItemId)
        {
            return _elementSuggestService.SuggestElements(mappingProjectId, systemItemId);
        }
    }
}