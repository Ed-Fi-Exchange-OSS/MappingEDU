// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.Model.MappedSystem;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing mapped systems
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappedSystemController : ControllerBase
    {
        private readonly IMappedSystemService _mappedSystemService;
        private readonly IMappedSystemUserService _mappedSystemUserService;

        public MappedSystemController(IMappedSystemService mappedSystemService, IMappedSystemUserService mappedSystemUserService)
        {
            _mappedSystemService = mappedSystemService;
            _mappedSystemUserService = mappedSystemUserService;
        }

        [Route("MappedSystem/{id:guid}/users")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddUser(Guid id, MappedSystemUserAddModel model)
        {
            var user = _mappedSystemUserService.AddUserToMappedSystem(id, model);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [Route("MappedSystem/{id:guid}/users/{userId}")]
        [AcceptVerbs("DELETE")]
        public void DeleteUser(Guid id, string userId)
        {
            _mappedSystemUserService.RemoveUserFromMappedSystem(id, userId);
        }

        [Route("MappedSystem")]
        [AcceptVerbs("GET")]
        public MappedSystemViewModel[] Get()
        {
            return _mappedSystemService.Get();
        }

        [Route("MappedSystem/{id:guid}")]
        [AcceptVerbs("GET")]
        public MappedSystemViewModel Get(Guid id)
        {
            return _mappedSystemService.Get(id);
        }

        [Route("MappedSystem/{id:guid}/export")]
        [AcceptVerbs("GET")]
        public MappedSystemViewModel Export(Guid id)
        {
            return _mappedSystemService.Get(id);
        }

        [Route("MappedSystem/{systemId:guid}/user/{userId:guid}")]
        [AcceptVerbs("GET")]
        public MappedSystemUserModel GetUser(Guid systemId, Guid userId)
        {
            return _mappedSystemUserService.GetMappedSystemUser(systemId, userId);
        }

        [Route("MappedSystem/{id:guid}/users")]
        [AcceptVerbs("GET")]
        public MappedSystemUserModel[] GetUsers(Guid id)
        {
            return _mappedSystemUserService.GetUsersForMappedSystem(id);
        }

        [Route("MappedSystem/{id:guid}/taggable-users")]
        [AcceptVerbs("GET")]
        public MappedSystemUserModel[] GetTaggableUsers(Guid id)
        {
            return _mappedSystemUserService.GetTaggableUsersForMappedSystem(id);
        }

        [Route("MappedSystem")]
        [AcceptVerbs("POST")]
        public MappedSystemViewModel Post(MappedSystemCreateModel model)
        {
            return _mappedSystemService.Post(model);
        }

        [Route("MappedSystem/{systemId:guid}/user/{userId:guid}/toggle-flag")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage ToggleFlagged(Guid systemId, Guid userId)
        {
            _mappedSystemUserService.ToggleFlagged(systemId, userId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}