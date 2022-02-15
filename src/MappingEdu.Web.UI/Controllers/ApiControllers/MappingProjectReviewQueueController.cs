// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing mapping project review queue
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappingProjectReviewQueueController : ControllerBase
    {
        private readonly IMappingProjectReviewQueueService _mappingProjectReviewQueueService;

        public MappingProjectReviewQueueController(IMappingProjectReviewQueueService mappingProjectReviewQueueService)
        {
            _mappingProjectReviewQueueService = mappingProjectReviewQueueService;
        }

        [Route("MappingProjectReviewQueue/{id:guid}")]
        [AcceptVerbs("GET")]
        public MappingProjectReviewQueueViewModel Get(Guid id)
        {
            return _mappingProjectReviewQueueService.Get(id);
        }

        [Route("MappingProjectReviewQueue/{id:guid}/paging")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetPaging(Guid id, ReviewQueueDatatablesModel model)
        {
            var result = _mappingProjectReviewQueueService.GetPaging(id, model);
            var returnPage = new DatatablesReturnModel<MappingProjectReviewQueueViewModel.ReviewItemViewModel>()
            {
                data = result.Items.ToList(),
                draw = model.draw,
                recordsFiltered = result.TotalFiltered,
                recordsTotal = result.TotalRecords
            };

            return Request.CreateResponse(HttpStatusCode.OK, returnPage);
        }

        [Route("MappingProjectReviewQueue/{id:guid}/elements")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetElementIds(Guid id, ReviewQueueDatatablesModel model)
        {
            var elements = _mappingProjectReviewQueueService.GetElementIds(id, model);
            return Request.CreateResponse(HttpStatusCode.OK, elements);
        }
    }
}