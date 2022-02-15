// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.SystemItemCustomDetail;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing data standards
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class CustomDetailMetadataController : ControllerBase
    {
        private readonly ICustomDetailMetadataService _customDetailMetadataService;

        public CustomDetailMetadataController(ICustomDetailMetadataService customDetailMetadataService)
        {
            _customDetailMetadataService = customDetailMetadataService;
        }

        [Route("CustomDetailMetadata/{mappedSystemId:guid}")]
        [AcceptVerbs("GET")]
        public CustomDetailMetadataViewModel[] Get(Guid mappedSystemId)
        {
            return _customDetailMetadataService.Get(mappedSystemId);
        }

        [Route("CustomDetailMetadata/{mappedSystemId:guid}")]
        [AcceptVerbs("POST")]
        public CustomDetailMetadataViewModel Get(Guid mappedSystemId, CustomDetailMetadataCreateModel model)
        {
            return _customDetailMetadataService.Post(mappedSystemId, model);
        }

        [Route("CustomDetailMetadata/{customDetailMetadataId:guid}/DataStandard/{mappedSystemId:guid}")]
        [AcceptVerbs("PUT")]
        public CustomDetailMetadataViewModel Put(Guid customDetailMetadataId, Guid mappedSystemId, CustomDetailMetadataEditModel model)
        {
            return _customDetailMetadataService.Put(mappedSystemId, customDetailMetadataId, model);
        }

        [Route("CustomDetailMetadata/{customDetailMetadataId:guid}/DataStandard/{mappedSystemId:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid customDetailMetadataId, Guid mappedSystemId)
        {
            _customDetailMetadataService.Delete(mappedSystemId, customDetailMetadataId);
        }
    }
}