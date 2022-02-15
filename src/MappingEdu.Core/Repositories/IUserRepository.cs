// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MappingEdu.Core.Domain.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MappingEdu.Core.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> AuthenticateUserAsync(string username, string password);

        ApplicationUser AuthenticateUser(string username, string password);

        ApplicationUser FindByEmail(string email);

        ApplicationUser FindById(string id);

        ApplicationUser FindById(Guid id);

        IQueryable<IdentityRole> GetAvailableRoles();

        IQueryable<ApplicationUser> GetAllUsers();

        ICollection<string> GetUserRoles(string id);

        ICollection<string> GetUserRoles(Guid id);

        bool IsAdministrator(string id);

        bool IsAdministrator(Guid id);

        bool IsGuest(string id);

        bool IsGuest(Guid id);

    }
}