// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Common;
using MappingEdu.Common.Configuration;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Service.Membership;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MappingEdu.Web.UI.Infrastructure.Helpers
{
    // This class exists for being able to add unit tests to some logic
    /// <summary>
    /// Contains logic for Generating the identity 
    /// </summary>
    public class AuthorizationServerHelper : IAuthorizationServerHelper
    {
        /// <summary>
        /// Generates the identity 
        /// </summary>
        public async Task<ClaimsIdentity> GenerateIdentity(
            string userName,
            string password,
            string authenticationType,
            IIdentityFactory _identityFactory,
            IConfigurationStore _configurationStore,
            IUserService _userService)
        {
            var user = await _userService.AuthenticateAsync(userName, password);

            if (null == user)
                throw new InvalidOperationException("The user name or password is incorrect.");

            if (IsGuestAccount(userName, password))
                return _identityFactory.CreateGuestIdentity(authenticationType, user.Id);

            return _identityFactory.CreateIdentity(authenticationType, user.UserName, user.Id, user.IsAdministrator);

        }

        bool IsGuestAccount(string userName, string password)
        {
            return (userName.EqualsIgnoreCase(Constants.Security.GuestUsername)
                && password.EqualsIgnoreCase(Constants.Security.GuestPassword));
        }
    }
}
