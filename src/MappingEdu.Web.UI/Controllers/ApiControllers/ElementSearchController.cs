// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.ElementDetails;
using MappingEdu.Service.Model.SystemItem;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for searching elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class ElementSearchController : ControllerBase
    {
        private readonly IElementSearchService _elementSearchService;

        public ElementSearchController(IElementSearchService elementSearchService)
        {
            _elementSearchService = elementSearchService;
        }

        [Route("ElementSearch/{dataStandardId:guid}")]
        [AcceptVerbs("GET")]
        public IEnumerable<SystemItemTypeaheadViewModel> Get(Guid dataStandardId, Guid? parentSystemItemId = null, string searchText = null, string domainItemPath = null)
        {
           return _elementSearchService.SearchElements(dataStandardId, parentSystemItemId, searchText, domainItemPath);
        }

        [Route("ElementSearch/{id:guid}/paging")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage GetPaging(Guid id, SystemItemSearchDatatablesModel model)
        {
            var result = _elementSearchService.SearchElementsPaging(id, model);
            var returnPage = new DatatablesReturnModel<ElementDetailsSearchModel>()
            {
                data = result.Items.ToList(),
                draw = model.draw,
                recordsFiltered = result.TotalFiltered,
                recordsTotal = result.TotalRecords
            };

            return Request.CreateResponse(HttpStatusCode.OK, returnPage);
        }
    }
}