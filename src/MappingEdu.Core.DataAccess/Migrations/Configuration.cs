// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using AutoMapper;
using MappingEdu.Common;
using MappingEdu.Common.Resources;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Migrations.Seed;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Mappings;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.Services;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public class Configuration : DbMigrationsConfiguration<EntityContext>
    {
        private readonly IIdentityFactory _identityFactory;

        public Configuration() : this(new IdentityFactory())
        {
            // This constructor is required for the Entity Framework migration process to work properly.
        }

        public Configuration(IIdentityFactory identityFactory)
        {
            AutomaticMigrationsEnabled = true;
            CommandTimeout = int.MaxValue;
            _identityFactory = identityFactory;
        }

        protected override void Seed(EntityContext context)
        {
            try
            {
                // sql auditing
                var principal = new ClaimsPrincipal(_identityFactory.CreateIdentity("MIGRATION", "SYSTEM", "0", true));
                Thread.CurrentPrincipal = principal;

                Mapper.AddProfile(new DomainMappingProfile());
                Mapper.AssertConfigurationIsValid();

                SystemClock.Now = () => DateTime.Now;

                // Ensure that the System Constants use throughout the Application
                ConfigureSystemConstants(context);

                context.TryAddRole(Constants.Permissions.Administrator);
                context.TryAddRole(Constants.Permissions.Guest);
                context.TryAddRole(Constants.Permissions.User);

                context.TryAddUser("Administrator", "Account", "admin@example.com", "password", "admin@example.com", new[] {Constants.Permissions.Administrator, Constants.Permissions.User});
                context.TryAddUser("Guest", "Account", "guest@example.com", "guest9999", "guest@example.com", new[] { Constants.Permissions.Guest });
                context.TryAddUser("User", "Account", "user@example.com", "password", "user@example.com", new[] {Constants.Permissions.User});

                context.TryAddOrganization("Ed-Fi Alliance");

                AddOrUpdateAllDatabaseEnumerations.Run(context);

                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var message = new StringBuilder();

                foreach (var eve in ex.EntityValidationErrors)
                {
                    message.AppendFormat(ExceptionResources.Error_EntityValidation, eve.Entry.Entity.GetType().Name, eve.Entry.State, ex);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message.AppendLine().AppendFormat(ExceptionResources.Error_EntityValidation_Property, ve.PropertyName, ve.ErrorMessage);
                    }
                }

                throw new Exception(message.ToString());
            }
        }

        private void ConfigureSystemConstants(EntityContext context)
        {
            var docSite = context.SystemConstants.FirstOrDefault(x => x.Name == "Documentation Site");
            if (docSite == null)
                context.SystemConstants.Add(new SystemConstant
                {
                    Name = "Documentation Site",
                    Value = "https://github.com/Ed-Fi-Exchange-OSS/MappingEdu",
                    SystemConstantTypeId = Domain.Enumerations.SystemConstantType.Text.Id
                });

            var termsOfUseUrl = context.SystemConstants.FirstOrDefault(x => x.Name == "Terms of Use Url");
            if (termsOfUseUrl == null)
                context.SystemConstants.Add(new SystemConstant
                {
                    Name = "Terms of Use Url",
                    Value = "https://github.com/Ed-Fi-Exchange-OSS/MappingEdu",
                    SystemConstantTypeId = Domain.Enumerations.SystemConstantType.Text.Id
                });

            var termsOfUse = context.SystemConstants.FirstOrDefault(x => x.Name == "Terms of Use");
            if (termsOfUse == null)
                context.SystemConstants.Add(new SystemConstant
                {
                    Name = "Terms of Use",
                    Value = @"<p>Users may not:</p>
                                  <ul>
                                    <li>Upload or enter data standard definitions that are considered a trade secret by their owner.</li>
                                    <li>Upload or enter data standard definitions that are considered proprietary by their owner.</li>
                                    <li>Upload or enter any copyright-infringing material to the system.</li>
                                  </ul>",
                    SystemConstantTypeId = Domain.Enumerations.SystemConstantType.ComplexText.Id
                });

            var showSignIn = context.SystemConstants.FirstOrDefault(x => x.Name == "Show Sign-In");
            if (showSignIn != null)
                context.SystemConstants.Remove(showSignIn);

        }
    }
}
