// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.Entity;
using MappingEdu.Service.Model.NewSystemItem;
using MappingEdu.Service.Model.SystemItemName;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing entities
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class EntityController : ControllerBase
    {
        private readonly INewEntityService _entityService;

        public EntityController(INewEntityService entityService)
        {
            _entityService = entityService;
        }

        [Route("Entity/{systemItemId:guid}")]
        [AcceptVerbs("GET")]
        public NewEntityViewModel Get(Guid systemItemId, Guid mappingProjectId = new Guid())
        {
            return _entityService.Get(systemItemId, mappingProjectId);
        }


        [Route("Entity/{mappedSystemId:guid}/first-level")]
        [AcceptVerbs("GET")]
        public NewSystemItemViewModel[] GetFirstLevelEntities(Guid mappedSystemId)
        {
            return _entityService.GetFirstLevelEntities(mappedSystemId);
        }

        [Route("Entity/{id:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id)
        {
            _entityService.Delete(id);
        }
    }
}