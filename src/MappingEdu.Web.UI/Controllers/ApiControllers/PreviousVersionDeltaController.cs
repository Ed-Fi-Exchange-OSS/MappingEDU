// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.PreviousVersionDelta;
using MappingEdu.Service.Model.SystemItem;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing previous version deltas
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class PreviousVersionDeltaController : ControllerBase
    {
        private readonly IPreviousVersionDeltaService _previousVersionDeltaService;

        public PreviousVersionDeltaController(IPreviousVersionDeltaService previousVersionDeltaService)
        {
            _previousVersionDeltaService = previousVersionDeltaService;
        }

        [Route("PreviousVersionDelta/{id:guid}")]
        [AcceptVerbs("GET")]
        public PreviousVersionDeltaViewModel[] Get(Guid id)
        {
            return _previousVersionDeltaService.Get(id);
        }

        [Route("PreviousVersionDelta/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public PreviousVersionDeltaViewModel Get(Guid id, Guid id2)
        {
            return _previousVersionDeltaService.Get(id, id2);
        }

        [Route("PreviousVersionDelta/{id:guid}")]
        [AcceptVerbs("POST")]
        public PreviousVersionDeltaViewModel Post(Guid id, PreviousVersionDeltaCreateModel model)
        {
            return _previousVersionDeltaService.Post(id, model);
        }

        [Route("PreviousVersionDelta/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public PreviousVersionDeltaViewModel Put(Guid id, Guid id2, PreviousVersionDeltaEditModel model)
        {
            return _previousVersionDeltaService.Put(id, id2, model);
        }

        [Route("PreviousVersionDelta/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _previousVersionDeltaService.Delete(id, id2);
        }
    }
}