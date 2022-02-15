// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Autofac;
using MappingEdu.Core.DataAccess;
using MappingEdu.Service.Admin;
using MappingEdu.Service.AutoMap;
using MappingEdu.Service.Domains;
using MappingEdu.Service.EntityHints;
using MappingEdu.Service.Export;
using MappingEdu.Service.Home;
using MappingEdu.Service.Import;
using MappingEdu.Service.Logging;
using MappingEdu.Service.MappedSystems;
using MappingEdu.Service.MappingProjects;
using MappingEdu.Service.Membership;
using MappingEdu.Service.Providers;
using MappingEdu.Service.SystemItems;
using MappingEdu.Service.SystemItemSelector;
using MappingEdu.Service.SystemItemTree;
using MappingEdu.Service.UserNotifications;

namespace MappingEdu.Service
{
    /// <summary>
    ///     Module for the services tier
    /// </summary>
    public class ServicesModule : Module
    {
        /// <summary>
        ///     Loads the module
        /// </summary>
        /// <param name="builder">The container builder</param>
        protected override void Load(ContainerBuilder builder)
        {
            // module dependencies

            builder.RegisterModule(new DataAccessModule());

            // services

            builder.RegisterType<ConfigurationStatusService>().As<IConfigurationStatusService>().InstancePerLifetimeScope();
            builder.RegisterType<DomainService>().As<IDomainService>().InstancePerLifetimeScope();
            builder.RegisterType<LoggingService>().As<ILoggingService>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof (LoggingProvider<>)).As(typeof (ILoggingProvider<>)).InstancePerLifetimeScope();

            // mapped systems

            builder.RegisterType<DataStandardService>().As<IDataStandardService>().InstancePerLifetimeScope();
            builder.RegisterType<DataStandardCloneService>().As<IDataStandardCloneService>().InstancePerLifetimeScope();
            builder.RegisterType<MappedSystemService>().As<IMappedSystemService>().InstancePerLifetimeScope();
            builder.RegisterType<MappedSystemUserService>().As<IMappedSystemUserService>().InstancePerLifetimeScope();
            builder.RegisterType<NextDataStandardService>().As<INextDataStandardService>().InstancePerLifetimeScope();
            builder.RegisterType<ExportService>().As<IExportService>().InstancePerLifetimeScope();
            builder.RegisterType<ImportService>().As<IImportService>().InstancePerLifetimeScope();
            builder.RegisterType<ImportExtensionsService>().As<IImportExtensionsService>().InstancePerLifetimeScope();
            builder.RegisterType<ImportOdsApiService>().As<IImportOdsApiService>().InstancePerLifetimeScope();
            builder.RegisterType<MappedSystemExtensionService>().As<IMappedSystemExtensionService>().InstancePerLifetimeScope();

            // mapping projects
            builder.RegisterType<MappingProjectDashboardService>().As<IMappingProjectDashboardService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectElementsService>().As<IMappingProjectElementsService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectQueueFilterService>().As<IMappingProjectQueueFilterService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectReportsService>().As<IMappingProjectReportsService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectReviewQueueService>().As<IMappingProjectReviewQueueService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectService>().As<IMappingProjectService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectStatusService>().As<IMappingProjectStatusService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectSummaryService>().As<IMappingProjectSummaryService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectSynonymService>().As<IMappingProjectSynonymService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectTemplateService>().As<IMappingProjectTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingProjectUserService>().As<IMappingProjectUserService>().InstancePerLifetimeScope();

            // auto map
            builder.RegisterType<AutoMapService>().As<IAutoMapService>().InstancePerLifetimeScope();

            // home
            builder.RegisterType<HomeService>().As<IHomeService>().InstancePerLifetimeScope();

            // membership

            builder.RegisterType<OrganizationService>().As<IOrganizationService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

            // system constants

            builder.RegisterType<SystemConstantService>().As<ISystemConstantService>().InstancePerLifetimeScope();

            // system items

            builder.RegisterType<ApproveAllSystemItemMapsService>().As<IApproveAllSystemItemMapsService>().InstancePerLifetimeScope();
            builder.RegisterType<BriefElementService>().As<IBriefElementService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemMapsService>().As<ISystemItemMapsService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomDetailMetadataService>().As<ICustomDetailMetadataService>().InstancePerLifetimeScope();
            builder.RegisterType<ElementDetailsService>().As<IElementDetailsService>().InstancePerLifetimeScope();
            builder.RegisterType<ElementListService>().As<IElementListService>().InstancePerLifetimeScope();
            builder.RegisterType<ElementSearchService>().As<IElementSearchService>().InstancePerLifetimeScope();
            builder.RegisterType<ElementService>().As<IElementService>().InstancePerLifetimeScope();
            builder.RegisterType<ElementSuggestService>().As<IElementSuggestService>().InstancePerLifetimeScope();
            builder.RegisterType<EntityHintService>().As<IEntityHintService>().InstancePerLifetimeScope();
            builder.RegisterType<EntityService>().As<IEntityService>().InstancePerLifetimeScope();
            builder.RegisterType<EnumerationItemMappingService>().As<IEnumerationItemMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<EnumerationItemService>().As<IEnumerationItemService>().InstancePerLifetimeScope();
            builder.RegisterType<EnumerationService>().As<IEnumerationService>().InstancePerLifetimeScope();
            builder.RegisterType<MapNoteService>().As<IMapNoteService>().InstancePerLifetimeScope();
            builder.RegisterType<NewElementService>().As<INewElementService>().InstancePerLifetimeScope();
            builder.RegisterType<NewEntityService>().As<INewEntityService>().InstancePerLifetimeScope();
            builder.RegisterType<NewSystemItemService>().As<INewSystemItemService>().InstancePerLifetimeScope();
            builder.RegisterType<NextVersionDeltaService>().As<INextVersionDeltaService>().InstancePerLifetimeScope();
            builder.RegisterType<NoteService>().As<INoteService>().InstancePerLifetimeScope();
            builder.RegisterType<PreviousVersionDeltaService>().As<IPreviousVersionDeltaService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemService>().As<ISystemItemService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemCustomDetailService>().As<ISystemItemCustomDetailService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemDefinitionService>().As<ISystemItemDefinitionService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemEnumerationService>().As<ISystemItemEnumerationService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemMappingService>().As<ISystemItemMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<SystemItemNameService>().As<ISystemItemNameService>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowStatusService>().As<IWorkflowStatusService>().InstancePerLifetimeScope();

            // system item selector

            builder.RegisterType<SystemItemSelectorService>().As<ISystemItemSelectorService>().InstancePerLifetimeScope();

            // system item tree

            builder.RegisterType<SystemItemTreeService>().As<ISystemItemTreeService>().InstancePerLifetimeScope();

            // user notifications

            builder.RegisterType<UserNotificationService>().As<IUserNotificationService>().InstancePerLifetimeScope();

        }
    }
}
