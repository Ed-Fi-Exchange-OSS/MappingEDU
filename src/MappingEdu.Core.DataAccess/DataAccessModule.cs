// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using Autofac;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Repositories;
using MappingEdu.Core.DataAccess.Services;
using MappingEdu.Core.DataAccess.Services.Validation;
using MappingEdu.Core.Repositories;
using MappingEdu.Core.Services;
using Module = Autofac.Module;

namespace MappingEdu.Core.DataAccess
{
    /// <summary>
    ///     Module for the data access tier
    /// </summary>
    public class DataAccessModule : Module
    {
        /// <summary>
        ///     Loads the module
        /// </summary>
        /// <param name="builder">The container builder</param>
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // modules

            builder.RegisterModule(new CoreModule());

            // data context

            builder.RegisterType<EntityContext>().InstancePerLifetimeScope();

            // repositories

            builder.RegisterGeneric(typeof (Repository<>)).As(typeof (IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<DataStandardExportRepository>().As<IDataStandardExportRepository>().InstancePerLifetimeScope();
            builder.RegisterType<EnumerationRepository>().As<IEnumerationRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ExecuteBooleanScalar>().As<IExecuteBooleanScalar>().InstancePerLifetimeScope();
            builder.RegisterType<EnumerationRepository>().As<IEnumerationRepository>().InstancePerLifetimeScope();
            builder.RegisterType<EnumerationRepository>().As<IEnumerationRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LoggingRepository>().As<ILoggingRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MappedSystemRepository>().As<IMappedSystemRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectRepository>().As<IMappingProjectRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectReportRepository>().As<IMappingProjectReportRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemMapRepository>().As<ISystemItemMapRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemRepository>().As<ISystemItemRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();

            // services

            builder.RegisterType<BuildVersionMigrator>().As<IBuildVersionMigrator>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseConnection>().As<IDatabaseConnection>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseMigrator>().As<IDatabaseMigrator>().InstancePerLifetimeScope();
            builder.RegisterType<EnumerationEntityCreator>().As<IEnumerationEntityCreator>().InstancePerLifetimeScope();
            builder.RegisterType<FindEntity>().As<IFindEntity>().InstancePerLifetimeScope();

            // validators

            builder
                .RegisterAssemblyTypes(assembly)
                .AssignableTo<IValidateEntity>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}