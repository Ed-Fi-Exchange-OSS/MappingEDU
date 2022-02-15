// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for accessing approve all system item maps
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class ApproveAllSystemItemMapsController : ControllerBase
    {
        private readonly IApproveAllSystemItemMapsService _approveAllSystemItemMapsService;

        public ApproveAllSystemItemMapsController(IApproveAllSystemItemMapsService approveAllSystemItemMapsService)
        {
            _approveAllSystemItemMapsService = approveAllSystemItemMapsService;
        }

        [Route("ApproveAllSystemItemMaps/{id:guid}")]
        [AcceptVerbs("PUT")]
        public BulkActionResultsModel Put(Guid id, MappingProjectViewModel model)
        {
            return _approveAllSystemItemMapsService.Put(id, model);
        }
    }
}