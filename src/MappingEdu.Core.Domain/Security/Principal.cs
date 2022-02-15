// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using CuttingEdge.Conditions;
using MappingEdu.Common;
using MappingEdu.Common.Configuration;
using MappingEdu.Common.Extensions;

namespace MappingEdu.Core.Domain.Security
{
    /// <summary>
    ///     Application claims principal
    /// </summary>
    public class Principal : ClaimsPrincipal
    {
        public Principal(IIdentity identity) : base(identity)
        {
        }

        /// <summary>
        ///     The current principal
        /// </summary>
        public new static Principal Current
        {
            get
            {
                var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
                return null == principal ? null : new Principal(principal.Identities.First());
            }
        }

        /// <summary>
        ///     Indicates if the principal is an administrator.
        /// </summary>
        public bool IsAdministrator
        {
            get { return IsInRole(Constants.Permissions.Administrator); }
        }

        /// <summary>
        ///     Indicates if the principal is a guest.
        /// </summary>
        public bool IsGuest
        {
            get { return IsInRole(Constants.Permissions.Guest); }
        }

        /// <summary>
        ///     The user id
        /// </summary>
        public string UserId
        {
            get
            {
                var claim = FindFirst(o => o.Type == IdentityFactory.UserIdKey);
                return claim != null ? claim.Value : null;
            }
        }

        /// <summary>
        ///     The username
        /// </summary>
        public string Username
        {
            get { return Identity.Name; }
        }
    }
}