// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.Enumeration;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing enumerations
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class EnumerationController : ControllerBase
    {
        private readonly IEnumerationService _enumerationService;

        public EnumerationController(IEnumerationService enumerationService)
        {
            _enumerationService = enumerationService;
        }

        [Route("Enumeration/{id:guid}")]
        [AcceptVerbs("GET")]
        public EnumerationViewModel Get(Guid id)
        {
            return _enumerationService.Get(id);
        }

        [Route("Enumeration/{id:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id)
        {
            _enumerationService.Delete(id);
        }
    }
}