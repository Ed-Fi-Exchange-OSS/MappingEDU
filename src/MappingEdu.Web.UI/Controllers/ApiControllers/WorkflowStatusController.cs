// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItemMapping;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing workflow status
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class WorkflowStatusController : ControllerBase
    {
        private readonly IWorkflowStatusService _workflowStatusService;

        public WorkflowStatusController(IWorkflowStatusService workflowStatusService)
        {
            _workflowStatusService = workflowStatusService;
        }

        [Route("WorkflowStatus/{id:guid}/{id2:guid}")]
        [AcceptVerbs("PUT")]
        public SystemItemMappingViewModel Put(Guid id, Guid id2, SystemItemMappingEditModel model)
        {
            return _workflowStatusService.Put(id, id2, model);
        }
    }
}