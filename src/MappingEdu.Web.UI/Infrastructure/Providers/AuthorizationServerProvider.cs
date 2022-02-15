// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using Autofac;
using Autofac.Integration.Owin;
using MappingEdu.Common.Configuration;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Service.Membership;
using MappingEdu.Web.UI.Infrastructure.Helpers;
using Microsoft.Owin.Security.OAuth;

namespace MappingEdu.Web.UI.Infrastructure.Providers
{
#pragma warning disable 1591
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
#pragma warning disable 1591
    {
        public const string InvalidGrantError = "invalid_grant";
        private IAuthorizationServerHelper _authorizationServerHelper;

        public AuthorizationServerProvider(IAuthorizationServerHelper authorizationServerHelper)
        {
            _authorizationServerHelper = authorizationServerHelper;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var scope = context.OwinContext.GetAutofacLifetimeScope();

                var identity = await _authorizationServerHelper.GenerateIdentity(
                    context.UserName,
                    context.Password,
                    context.Options.AuthenticationType,
                    scope.Resolve<IIdentityFactory>(),
                    scope.Resolve<IConfigurationStore>(),
                    scope.Resolve<IUserService>());

                context.Validated(identity);
            }
            catch (InvalidOperationException ex)
            {
                context.SetError(InvalidGrantError, ex.Message);
            }
        }

        public override async Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            var mappedRoles = "";
            var userRoles = context.Identity.FindAll(ClaimTypes.Role).Select(x => x.Value);
            var userName = context.Identity.FindFirst(ClaimTypes.Name).Value;

            if (userRoles != null)
                mappedRoles = Json.Encode(userRoles);

            context.AdditionalResponseParameters.Add("roles", mappedRoles);
            context.AdditionalResponseParameters.Add("name", userName);

            await base.TokenEndpoint(context);
        }

#pragma warning disable 1998
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
#pragma warning restore 1998
        {
            context.Validated(); // using resource credentials
        }

#pragma warning disable 1998
        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
#pragma warning restore 1998
        {
            context.SetError(InvalidGrantError);
        }
    }
}
