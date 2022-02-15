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
    
    public partial class CustomDetailPivotFix : DbMigration
    {
        private readonly string _alterDataStandardExportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportEnumerationItems", "Alter2.sql");
        private readonly string _revertDataStandardExportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportEnumerationItems", "Alter1.sql");

        private readonly string _alterDataStandardExportSystemItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportSystemItems", "Alter2.sql");
        private readonly string _revertDataStandardExportSystemItems = DataAccessHelper.GetStoredProcedurePath("DataStandardExportSystemItems", "Alter1.sql");

        private readonly string _alterMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Alter3.sql");
        private readonly string _revertMappingProjectReportElementList = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportElementList", "Alter2.sql");

        private readonly string _alterMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Alter3.sql");
        private readonly string _revertMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportEnumerationItems", "Alter2.sql");

        private readonly string _alterMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Alter3.sql");
        private readonly string _revertMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Alter2.sql");

        private readonly string _alterMappingProjectReportSourceEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceEnumerationItemMappings", "Alter3.sql");
        private readonly string _revertMappingProjectReportSourceEnumerationItemMappings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin" }, StringSplitOptions.None).First(), "bin/StoredProcedures/", "MappingProjectReportSourceEnumerationItemMappings", "Alter2.sql");

        private readonly string _alterMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Alter3.sql");
        private readonly string _revertMappingProjectReportTargetElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetElementMappings", "Alter2.sql");

        private readonly string _alterMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Alter3.sql");
        private readonly string _revertMappingProjectReportTargetEnumerationItemMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportTargetEnumerationItemMappings", "Alter2.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterDataStandardExportEnumerationItems));
            Sql(File.ReadAllText(_alterDataStandardExportSystemItems));
            Sql(File.ReadAllText(_alterMappingProjectReportElementList));
            Sql(File.ReadAllText(_alterMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_alterMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_alterMappingProjectReportTargetEnumerationItemMappings));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_revertDataStandardExportEnumerationItems));
            Sql(File.ReadAllText(_revertDataStandardExportSystemItems));
            Sql(File.ReadAllText(_revertMappingProjectReportElementList));
            Sql(File.ReadAllText(_revertMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_revertMappingProjectReportSourceElementMappings));
            Sql(File.ReadAllText(_revertMappingProjectReportSourceEnumerationItemMappings));
            Sql(File.ReadAllText(_revertMappingProjectReportTargetElementMappings));
            Sql(File.ReadAllText(_revertMappingProjectReportTargetEnumerationItemMappings));
        }
    }
}
