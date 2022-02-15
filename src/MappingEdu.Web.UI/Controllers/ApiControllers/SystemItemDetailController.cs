// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing system item details
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class SystemItemDetailController : ControllerBase
    {
        private readonly ISystemItemCustomDetailService _systemItemCustomDetailService;

        public SystemItemDetailController(ISystemItemCustomDetailService systemItemCustomDetailService)
        {
            _systemItemCustomDetailService = systemItemCustomDetailService;
        }

        [Route("SystemItemDetail/{id:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemCustomDetailViewModel[] Get(Guid id)
        {
            return _systemItemCustomDetailService.Get(id);
        }

        [Route("SystemItemDetail/{id:guid}/{systemItemCustomDetailId:guid}")]
        [AcceptVerbs("GET")]
        public SystemItemCustomDetailViewModel Get(Guid id, Guid systemItemCustomDetailId)
        {
            return _systemItemCustomDetailService.Get(id, systemItemCustomDetailId);
        }

        [Route("SystemItemDetail/{id:guid}")]
        [AcceptVerbs("POST")]
        public SystemItemCustomDetailViewModel Post(Guid id, SystemItemCustomDetailCreateModel model)
        {
            return _systemItemCustomDetailService.Post(id, model);
        }

        [Route("SystemItemDetail")]
        [AcceptVerbs("PUT")]
        public SystemItemCustomDetailsGroupViewModel Put(SystemItemCustomDetailsGroupEditModel model)
        {
            var viewModels =
                model.SystemItemCustomDetails.Select(
                    customDetail => (customDetail.SystemItemCustomDetailId != Guid.Empty) ? 
                        _systemItemCustomDetailService.Put(model.SystemItemId, customDetail) : 
                        _systemItemCustomDetailService.Post(model.SystemItemId, customDetail)).ToArray();

            var returnViewModel = new SystemItemCustomDetailsGroupViewModel(viewModels, model.SystemItemId);

            return returnViewModel;
        }

        [Route("SystemItemDetail/{id:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id, Guid systemItemCustomDetailId)
        {
            _systemItemCustomDetailService.Delete(id, systemItemCustomDetailId);
        }
    }
}