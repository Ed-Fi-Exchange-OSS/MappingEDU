// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using Autofac;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Mapping;
using MappingEdu.Core.Services.Auditing;
using MappingEdu.Core.Services.Import;
using MappingEdu.Core.Services.Mapping;
using MappingEdu.Core.Services.Validation;
using Module = Autofac.Module;

namespace MappingEdu.Core
{
    /// <summary>
    ///     Module for the core tier
    /// </summary>
    public class CoreModule : Module
    {
        /// <summary>
        ///     Loads the module
        /// </summary>
        /// <param name="builder">The container builder</param>
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // modules

            builder.RegisterModule(new DomainModule());

            // business logic

            builder.RegisterType<BusinessLogicParser>().As<IBusinessLogicParser>().InstancePerLifetimeScope();

            // services (import/export)

            builder.RegisterType<Auditor>().As<IAuditor>().InstancePerLifetimeScope();
            builder.RegisterType<ImportService>().As<IImportService>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlImportService>().As<IOpenXmlImportService>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlSpreadsheetReader>().As<IOpenXmlSpreadsheetReader>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlToSerializedDomainMapper>().As<IOpenXmlToSerializedDomainMapper>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlToSerializedElementMapper>().As<IOpenXmlToSerializedElementMapper>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlToSerializedEntityMapper>().As<IOpenXmlToSerializedEntityMapper>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlToSerializedEnumerationMapper>().As<IOpenXmlToSerializedEnumerationMapper>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlToSerializedEnumerationValueMapper>().As<IOpenXmlToSerializedEnumerationValueMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedDomainToSystemItemMapper>().As<ISerializedDomainToSystemItemMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedElementToSystemItemMapper>().As<ISerializedElementToSystemItemMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedElementToEnumerationTypeMapper>().As<ISerializedElementToEnumerationTypeMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedEntityToSystemItemMapper>().As<ISerializedEntityToSystemItemMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedEnumerationToSystemItemMapper>().As<ISerializedEnumerationToSystemItemMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedToCustomDetailMetadataMapper>().As<ISerializedToCustomDetailMetadataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedToMappedSystemMapper>().As<ISerializedToMappedSystemMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedToSystemItemCustomDetailMapper>().As<ISerializedToSystemItemCustomDetailMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SerializedToSystemItemEnumerationMapper>().As<ISerializedToSystemItemEnumerationMapper>().InstancePerLifetimeScope();
            builder.RegisterType<SpreadsheetDocumentWrapper>().As<ISpreadsheetDocumentWrapper>().InstancePerLifetimeScope();

            // mapper

            builder.RegisterType<Mapper>().As<IMapper>().InstancePerLifetimeScope();

            // validators

            builder
                .RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof (IEntityValidator<,>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof (IRuleProvider<,>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder
                .RegisterAssemblyTypes(assembly)
                .AssignableTo<IValidator>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}