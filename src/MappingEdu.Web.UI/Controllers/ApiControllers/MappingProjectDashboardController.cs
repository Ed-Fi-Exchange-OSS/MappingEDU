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
    public class MappingProjectDashboardController : ControllerBase
    {
        private readonly IMappingProjectDashboardService _mappingProjectDashboardService;

        public MappingProjectDashboardController(IMappingProjectDashboardService mappingProjectDashboardService)
        {
            _mappingProjectDashboardService = mappingProjectDashboardService;
        }

        [Route("MappingProjectDashboard/{id:guid}")]
        [AcceptVerbs("GET")]
        public MappingProjectDashboardViewModel Get(Guid id)
        {
            return _mappingProjectDashboardService.Get(id);
        }
    }
}