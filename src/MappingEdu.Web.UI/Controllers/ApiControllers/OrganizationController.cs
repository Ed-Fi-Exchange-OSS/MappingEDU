// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MappingEdu.Common;
using MappingEdu.Service.Membership;
using MappingEdu.Service.Model.Membership;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing organizations
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class OrganizationController : ApiController
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        ///     Creates an organization
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage Create(OrganizationCreateModel model)
        {
            var result = _organizationService.CreateOrganization(model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        ///     Deletes an organization
        /// </summary>
        /// <param name="id">The id</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="404">A user was not found with given id</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations/{id:guid}")]
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage Delete(Guid id)
        {
            _organizationService.DeleteOrganization(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Retrieve all organizations
        /// </summary>
        /// <remarks>Returns a collection of all organizations</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (IEnumerable<OrganizationModel>))]
        public HttpResponseMessage GetAll()
        {
            var organizations = _organizationService.GetAllOrganizations();
            return Request.CreateResponse(HttpStatusCode.OK, organizations);
        }

        /// <summary>
        ///     Retrieve all organization users
        /// </summary>
        /// <remarks>Returns a collection of all users for an organization</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations/{id:guid}/users")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (IEnumerable<OrganizationUserModel>))]
        public HttpResponseMessage GetAllOrganizationUsers(Guid id)
        {
            var users = _organizationService.GetAllOrganizationUsers(id);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        /// <summary>
        ///     Adds a user to an organization
        /// </summary>
        /// <param name="id">The organization id</param>
        /// <param name="model">The organization user data</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations/{id:guid}/users")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage AddOrganizationUser(Guid id, OrganizationUserCreateModel model)
        {
            _organizationService.AddOrganizationUser(id, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Removes a user from an organization
        /// </summary>
        /// <param name="id">The organization id</param>
        /// <param name="userId">The user id</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations/{id:guid}/users/{userId}")]
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage DeleteOrganizationUser(Guid id, string userId)
        {
            _organizationService.DeleteOrganizationUser(id, userId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Retrieve a single organization
        /// </summary>
        /// <remarks>Returns a single organization, specified by the id parameter.</remarks>
        /// <param name="id">The id of the desired organization</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="404">An organization was not found with given id</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations/{id:guid}")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (OrganizationModel))]
        public HttpResponseMessage GetSingle(Guid id)
        {
            var organization = _organizationService.FindOrganizationById(id);
            return Request.CreateResponse(HttpStatusCode.OK, organization);
        }

        /// <summary>
        ///     Updates an organization
        /// </summary>
        /// <remarks>Updates a single organization, specified by the id parameter.</remarks>
        /// <param name="model">The organization data</param>
        /// <param name="id">The id</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="404">An organization was not found with given id</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("organizations/{id}")]
        [AcceptVerbs("PUT")]
        public HttpResponseMessage Update(Guid id, OrganizationUpdateModel model)
        {
            _organizationService.UpdateOrganization(id, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}