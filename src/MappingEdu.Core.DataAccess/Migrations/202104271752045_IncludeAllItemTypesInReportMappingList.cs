// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;
using System.IO;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class IncludeAllItemTypesInReportMappingList : DbMigration
    {
        private readonly string _alterMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Alter5.sql");
        private readonly string _revertMappingProjectReportSourceElementMappings = DataAccessHelper.GetStoredProcedurePath("MappingProjectReportSourceElementMappings", "Alter4.sql");

        public override void Down()
        {
            Sql(File.ReadAllText(_revertMappingProjectReportSourceElementMappings));
        }

        public override void Up()
        {
            Sql(File.ReadAllText(_alterMappingProjectReportSourceElementMappings));
        }
    }
}
