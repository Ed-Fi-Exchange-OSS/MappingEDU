// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Model.NewSystemItem;
using MappingEdu.Service.SystemItems;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing new system items
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class NewSystemItemController : ControllerBase
    {
        private readonly INewSystemItemService _newSystemItemService;

        public NewSystemItemController(INewSystemItemService newSystemItemService)
        {
            _newSystemItemService = newSystemItemService;
        }

        [Route("NewSystemItem")]
        [AcceptVerbs("POST")]
        public NewSystemItemViewModel Post(NewSystemItemCreateModel model)
        {
            return _newSystemItemService.Post(model);
        }
    }
}