// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MappingEdu.Common;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing projects
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class MappingProjectController : ControllerBase
    {
        private readonly IMappingProjectService _mappingProjectService;
        private readonly IMappingProjectUserService _mappingProjectUserService;

        public MappingProjectController(IMappingProjectService mappingProjectService, IMappingProjectUserService mappingProjectUserService)
        {
            _mappingProjectService = mappingProjectService;
            _mappingProjectUserService = mappingProjectUserService;
        }

        [Route("MappingProject/{id:guid}/users")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> AddUser(Guid id, MappingProjectUserAddModel model)
        {
            MappingProjectUserModel user = await _mappingProjectUserService.AddUserToMappingProject(id, model);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [Route("MappingProject/{id:guid}")]
        [AcceptVerbs("DELETE")]
        public void Delete(Guid id)
        {
            _mappingProjectService.Delete(id);
        }

        [Route("MappingProject/{id:guid}/users/{userId}")]
        [AcceptVerbs("DELETE")]
        public void DeleteUser(Guid id, string userId)
        {
            _mappingProjectUserService.RemoveUserFromMappingProject(id, userId);
        }

        [Route("MappingProject")]
        [AcceptVerbs("GET")]
        public MappingProjectViewModel[] Get()
        {
            return _mappingProjectService.Get();
        }

        [Route("MappingProject/orphaned")]
        [AcceptVerbs("GET")]
        public MappingProjectViewModel[] GetOrphaned()
        {
            return _mappingProjectService.Get(true);
        }

        [Route("MappingProject/public")]
        [AcceptVerbs("GET")]
        public MappingProjectViewModel[] GetPublic()
        {
            return _mappingProjectService.GetPublic();
        }

        [Route("MappingProject/{id:guid}")]
        [AcceptVerbs("GET")]
        public MappingProjectViewModel Get(Guid id)
        {
            return _mappingProjectService.Get(id);
        }

        [Route("MappingProject/{id:guid}/owners")]
        [AcceptVerbs("GET")]
        public List<string> GetOwners(Guid id)
        {
            return _mappingProjectService.GetOwners(id);
        }

        [Route("MappingProject/{id:guid}/creator")]
        [AcceptVerbs("GET")]
        public string GetCreator(Guid id)
        {
            return _mappingProjectService.GetCreator(id);
        }

        [Route("MappingProject/{projectId:guid}/user/{userId:guid}")]
        [AcceptVerbs("GET")]
        public MappingProjectUserModel GetUserAccess(Guid projectId, Guid userId)
        {
            return _mappingProjectUserService.GetMappingProjectUser(projectId, userId);
        }

        [Route("MappingProject/{id:guid}/users")]
        [AcceptVerbs("GET")]
        public MappingProjectUserModel[] GetUsers(Guid id)
        {
            return _mappingProjectUserService.GetUsersForMappingProject(id);
        }


        [Route("MappingProject/{id:guid}/taggable-users")]
        [AcceptVerbs("GET")]
        public MappingProjectUserModel[] GetTaggableUsers(Guid id)
        {
            return _mappingProjectUserService.GetTaggableUsersForMappingProjet(id);
        }

        [Route("MappingProject")]
        [AcceptVerbs("POST")]
        public MappingProjectViewModel Post(MappingProjectCreateModel model, bool autoMap = false)
        {
            return _mappingProjectService.Post(model, autoMap);
        }

        [Route("MappingProject/{mappingProjectId:guid}/clone")]
        [AcceptVerbs("POST")]
        public MappingProjectViewModel Clone(Guid mappingProjectId, MappingProjectCloneModel model)
        {
            return _mappingProjectService.Clone(mappingProjectId, model);
        }

        [Route("MappingProject/{id:guid}")]
        [AcceptVerbs("PUT")]
        public MappingProjectViewModel Put(Guid id, MappingProjectEditModel model)
        {
            return _mappingProjectService.Put(id, model);
        }

        [Route("MappingProject/{projectId:guid}/user/{userId:guid}/toggle-flag")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage ToggleFlagged(Guid projectId, Guid userId)
        {
            _mappingProjectUserService.ToggleFlagged(projectId, userId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("MappingProject/{projectId:guid}/toggle-public")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage TogglePublic(Guid projectId)
        {
            _mappingProjectService.TogglePublic(projectId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
