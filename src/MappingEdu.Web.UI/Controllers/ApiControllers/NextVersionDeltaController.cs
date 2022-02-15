// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.NextVersionDelta;
using MappingEdu.Service.Model.SystemItem;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing next version deltas
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class NextVersionDeltaController : ControllerBase
    {
        private readonly INextVersionDeltaService _nextVersionDeltaService;

        public NextVersionDeltaController(INextVersionDeltaService nextVersionDeltaService)
        {
            _nextVersionDeltaService = nextVersionDeltaService;
        }

        [Route("NextVersionDelta/{id:guid}")]
        [AcceptVerbs("GET")]
        public NextVersionDeltaViewModel[] Get(Guid id)
        {
            return _nextVersionDeltaService.Get(id);
        }

        [Route("NextVersionDelta/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public NextVersionDeltaViewModel Get(Guid id, Guid id2)
        {
            return _nextVersionDeltaService.Get(id, id2);
        }

        [Route("NextVersionDelta/{id:guid}")]
        [AcceptVerbs("POST")]
        public NextVersionDeltaViewModel Post(Guid id, NextVersionDeltaCreateModel model)
        {
            return _nextVersionDeltaService.Post(id, model);
        }

        [Route("NextVersionDelta/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public NextVersionDeltaViewModel Put(Guid id, Guid id2, NextVersionDeltaEditModel model)
        {
            return _nextVersionDeltaService.Put(id, id2, model);
        }

        [Route("NextVersionDelta/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _nextVersionDeltaService.Delete(id, id2);
        }
    }
}