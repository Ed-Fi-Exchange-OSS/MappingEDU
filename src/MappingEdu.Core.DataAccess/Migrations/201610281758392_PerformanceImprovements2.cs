// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PerformanceImprovements2 : DbMigration
    {
        private readonly string _alterGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Alter2.sql");
        private readonly string _revertGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Alter1.sql");

        private readonly string _alterGetDashboardQueueFilters = DataAccessHelper.GetStoredProcedurePath("GetDashboardQueueFilters", "Alter1.sql");
        private readonly string _createGetDashboardQueueFilters = DataAccessHelper.GetStoredProcedurePath("GetDashboardQueueFilters", "Create.sql");
        private readonly string _dropGetDashboardQueueFilters = DataAccessHelper.GetStoredProcedurePath("GetDashboardQueueFilters", "Drop.sql");

        private readonly string _alterGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Alter3.sql");
        private readonly string _revertGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Alter2.sql");

        private readonly string _alterGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter6.sql");
        private readonly string _revertGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter5.sql");

        private readonly string _alterGetMappingProjectSummary = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummary", "Alter3.sql");
        private readonly string _revertGetMappingProjectSummary = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummary", "Alter2.sql");

        private readonly string _alterGetMappingProjectSummaryDetail = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummaryDetail", "Alter1.sql");
        private readonly string _createGetMappingProjectSummaryDetail = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummaryDetail", "Create.sql");
        private readonly string _dropGetMappingProjectSummaryDetail = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummaryDetail", "Drop.sql");

        private readonly string _alterGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Alter3.sql");
        private readonly string _revertGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Alter2.sql");

        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter12.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter11.sql");

        private readonly string _alterMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Alter1.sql");
        private readonly string _createMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Create.sql");
        private readonly string _dropMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Drop.sql");

        private readonly string _alterMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Alter1.sql");
        private readonly string _createMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Create.sql");
        private readonly string _dropMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Drop.sql");

        private readonly string _alterMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Alter1.sql");
        private readonly string _createMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Create.sql");
        private readonly string _dropMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Drop.sql");

        private readonly string _alterMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Alter1.sql");
        private readonly string _createMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Create.sql");
        private readonly string _dropMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Drop.sql");

        private readonly string _alterMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Alter1.sql");
        private readonly string _createMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Create.sql");
        private readonly string _dropMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Drop.sql");

        private readonly string _alterMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Alter1.sql");
        private readonly string _createMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Create.sql");
        private readonly string _dropMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Drop.sql");

        private readonly string _createDataStandardExportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportEnumerationItems", "Create.sql");
        private readonly string _dropDataStandardExportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportEnumerationItems", "Drop.sql");

        private readonly string _createDataStandardExportSystemItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportSystemItems", "Create.sql");
        private readonly string _dropDataStandardExportSystemItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportSystemItems", "Drop.sql");

        private readonly string _createUpdateAllPaths = DataAccessHelper.GetStoredProcedurePath("UpdateAllPaths", "Create.sql");
        private readonly string _dropUpdateAllPaths = DataAccessHelper.GetStoredProcedurePath("UpdateAllPaths", "Drop.sql");

        private readonly string _createUpdatePaths = DataAccessHelper.GetStoredProcedurePath("UpdatePaths", "Create.sql");
        private readonly string _dropUpdatePaths = DataAccessHelper.GetStoredProcedurePath("UpdatePaths", "Drop.sql");

        public override void Up()
        {
            DropColumn("dbo.SystemItem", "ElementGroupSystemItemId");
            RenameColumn(table: "dbo.SystemItem", name: "ElementGroupSystemItem_SystemItemId", newName: "ElementGroupSystemItemId");
            RenameIndex(table: "dbo.SystemItem", name: "IX_ElementGroupSystemItem_SystemItemId", newName: "IX_ElementGroupSystemItemId");
            AddColumn("dbo.SystemItem", "DomainItemPath", c => c.String());
            AddColumn("dbo.SystemItem", "DomainItemPathIds", c => c.String());
            AddColumn("dbo.SystemItem", "IsExtendedPath", c => c.String());

            Sql(@"
                with result as (
	                SELECT CAST(ItemName AS NVARCHAR(MAX)) AS DomainItemPath,
	                       CAST(SystemItemId AS NVARCHAR(MAX)) as DomainItemPathIds,
                           SystemItemId as ElementGroupId,
		                   CAST(IsExtended AS NVARCHAR(MAX)) as IsExtendedPath,
	                       ParentSystemItemId,
	                       SystemItemId
                    FROM dbo.SystemItem
                    WHERE ParentSystemItemId IS NULL
                    UNION all 
                    SELECT result.DomainItemPath + '.' + i2.ItemName,
                           result.DomainItemPathIds + '/' + CAST(i2.SystemItemId AS VARCHAR(50)),
                           result.ElementGroupId,
		                   result.IsExtendedPath + '/' + CAST(i2.IsExtended AS VARCHAR(50)),
		                   i2.ParentSystemItemId,
	                       i2.SystemItemId
                    FROM SystemItem AS i2
                    inner join result
                        ON result.SystemItemId = i2.ParentSystemItemId)

                UPDATE si
                SET si.DomainItemPath = paths.DomainItemPath,
                    si.DomainItemPathIds = paths.DomainItemPathIds,
	                si.IsExtendedPath = paths.IsExtendedPath,
                    si.ElementGroupSystemItemId = paths.ElementGroupId
                FROM SystemItem si
                JOIN result paths on si.SystemItemId = paths.SystemItemId");

            Sql(File.ReadAllText(_createDataStandardExportSystemItems));
            Sql(File.ReadAllText(_createDataStandardExportEnumerationItems));
            Sql(File.ReadAllText(_createUpdateAllPaths));
            Sql(File.ReadAllText(_createUpdatePaths));

            Sql(File.ReadAllText(_alterGetDashboard));
            Sql(File.ReadAllText(_alterGetDashboardQueueFilters));
            Sql(File.ReadAllText(_alterGetElementListForDeltaPage));
            Sql(File.ReadAllText(_alterGetElementListPage));
            Sql(File.ReadAllText(_alterGetMappingProjectSummary));
            Sql(File.ReadAllText(_alterGetMappingProjectSummaryDetail));
            Sql(File.ReadAllText(_alterGetMatchmakerSearchPage));
            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterMappingProjectReportElementList));
            Sql(File.ReadAllText(_alterMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_alterMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportTargetEnumerationItemMappings));
        }
        
        public override void Down()
        {
            Sql(File.ReadAllText(_dropDataStandardExportEnumerationItems));
            Sql(File.ReadAllText(_dropDataStandardExportSystemItems));
            Sql(File.ReadAllText(_dropUpdateAllPaths));
            Sql(File.ReadAllText(_dropUpdatePaths));

            Sql(File.ReadAllText(_revertGetDashboard));
            Sql(File.ReadAllText(_revertGetElementListForDeltaPage));
            Sql(File.ReadAllText(_revertGetElementListPage));
            Sql(File.ReadAllText(_revertGetMappingProjectSummary));
            Sql(File.ReadAllText(_revertGetMatchmakerSearchPage));
            Sql(File.ReadAllText(_revertGetReviewQueuePage));

            Sql(File.ReadAllText(_dropGetDashboardQueueFilters));
            Sql(File.ReadAllText(_createGetDashboardQueueFilters));

            Sql(File.ReadAllText(_dropGetMappingProjectSummaryDetail));
            Sql(File.ReadAllText(_createGetMappingProjectSummaryDetail));

            Sql(File.ReadAllText(_dropMappingProjectReportElementList));
            Sql(File.ReadAllText(_createMappingProjectReportElementList));

            Sql(File.ReadAllText(_dropMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_createMappingProjectReportEnumerationItems));

            Sql(File.ReadAllText(_dropMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_createMappingProjectReportSourceElementMappings));

            Sql(File.ReadAllText(_dropMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_createMappingProjectReportSourceEnumerationItemMappings));

            Sql(File.ReadAllText(_dropMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_createMappingProjectReportTargetElementMappings));

            Sql(File.ReadAllText(_dropMappingProjectReportTargetEnumerationItemMappings));
            Sql(File.ReadAllText(_createMappingProjectReportTargetEnumerationItemMappings));

            DropColumn("dbo.SystemItem", "IsExtendedPath");
            DropColumn("dbo.SystemItem", "DomainItemPathIds");
            DropColumn("dbo.SystemItem", "DomainItemPath");
            RenameIndex(table: "dbo.SystemItem", name: "IX_ElementGroupSystemItemId", newName: "IX_ElementGroupSystemItem_SystemItemId");
            RenameColumn(table: "dbo.SystemItem", name: "ElementGroupSystemItemId", newName: "ElementGroupSystemItem_SystemItemId");
            AddColumn("dbo.SystemItem", "ElementGroupSystemItemId", c => c.Guid());
        }
    }
}
