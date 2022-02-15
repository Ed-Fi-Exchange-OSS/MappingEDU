// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Reflection;
using System.Web.Http;
using log4net;
using MappingEdu.Common;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for returning system health status.
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class HealthController : ControllerBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ISystemItemRepository _systemItemRepository;

        /// <summary>
        /// </summary>
        /// <param name="systemItemRepository"></param>
        public HealthController(ISystemItemRepository systemItemRepository)
        {
            _systemItemRepository = systemItemRepository;
        }

        /// <summary>
        ///     Tests the database connection and keeps system warmed up for load balancing.
        /// </summary>
        [AllowAnonymous]
        [Route("Status")]
        [AcceptVerbs("GET")]
        public bool Status()
        {
            return _systemItemRepository.GetAllItems().Any();
        }
    }
}