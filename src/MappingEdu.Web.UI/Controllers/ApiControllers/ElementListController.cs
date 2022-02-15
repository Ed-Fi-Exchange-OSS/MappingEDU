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
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing element lists
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class ElementListController : ControllerBase
    {
        private readonly IElementListService _elementListService;

        public ElementListController(IElementListService elementListService)
        {
            _elementListService = elementListService;
        }

        [Route("ElementList/{id:guid}")]
        [AcceptVerbs("GET")]
        public ElementListViewModel Get(Guid id)
        {
            return _elementListService.Get(id);
        }

        [Route("ElementList/{id:guid}/paging")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetPaging(Guid id, ElementListDatatablesModel model)
        {
            var result = _elementListService.GetPaging(id, model);
            var returnPage = new DatatablesReturnModel<ElementListViewModel.ElementPathViewModel>()
            {
                data = result.Items.ToList(),
                draw = model.draw,
                recordsFiltered = result.TotalFiltered,
                recordsTotal = result.TotalRecords
            };

            return Request.CreateResponse(HttpStatusCode.OK, returnPage);
        }

        [Route("ElementList/{id:guid}/delta-paging")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetPagingForDelta(Guid id, ElementListDatatablesModel model)
        {
            var result = _elementListService.GetPagingForDelta(id, model);
            var returnPage = new DatatablesReturnModel<ElementListViewModel.ElementPathViewModel>()
            {
                data = result.Items.ToList(),
                draw = model.draw,
                recordsFiltered = result.TotalFiltered,
                recordsTotal = result.TotalRecords
            };

            return Request.CreateResponse(HttpStatusCode.OK, returnPage);
        }

        [Route("ElementList/{id:guid}/elements")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetElementIds(Guid id, ElementListDatatablesModel model)
        {
            var elements = _elementListService.GetElementIds(id, model);
            return Request.CreateResponse(HttpStatusCode.OK, elements);
        }
    }
}