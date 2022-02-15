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
    
    public partial class DashboardProcedureUpdate : DbMigration
    {
        private readonly string _alterGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Alter1.sql");
        private readonly string _createGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Create.sql");
        private readonly string _dropGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetDashboard));
        }
        
        public override void Down()
        {
            Sql(File.ReadAllText(_dropGetDashboard));
            Sql(File.ReadAllText(_createGetDashboard));
        }
    }
}
