// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Common.Configuration;
using System.Security.Claims;

namespace MappingEdu.Core.Domain.Security
{
    public interface IIdentityFactory
    {
        ClaimsIdentity CreateGuestIdentity(string authenticationType, string guestUserId);
        ClaimsIdentity CreateIdentity(string authenticationType, string username, string userId, bool isAdministrator = false);
    }
}
