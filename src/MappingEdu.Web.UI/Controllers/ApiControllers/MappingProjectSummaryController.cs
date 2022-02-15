// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing mapping project dashboards
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappingProjectSummaryController : ControllerBase
    {
        private readonly IMappingProjectSummaryService _mappingProjectSummaryService;

        public MappingProjectSummaryController(IMappingProjectSummaryService mappingProjectSummaryService)
        {
            _mappingProjectSummaryService = mappingProjectSummaryService;
        }

        [Route("MappingProjectSummary/{id:guid}")]
        [AcceptVerbs("GET")]
        public MappingProjectSummaryViewModel[] Get(Guid id, int? itemTypeId = null, Guid? parentSystemItemId = null)
        {
            return _mappingProjectSummaryService.Get(id, itemTypeId, parentSystemItemId);
        }

        [Route("MappingProjectSummary/{id:guid}/detail")]
        [AcceptVerbs("GET")]
        public MappingProjectSummaryDetailViewModel[] GetDetail(Guid id, int? itemTypeId = null, Guid? systemItemId = null)
        {
            return _mappingProjectSummaryService.GetDetail(id, itemTypeId, systemItemId);
        }
    }
}