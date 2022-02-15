// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.Home;
using MappingEdu.Service.Model.Home;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller retrieving Home Page Projects and Standards
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [Route("Home")]
        [AcceptVerbs("Get")]
        public HomeViewModel Get()
        {
            return _homeService.Get();
        }
    }
}
