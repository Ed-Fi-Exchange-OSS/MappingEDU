// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Common.Configuration;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Service.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MappingEdu.Web.UI.Infrastructure.Helpers
{
    // This interface exists for being able to add unit tests to some logic
    /// <summary>
    /// Contains methods for Generating the identity 
    /// </summary>
    public interface IAuthorizationServerHelper
    {
        /// <summary>
        /// Generates the identity 
        /// </summary>
        Task<ClaimsIdentity> GenerateIdentity(
            string userName,
            string password,
            string authenticationType,
            IIdentityFactory _identityFactory,
            IConfigurationStore _configurationStore,
            IUserService _userService);
    }
}