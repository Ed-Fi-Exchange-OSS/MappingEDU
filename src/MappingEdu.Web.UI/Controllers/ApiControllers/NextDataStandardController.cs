// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.DataStandard;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing next data standards
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class NextDataStandardController : ControllerBase
    {
        private readonly INextDataStandardService _nextDataStandardService;

        public NextDataStandardController(INextDataStandardService nextDataStandardService)
        {
            _nextDataStandardService = nextDataStandardService;
        }

        [Route("NextDataStandard/{id:guid}")]
        [AcceptVerbs("GET")]
        public DataStandardViewModel Get(Guid id)
        {
            return _nextDataStandardService.Get(id);
        }
    }
}