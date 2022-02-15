// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.Membership;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing system item mappings
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemMappingController : ControllerBase
    {
        private readonly ISystemItemMappingService _systemItemMappingService;

        public SystemItemMappingController(ISystemItemMappingService systemItemMappingService)
        {
            _systemItemMappingService = systemItemMappingService;
        }

        [Route("SystemItemMapping/{sourceSystemItemId:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemMappingViewModel[] Get(Guid sourceSystemItemId)
        {
            return _systemItemMappingService.GetSourceMappings(sourceSystemItemId);
        }

        [Route("SystemItemMapping/{systemItemId:guid}/{systemItemMapId:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemMappingViewModel Get(Guid systemItemId, Guid systemItemMapId)
        {
            return _systemItemMappingService.Get(systemItemId, systemItemMapId);
        }

        [Route("SystemItemMapping/{systemItemId:guid}/Project/{mappingProjectId:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemMappingViewModel GetMappingByProject(Guid systemItemId, Guid mappingProjectId)
        {
            return _systemItemMappingService.GetMappingByProject(systemItemId, mappingProjectId);
        }

        [Route("SystemItemMapping/{mappingProjectId:guid}/UniqueCreateBy")]
        [AcceptVerbs("GET")]
        public UserNameViewModel[] GetUnquieCreateBy(Guid mappingProjectId)
        {
            return _systemItemMappingService.GetUniqueCreateBy(mappingProjectId);
        }


        [Route("SystemItemMapping/{mappingProjectId:guid}/UniqueUpdateBy")]
        [AcceptVerbs("GET")]
        public UserNameViewModel[] GetUnquieUpdateBy(Guid mappingProjectId)
        {
            return _systemItemMappingService.GetUniqueUpdateBy(mappingProjectId);
        }

        [Route("SystemItemMapping/{id:guid}")]
        [AcceptVerbs("POST")]
        public SystemItemMappingViewModel Post(Guid id, SystemItemMappingCreateModel model)
        {
            return _systemItemMappingService.Post(id, model);
        }

        [Route("SystemItemMapping/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public SystemItemMappingViewModel Put(Guid id, Guid id2, SystemItemMappingEditModel model)
        {
            return _systemItemMappingService.Put(id, id2, model);
        }

        [Route("SystemItemMapping/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _systemItemMappingService.Delete(id, id2);
        }
    }
}