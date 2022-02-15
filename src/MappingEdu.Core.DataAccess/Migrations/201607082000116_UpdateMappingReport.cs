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
    
    public partial class UpdateMappingReport : DbMigration
    {
        private readonly string _alterGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Alter3.sql");
        private readonly string _revertGetMappingProjectReportEnumerationItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectReportEnumerationItemMaps", "Alter2.sql");

        private readonly string _alterGetMappingProjectItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectItemMaps", "Alter2.sql");
        private readonly string _revertGetMappingProjectItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectItemMaps", "Alter1.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetMappingProjectReportEnumerationItemMaps));
            Sql(File.ReadAllText(_alterGetMappingProjectItemMaps));
        }
        
        public override void Down()
        {
            Sql(File.ReadAllText(_revertGetMappingProjectReportEnumerationItemMaps));
            Sql(File.ReadAllText(_revertGetMappingProjectItemMaps));
        }
    }
}
