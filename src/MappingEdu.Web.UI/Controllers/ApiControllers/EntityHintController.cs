// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.EntityHints;
using MappingEdu.Service.Model.EntityHints;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for searching elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class EntityHintController : ControllerBase
    {
        private readonly IEntityHintService _entityHintService;

        public EntityHintController(IEntityHintService entityHintService)
        {
            _entityHintService = entityHintService;
        }

        [Route("EntityHint/{mappingProjectId:guid}")]
        [AcceptVerbs("GET")]
        public ICollection<EntityHintViewModel> Get(Guid mappingProjectId)
        {
            return _entityHintService.Get(mappingProjectId);
        }


        [Route("EntityHint/{mappingProjectId:guid}/filter/{systemItemId:guid}")]
        [AcceptVerbs("GET")]
        public EntityHintViewModel GetEntityFilter(Guid mappingProjectId, Guid systemItemId)
        {
            return _entityHintService.GetEntityFilter(mappingProjectId, systemItemId);
        }

        [Route("EntityHint/{mappingProjectId:guid}")]
        [AcceptVerbs("POST")]
        public EntityHintViewModel Post(Guid mappingProjectId, EntityHintEditModel model)
        {
            return _entityHintService.Post(mappingProjectId, model);
        }

        [Route("EntityHint/{mappingProjectId:guid}/{entityHintId:guid}")]
        [AcceptVerbs("PUT")]
        public EntityHintViewModel Put(Guid mappingProjectId, Guid entityHintId, EntityHintEditModel model)
        {
            return _entityHintService.Put(mappingProjectId, entityHintId, model);
        }

        [Route("EntityHint/{mappingProjectId:guid}/{entityHintId:guid}")]
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage Delete(Guid mappingProjectId, Guid entityHintId)
        {
            _entityHintService.Delete(mappingProjectId, entityHintId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}