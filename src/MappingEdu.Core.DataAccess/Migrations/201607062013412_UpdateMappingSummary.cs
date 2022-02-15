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
    
    public partial class UpdateMappingSummary : DbMigration
    {
        private readonly string _alterGetMappingProjectSummary = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummary", "Alter1.sql");
        private readonly string _createGetMappingProjectSummary = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummary", "Create.sql");
        private readonly string _dropGetMappingProjectSummary = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectSummary", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetMappingProjectSummary));
        }
        
        public override void Down()
        {
            Sql(File.ReadAllText(_dropGetMappingProjectSummary));
            Sql(File.ReadAllText(_createGetMappingProjectSummary));
        }
    }
}
