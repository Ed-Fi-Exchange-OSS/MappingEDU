// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using CuttingEdge.Conditions;
using MappingEdu.Common;
using MappingEdu.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MappingEdu.Core.Domain.Security
{
    public class IdentityFactory : IIdentityFactory
    {
        public const string IssuedKey = "http://mappingedu/identity/claims/issued";
        public const string UserIdKey = "http://mappingedu/identity/claims/userid";

        /// <summary>
        ///     Creates a new guest user principal
        /// </summary>
        /// <param name="authenticationType"></param>
        public ClaimsIdentity CreateGuestIdentity(string authenticationType, string guestUserId)
        {
            Condition.Requires(authenticationType, "authenticationType").IsNotNullOrWhiteSpace();
            Condition.Requires(guestUserId, "userId").IsNotNullOrWhiteSpace().IsLongerOrEqual(1);

            var claims = new List<Claim>
            {
                new Claim(IssuedKey, DateTime.UtcNow.ToUnixTime().ToString()),
                new Claim(UserIdKey, guestUserId),
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.Role, Constants.Permissions.Guest)
            };

            var identity = new ClaimsIdentity(authenticationType);
            identity.AddClaims(claims);

            return identity;
        }

        /// <summary>
        ///     Creates a new principal
        /// </summary>
        /// <param name="authenticationType">Standard value is "bearer" except when bypassing authentication for integration test automation</param>
        /// <param name="username">The username</param>
        /// <param name="userId">The user id</param>
        /// <param name="isAdministrator">Indicates if user is administrator</param>
        public ClaimsIdentity CreateIdentity(string authenticationType, string username, string userId, bool isAdministrator = false)
        {
            Condition.Requires(authenticationType, "authenticationType").IsNotNullOrWhiteSpace();
            Condition.Requires(username, "username").IsNotNullOrWhiteSpace().IsLongerOrEqual(1);
            Condition.Requires(username, "userId").IsNotNullOrWhiteSpace().IsLongerOrEqual(1);

            var claims = new List<Claim>
            {
                new Claim(IssuedKey, DateTime.UtcNow.ToUnixTime().ToString()),
                new Claim(UserIdKey, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, Constants.Permissions.User)
            };

            if (isAdministrator)
                claims.Add(new Claim(ClaimTypes.Role, Constants.Permissions.Administrator));

            var identity = new ClaimsIdentity(authenticationType);
            identity.AddClaims(claims);

            return identity;
        }
    }
}
