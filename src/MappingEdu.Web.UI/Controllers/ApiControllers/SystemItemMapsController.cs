// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.MappingProject;
using MappingEdu.Service.Model.SystemItemBulk;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for accessing approve all system item maps
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemMapsController : ControllerBase
    {
        private readonly ISystemItemMapsService _systemItemMapsService;

        public SystemItemMapsController(ISystemItemMapsService systemItemMapsService)
        {
            _systemItemMapsService = systemItemMapsService;
        }

        [Route("SystemItemMaps/{mappingProjectId:guid}/Add")]
        [AcceptVerbs("PUT")]
        public BulkActionResultsModel Add(Guid mappingProjectId, BulkAddMappingsModel model)
        {
            return _systemItemMapsService.CreateMappings(mappingProjectId, model);
        }

        [Route("SystemItemMaps/{mappingProjectId:guid}/ApproveReviewed")]
        [AcceptVerbs("PUT")]
        public BulkActionResultsModel ApproveReviewed(Guid mappingProjectId)
        {
            return _systemItemMapsService.MarkReviewApproved(mappingProjectId);
        }

        [Route("SystemItemMaps/{mappingProjectId:guid}/AddCount")]
        [AcceptVerbs("PUT")]
        public BulkActionResultsModel NumberToCreate(Guid mappingProjectId, BulkAddMappingsModel model)
        {
            return _systemItemMapsService.NumberToCreate(mappingProjectId, model);
        }

        [Route("SystemItemMaps/{mappingProjectId:guid}/UpdateCount")]
        [AcceptVerbs("PUT")]
        public BulkActionResultsModel NumberToUpdate(Guid mappingProjectId, BulkUpdateMappingsModel model)
        {
            return _systemItemMapsService.NumberToUpdate(mappingProjectId, model);
        }

        [Route("SystemItemMaps/{mappingProjectId:guid}/Update")]
        [AcceptVerbs("PUT")]
        public BulkActionResultsModel Update(Guid mappingProjectId, BulkUpdateMappingsModel model)
        {
            return _systemItemMapsService.UpdateMappings(mappingProjectId, model);
        }
    }
}