// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MappingEdu.Common;
using MappingEdu.Service.Membership;
using MappingEdu.Service.Model.Datatables;
using MappingEdu.Service.Model.ElementList;
using MappingEdu.Service.Model.Membership;

namespace MappingEdu.Web.UI.Controllers.ApiControllers
{
    /// <summary>
    ///     Controller for managing elements
    /// </summary>
    [RoutePrefix(Constants.Api.V1.RoutePrefix)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        ///     Creates a new user controller
        /// </summary>
        /// <param name="userService">The security service</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        ///     Finishes Creating a user
        /// </summary>
        /// <remarks>Returns the currently logged in user.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/exists")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> CheckExists(string email)
        {
            var user = await _userService.CheckExistsByEmail(email);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        /// <summary>
        ///     Checks is user already exists
        /// </summary>
        /// <remarks>Returns the currently logged in user.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [AllowAnonymous]
        [Route("users/{id}/confirm")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> ConfirmEmailAsync(string id, string code, UserPasswordResetModel model)
        {
            await _userService.ConfirmEmailAsync(id, code, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Creates a user
        /// </summary>
        /// <remarks>Returns the currently logged in user.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> CreateAsync(UserCreateModel model)
        {
            await _userService.RegisterUserAsync(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Deletes a user
        /// </summary>
        /// <param name="id">The user id</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="404">A user was not found with given id</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/{id}")]
        [AcceptVerbs("DELETE")]
        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            await _userService.DeleteAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Retrieve all users
        /// </summary>
        /// <remarks>Returns a collection of all users.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (IEnumerable<UserModel>))]
        public async Task<HttpResponseMessage> GetAllAsync(string email = null)
        {
            var users = email == null ? await _userService.GetAllUsers() : await _userService.FindAllUserByEmailAsync(email);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        /// <summary>
        ///     Retrieve all users
        /// </summary>
        /// <remarks>Returns a collection of all users.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/paging")]
        [AcceptVerbs("POST")]
        [ResponseType(typeof(IEnumerable<UserModel>))]
        public async Task<HttpResponseMessage> GetAllPagingAsync(DatatablesModel model)
        {
            var result = await _userService.GetAllUsersPaging(model);
            var returnPage = new DatatablesReturnModel<UserModel>
            {
                data = result.Items.ToList(),
                draw = model.draw,
                recordsFiltered = result.TotalFiltered,
                recordsTotal = result.TotalRecords
            };

            return Request.CreateResponse(HttpStatusCode.OK, returnPage);
        }

        /// <summary>
        ///     Retrieve all user's organizations
        /// </summary>
        /// <remarks>Returns a collection of all organizations for a user</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/{id:guid}/organizations")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (IEnumerable<UserOrganizationModel>))]
        public async Task<HttpResponseMessage> GetAllUserOrganizations(string id)
        {
            var organizations = await _userService.GetAllUserOrganizations(id);
            return Request.CreateResponse(HttpStatusCode.OK, organizations);
        }

        /// <summary>
        ///     Retrieve all user's projects
        /// </summary>
        /// <remarks>Returns a collection of all projects associate to a user</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/{id:guid}/projects")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (IEnumerable<UserMappingProjectModel>))]
        public async Task<HttpResponseMessage> GetAllUserProjects(string id)
        {
            var projects = await _userService.GetAllUserProjects(id);
            return Request.CreateResponse(HttpStatusCode.OK, projects);
        }

        /// <summary>
        ///     Retrieve all user's standards
        /// </summary>
        /// <remarks>Returns a collection of all standards associate to a user</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/{id:guid}/standards")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (IEnumerable<UserMappedSystemModel>))]
        public async Task<HttpResponseMessage> GetAllUserStandards(string id)
        {
            var projects = await _userService.GetAllUserStandards(id);
            return Request.CreateResponse(HttpStatusCode.OK, projects);
        }

        /// <summary>
        ///     Retrieve a single user
        /// </summary>
        /// <remarks>Returns a single user, specified by the id parameter.</remarks>
        /// <param name="id">The id of the desired user</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="404">A user was not found with given id</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/{id}")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (UserModel))]
        public async Task<HttpResponseMessage> GetSingleAsync(string id)
        {
            var user = await _userService.FindUserByIdAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        /// <summary>
        ///     Me
        /// </summary>
        /// <remarks>Returns the currently logged in user.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/me")]
        [AcceptVerbs("GET")]
        [ResponseType(typeof (CurrentUserModel))]
        public async Task<HttpResponseMessage> MeAsync()
        {
            var user = await _userService.GetCurrentUserAsync();
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        /// <summary>
        ///     Me
        /// </summary>
        /// <remarks>Returns whether the guest account is active.</remarks>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [AllowAnonymous]
        [Route("users/guest-is-active")]
        [AcceptVerbs("GET")]
        public async Task<HttpResponseMessage> IsGuestAccountActive()
        {
            var isActive = await _userService.IsGuestAccountActive();
            return Request.CreateResponse(HttpStatusCode.OK, isActive);
        }

        /// <summary>
        ///     Resends the user invite email
        /// </summary>
        /// <param name="id">The user id</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="404">A user was not found with given id</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/{id}/resend-email")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> ResendEmail(string id)
        {
            await _userService.ResendActivationEmail(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Resets a users password
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [AllowAnonymous]
        [Route("users/{id}/reset-password")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> ResetPassword(string id, string token, UserPasswordResetModel model)
        {
            await _userService.ResetPassword(id, token, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Sends Reset-Password email to a user
        /// </summary>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="500">An unknown error occurred</response>
        [AllowAnonymous]
        [Route("users/{email}/forgot-password")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> SendResetPasswordEmail(string email)
        {
            await _userService.SendResetPasswordEmail(email);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        ///     Update user
        /// </summary>
        /// <remarks>Updates a single user, specified by the id parameter.</remarks>
        /// <param name="model">The user data</param>
        /// <param name="id">The id</param>
        /// <response code="400">Bad request</response>
        /// <response code="401">Credentials were not provided</response>
        /// <response code="403">Access was denied to the resource</response>
        /// <response code="404">A user was not found with given id</response>
        /// <response code="500">An unknown error occurred</response>
        [Route("users/{id}")]
        [AcceptVerbs("PUT")]
        public async Task<HttpResponseMessage> UpdateAsync(string id, UserUpdateModel model)
        {
            var user = await _userService.UpdateAsync(id, model);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [Route("users/me")]
        [AcceptVerbs("PUT")]
        public async Task<HttpResponseMessage> UpdateCurrentAsync(CurrentUserUpdateModel model)
        {
            var user = await _userService.UpdateCurrentAsync(model);
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [Route("users/{id}/toggle-active")]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> ToggleActive(string id)
        {
            var user = await _userService.ToggleActive(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
