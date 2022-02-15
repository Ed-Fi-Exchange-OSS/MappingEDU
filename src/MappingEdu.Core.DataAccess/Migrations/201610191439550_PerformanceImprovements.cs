// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using System.Linq;

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PerformanceImprovements : DbMigration
    {
        private readonly string _createIntId = DataAccessHelper.GetSqlTypesPath("IntId", "Create.sql");
        private readonly string _dropIntId = DataAccessHelper.GetSqlTypesPath("IntId", "Drop.sql");

        private readonly string _createUniqueIdentifierId = DataAccessHelper.GetSqlTypesPath("UniqueIdentifierId", "Create.sql");
        private readonly string _dropUniqueIdentifierId = DataAccessHelper.GetSqlTypesPath("UniqueIdentifierId", "Drop.sql");

        private readonly string _alterGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Alter2.sql");
        private readonly string _revertGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Alter1.sql");

        private readonly string _alterGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Alter2.sql");
        private readonly string _revertGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Alter1.sql");

        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter11.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter10.sql");

        private readonly string _alterGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter5.sql");
        private readonly string _revertGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter4.sql");

        private readonly string _createMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Create.sql");
        private readonly string _dropMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Drop.sql");

        private readonly string _createMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Create.sql");
        private readonly string _dropMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Drop.sql");

        private readonly string _createMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Create.sql");
        private readonly string _dropMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Drop.sql");

        private readonly string _createMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Create.sql");
        private readonly string _dropMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Drop.sql");

        private readonly string _createMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Create.sql");
        private readonly string _dropMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappingss", "Drop.sql");

        private readonly string _createMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Create.sql");
        private readonly string _dropMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Drop.sql");

        private readonly string _dropGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Drop.sql");
        private readonly string _createGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Create.sql");
        private readonly string _alterGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Alter3.sql");

        private readonly string _dropGetMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItems", "Drop.sql");
        private readonly string _createGetMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItems", "Create.sql");
        private readonly string _alterGetMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItems", "Alter1.sql");

        public override void Up()
        {
            // New User Defined Types

            Sql(File.ReadAllText(_createIntId));
            Sql(File.ReadAllText(_createUniqueIdentifierId));

            // List Improvements

            Sql(File.ReadAllText(_alterGetElementListForDeltaPage));
            Sql(File.ReadAllText(_alterGetMatchmakerSearchPage));
            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterGetElementListPage));

            //Report Improvements

            Sql(File.ReadAllText(_createMappingProjectReportElementList));
            Sql(File.ReadAllText(_createMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_createMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_createMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_createMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_createMappingProjectReportTargetEnumerationItemMappings));

            Sql(File.ReadAllText(_dropGetMappingProjectReportEnumerationItemMaps));
            Sql(File.ReadAllText(_dropGetMappingProjectReportEnumerationItems));
        }

        public override void Down()
        {
            // Drop Report Improvements

            Sql(File.ReadAllText(_dropMappingProjectReportElementList));
            Sql(File.ReadAllText(_dropMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_dropMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_dropMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_dropMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_dropMappingProjectReportTargetEnumerationItemMappings));

            Sql(File.ReadAllText(_createGetMappingProjectReportEnumerationItemMaps));
            Sql(File.ReadAllText(_alterGetMappingProjectReportEnumerationItemMaps));

            Sql(File.ReadAllText(_createGetMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_alterGetMappingProjectReportEnumerationItems));

            // Drop List Improvements

            Sql(File.ReadAllText(_revertGetElementListForDeltaPage));
            Sql(File.ReadAllText(_revertGetMatchmakerSearchPage));
            Sql(File.ReadAllText(_revertGetReviewQueuePage));
            Sql(File.ReadAllText(_revertGetElementListPage));

            // Drop User Defined Types

            Sql(File.ReadAllText(_dropIntId));
            Sql(File.ReadAllText(_dropUniqueIdentifierId));
        }
    }
}
