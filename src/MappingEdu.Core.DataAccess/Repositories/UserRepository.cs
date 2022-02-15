// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(EntityContext databaseContext) {

            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(databaseContext));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(databaseContext));
        }

        public Task<ApplicationUser> AuthenticateUserAsync(string username, string password)
        {
            return _userManager.FindAsync(username, password);
        }

        public ApplicationUser AuthenticateUser(string username, string password)
        {
            return _userManager.Find(username, password);
        }

        public ApplicationUser FindByEmail(string email)
        {
            return _userManager.FindByEmail(email);
        }

        public ApplicationUser FindById(string id)
        {
            return _userManager.FindById(id);
        }

        public ApplicationUser FindById(Guid id)
        {
            return _userManager.FindById(id.ToString());
        }

        public IQueryable<IdentityRole> GetAvailableRoles()
        {
            return _roleManager.Roles;
        }

        public IQueryable<ApplicationUser> GetAllUsers()
        {
            return _userManager.Users;
        }

        public ICollection<string> GetUserRoles(string id)
        {
            return _userManager.GetRoles(id);
        }

        public ICollection<string> GetUserRoles(Guid id)
        {
            return _userManager.GetRoles(id.ToString());
        }

        public bool IsAdministrator(string id)
        {
            return _userManager.GetRoles(id).Contains("admin");
        }

        public bool IsAdministrator(Guid id)
        {
            return _userManager.GetRoles(id.ToString()).Contains("admin");
        }

        public bool IsGuest(string id)
        {
            return _userManager.GetRoles(id).Contains("guest");
        }

        public bool IsGuest(Guid id)
        {
            return _userManager.GetRoles(id.ToString()).Contains("guest");
        }
    }
}