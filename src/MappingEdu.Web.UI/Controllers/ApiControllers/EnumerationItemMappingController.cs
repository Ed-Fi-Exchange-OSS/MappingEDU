// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.EnumerationItemMapping;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing enumeration item mappings
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class EnumerationItemMappingController : ControllerBase
    {
        private readonly IEnumerationItemMappingService _enumerationItemMappingService;

        public EnumerationItemMappingController(IEnumerationItemMappingService enumerationItemMappingService)
        {
            _enumerationItemMappingService = enumerationItemMappingService;
        }

        [Route("EnumerationItemMapping/{id:guid}")]
        [AcceptVerbs("GET")]
        public EnumerationItemMappingViewModel[] Get(Guid id)
        {
            return _enumerationItemMappingService.Get(id);
        }

        [Route("EnumerationItemMapping/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public EnumerationItemMappingViewModel Get(Guid id, Guid id2)
        {
            return _enumerationItemMappingService.Get(id, id2);
        }

        [Route("EnumerationItemMapping/{id:guid}")]
        [AcceptVerbs("POST")]
        public EnumerationItemMappingViewModel Post(Guid id, EnumerationItemMappingCreateModel model)
        {
            return _enumerationItemMappingService.Post(id, model);
        }

        [Route("EnumerationItemMapping/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public EnumerationItemMappingViewModel Put(Guid id, Guid id2, EnumerationItemMappingEditModel model)
        {
            return _enumerationItemMappingService.Put(id, id2, model);
        }

        [Route("EnumerationItemMapping/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _enumerationItemMappingService.Delete(id, id2);
        }
    }
}