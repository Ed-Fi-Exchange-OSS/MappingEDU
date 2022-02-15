// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.EnumerationItem;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing enumeration item types
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class EnumerationItemController : ControllerBase
    {
        private readonly IEnumerationItemService _enumerationItemService;

        public EnumerationItemController(IEnumerationItemService enumerationItemService)
        {
            _enumerationItemService = enumerationItemService;
        }

        [Route("EnumerationItem/{id:guid}")]
        [AcceptVerbs("GET")]
        public EnumerationItemViewModel[] Get(Guid id)
        {
            return _enumerationItemService.Get(id);
        }

        [Route("EnumerationItem/{id:guid}/{id2:guid}")]
        [AcceptVerbs("GET")]
        public EnumerationItemViewModel Get(Guid id, Guid id2)
        {
            return _enumerationItemService.Get(id, id2);
        }

        [Route("EnumerationItem/{id:guid}")]
        [AcceptVerbs("POST")]
        public EnumerationItemViewModel Post(Guid id, EnumerationItemCreateModel enumerationItemCreateModel)
        {
            return _enumerationItemService.Post(id, enumerationItemCreateModel);
        }

        [Route("EnumerationItem/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public EnumerationItemViewModel Put(Guid id, Guid id2, EnumerationItemEditModel enumerationItemEditModel)
        {
            return _enumerationItemService.Put(id, id2, enumerationItemEditModel);
        }

        [Route("EnumerationItem/{id:guid}/{id2:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid id2)
        {
            _enumerationItemService.Delete(id, id2);
        }
    }
}