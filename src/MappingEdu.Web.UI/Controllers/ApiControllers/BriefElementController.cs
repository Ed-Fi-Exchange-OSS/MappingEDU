// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.BriefElement;
using MappingEdu.Service.SystemItems;

//TODO: Figure out what id2 is used for

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for accessing approve all system item maps
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class BriefElementController : ControllerBase
    {
        private readonly IBriefElementService _briefElementService;
        private readonly IElementService _elementService;

        public BriefElementController(IBriefElementService briefElementService, IElementService elementService)
        {
            _briefElementService = briefElementService;
            _elementService = elementService;
        }

        [Route("BriefElement/{id:guid}")]
        [AcceptVerbs("GET")]
        public BriefElementViewModel[] Get(Guid id)
        {
            return _briefElementService.Get(id);
        }

        [Route("BriefElement/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public BriefElementViewModel Get(Guid id, Guid id2)
        {
            return _briefElementService.Get(id, id2);
        }

        [Route("BriefElement/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _elementService.Delete(id2);
        }
    }
}