// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MappedSystemExtensions : DbMigration
    {
        private readonly string _createExtensionReport = DataAccessHelper.GetStoredProcedurePath("ExtensionReport", "Create.sql");
        private readonly string _dropExtensionReport = DataAccessHelper.GetStoredProcedurePath("ExtensionReport", "Drop.sql");

        private readonly string _alterGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter7.sql");
        private readonly string _revertGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter6.sql");

        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter13.sql");
        private readonly string _reverGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter12.sql");

        private readonly string _alterGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Alter3.sql");
        private readonly string _revertGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Alter2.sql");

        private readonly string _alterGetDashboardQueueFilters = DataAccessHelper.GetStoredProcedurePath("GetDashboardQueueFilters", "Alter2.sql");
        private readonly string _revertGetDashboardQueueFilters = DataAccessHelper.GetStoredProcedurePath("GetDashboardQueueFilters", "Alter1.sql");

        private readonly string _alterGetMappingProjectSummary = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummary", "Alter4.sql");
        private readonly string _revertGetMappingProjectSummary = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummary", "Alter3.sql");

        private readonly string _alterGetMappingProjectSummaryDetail = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummaryDetail", "Alter2.sql");
        private readonly string _revertGetMappingProjectSummaryDetail = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummaryDetail", "Alter1.sql");

        private readonly string _alterGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Alter4.sql");
        private readonly string _revertGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Alter3.sql");

        private readonly string _alterDataStandardExportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportEnumerationItems", "Alter1.sql");
        private readonly string _dropDataStandardExportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportEnumerationItems", "Drop.sql");
        private readonly string _createDataStandardExportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportEnumerationItems", "Create.sql");

        private readonly string _alterDataStandardExportSystemItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportSystemItems", "Alter1.sql");
        private readonly string _dropDataStandardExportSystemItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportSystemItems", "Drop.sql");
        private readonly string _createDataStandardExportSystemItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportSystemItems", "Create.sql");

        private readonly string _alterGetUnmappedAndIncompleteByMappingProjectAndDomain = DataAccessHelper.GetStoredProcedurePath("GetUnmappedAndIncompleteByMappingProjectAndDomain", "Alter1.sql");
        private readonly string _dropGetUnmappedAndIncompleteByMappingProjectAndDomain = DataAccessHelper.GetStoredProcedurePath("GetUnmappedAndIncompleteByMappingProjectAndDomain", "Drop.sql");
        private readonly string _createGetUnmappedAndIncompleteByMappingProjectAndDomain = DataAccessHelper.GetStoredProcedurePath("GetUnmappedAndIncompleteByMappingProjectAndDomain", "Create.sql");

        private readonly string _alterMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Alter2.sql");
        private readonly string _revertMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Alter1.sql");

        private readonly string _alterMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Alter2.sql");
        private readonly string _revertMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Alter1.sql");

        private readonly string _alterMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Alter2.sql");
        private readonly string _revertMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Alter1.sql");

        private readonly string _alterMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Alter2.sql");
        private readonly string _revertMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Alter1.sql");

        private readonly string _alterMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Alter2.sql");
        private readonly string _revertMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Alter1.sql");

        private readonly string _alterMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Alter2.sql");
        private readonly string _revertMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Alter1.sql");

        private readonly string _dropSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Drop.sql");
        private readonly string _createSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Create.sql");
        private readonly string _alterSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Alter3.sql");

        public override void Up()
        {
            CreateTable(
                "dbo.MappedSystemExtension",
                c => new
                {
                    MappedSystemExtensionId = c.Guid(nullable: false, identity: true),
                    ExtensionMappedSystemId = c.Guid(),
                    MappedSystemId = c.Guid(nullable: false),
                    ShortName = c.String(maxLength: 5),
                    CreateBy = c.String(nullable: false, maxLength: 120),
                    CreateById = c.Guid(),
                    UpdateBy = c.String(maxLength: 120),
                    CreateDate = c.DateTime(nullable: false),
                    UpdateDate = c.DateTime(),
                    UpdateById = c.Guid(),
                })
                .PrimaryKey(t => t.MappedSystemExtensionId)
                .ForeignKey("dbo.MappedSystem", t => t.ExtensionMappedSystemId)
                .ForeignKey("dbo.MappedSystem", t => t.MappedSystemId)
                .Index(t => t.ExtensionMappedSystemId)
                .Index(t => t.MappedSystemId);

            AddColumn("dbo.SystemItem", "CopiedFromSystemItemId", c => c.Guid());
            AddColumn("dbo.SystemItem", "MappedSystemExtensionId", c => c.Guid());
            CreateIndex("dbo.SystemItem", "CopiedFromSystemItemId");
            CreateIndex("dbo.SystemItem", "MappedSystemExtensionId");
            AddForeignKey("dbo.SystemItem", "CopiedFromSystemItemId", "dbo.SystemItem", "SystemItemId");
            AddForeignKey("dbo.SystemItem", "MappedSystemExtensionId", "dbo.MappedSystemExtension", "MappedSystemExtensionId");

            Sql(File.ReadAllText(_createExtensionReport));
            Sql(File.ReadAllText(_alterGetElementListPage));
            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterGetDashboard));
            Sql(File.ReadAllText(_alterGetDashboardQueueFilters));
            Sql(File.ReadAllText(_alterGetMappingProjectSummary));
            Sql(File.ReadAllText(_alterGetMappingProjectSummaryDetail));
            Sql(File.ReadAllText(_alterGetMatchmakerSearchPage));
            Sql(File.ReadAllText(_alterDataStandardExportEnumerationItems));
            Sql(File.ReadAllText(_alterDataStandardExportSystemItems));
            Sql(File.ReadAllText(_alterGetUnmappedAndIncompleteByMappingProjectAndDomain));
            Sql(File.ReadAllText(_alterMappingProjectReportElementList));
            Sql(File.ReadAllText(_alterMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_alterMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportTargetEnumerationItemMappings));

            Sql(File.ReadAllText(_dropSystemItemSearch));
        }

        public override void Down()
        {
            DropForeignKey("dbo.SystemItem", "MappedSystemExtensionId", "dbo.MappedSystemExtension");
            DropForeignKey("dbo.MappedSystemExtension", "MappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.MappedSystemExtension", "ExtensionMappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.SystemItem", "CopiedFromSystemItemId", "dbo.SystemItem");
            DropIndex("dbo.MappedSystemExtension", new[] {"MappedSystemId"});
            DropIndex("dbo.MappedSystemExtension", new[] {"ExtensionMappedSystemId"});
            DropIndex("dbo.SystemItem", new[] {"MappedSystemExtensionId"});
            DropIndex("dbo.SystemItem", new[] {"CopiedFromSystemItemId"});
            DropColumn("dbo.SystemItem", "MappedSystemExtensionId");
            DropColumn("dbo.SystemItem", "CopiedFromSystemItemId");
            DropTable("dbo.MappedSystemExtension");

            Sql(File.ReadAllText(_dropExtensionReport));
            Sql(File.ReadAllText(_revertGetElementListPage));
            Sql(File.ReadAllText(_reverGetReviewQueuePage));
            Sql(File.ReadAllText(_revertGetDashboard));
            Sql(File.ReadAllText(_revertGetDashboardQueueFilters));
            Sql(File.ReadAllText(_revertGetMappingProjectSummary));
            Sql(File.ReadAllText(_revertGetMappingProjectSummaryDetail));
            Sql(File.ReadAllText(_revertGetMatchmakerSearchPage));

            Sql(File.ReadAllText(_dropDataStandardExportEnumerationItems));
            Sql(File.ReadAllText(_createDataStandardExportEnumerationItems));

            Sql(File.ReadAllText(_dropDataStandardExportSystemItems));
            Sql(File.ReadAllText(_createDataStandardExportSystemItems));

            Sql(File.ReadAllText(_dropGetUnmappedAndIncompleteByMappingProjectAndDomain));
            Sql(File.ReadAllText(_createGetUnmappedAndIncompleteByMappingProjectAndDomain));

            Sql(File.ReadAllText(_revertMappingProjectReportElementList));
            Sql(File.ReadAllText(_revertMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_revertMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_revertMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_revertMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_revertMappingProjectReportTargetEnumerationItemMappings));

            Sql(File.ReadAllText(_createSystemItemSearch));
            Sql(File.ReadAllText(_alterSystemItemSearch));
        }
    }
}
