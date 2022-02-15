// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using CuttingEdge.Conditions;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MappingEdu.Core.DataAccess.Migrations.Seed
{
    /// <summary>
    ///     Migration extensions
    /// </summary>
    public static class MigrationExtensions
    {
        public static Organization TryAddOrganization(this EntityContext context, string name)
        {
            Condition.Requires(context, "context").IsNotNull();
            Condition.Requires(name, "name").IsNotNullOrWhiteSpace();

            var organization = context.Organizations.SingleOrDefault(o => o.Name == name);
            if (null == organization)
            {
                organization = new Organization(name);
                context.Organizations.Add(organization);
            }

            return organization;
        }

        public static void TryAddRole(this EntityContext context, string name)
        {
            Condition.Requires(context, "context").IsNotNull();
            Condition.Requires(name, "name").IsNotNullOrWhiteSpace();

            using (var store = new RoleStore<IdentityRole>(context))
            {
                using (var manager = new RoleManager<IdentityRole>(store))
                {
                    var role = manager.FindByName(name);
                    if (role == null)
                    {
                        role = new IdentityRole {Name = name};

                        var result = manager.Create(role);
                        if (result.Succeeded == false)
                        {
                            throw new Exception(result.Errors.First());
                        }
                    }
                }
            }
        }

        public static ApplicationUser TryAddUser(this EntityContext context, string firstName, string lastName, string username, string password, string email, string[] roles)
        {
            Condition.Requires(context, "context").IsNotNull();
            Condition.Requires(firstName, "firstName").IsNotNullOrWhiteSpace();
            Condition.Requires(lastName, "lastName").IsNotNullOrWhiteSpace();
            Condition.Requires(username, "username").IsNotNullOrWhiteSpace();
            Condition.Requires(email, "email").IsNotNullOrWhiteSpace();
            Condition.Requires(password, "password").IsNotNullOrWhiteSpace();

            using (var userStore = new UserStore<ApplicationUser>(context))
            {
                using (var userManager = new UserManager<ApplicationUser>(userStore) {PasswordValidator = new MinimumLengthValidator(8)})
                {
                    var user = userManager.FindByEmail(email);

                    if (user == null)
                    {
                        user = new ApplicationUser {FirstName = firstName, LastName = lastName, UserName = username, Email = email, EmailConfirmed = true};
                        var result = userManager.Create(user, password);
                        if (result.Succeeded == false)
                        {
                            throw new Exception(result.Errors.First());
                        }

                        foreach (var role in roles)
                        {
                            userManager.AddToRole(user.Id, role);
                        }

                        user.IsActive = true;
                    }
                    else
                        user.UserName = username; //Make sure the admin and user have emails

                    user.EmailConfirmed = true; // ensure confirmed on existing

                    return user;
                }
            }
        }
    }
}