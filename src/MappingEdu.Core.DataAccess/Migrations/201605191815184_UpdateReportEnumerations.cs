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
    
    public partial class UpdateReportEnumerations : DbMigration
    {
        private readonly string _alterGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Alter1.sql");
        private readonly string _createGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Create.sql");
        private readonly string _dropGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Drop.sql");

        private readonly string _alterGetMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItems", "Alter1.sql");
        private readonly string _createGetMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItems", "Create.sql");
        private readonly string _dropGetMappingProjectReportEnumerationItems = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItems", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetMappingProjectReportEnumerationItemMaps));
            Sql(File.ReadAllText(_alterGetMappingProjectReportEnumerationItems));
        }
        
        public override void Down()
        {
            Sql(File.ReadAllText(_dropGetMappingProjectReportEnumerationItemMaps));
            Sql(File.ReadAllText(_createGetMappingProjectReportEnumerationItemMaps));

            Sql(File.ReadAllText(_dropGetMappingProjectReportEnumerationItems));
            Sql(File.ReadAllText(_createGetMappingProjectReportEnumerationItems));
        }

    }
}
