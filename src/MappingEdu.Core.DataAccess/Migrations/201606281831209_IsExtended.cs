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
    
    public partial class IsExtended : DbMigration
    {
        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter6.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter5.sql");

        private readonly string _alterGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter3.sql");
        private readonly string _revertGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter2.sql");

        private readonly string _alterGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Alter1.sql");
        private readonly string _createGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Create.sql");
        private readonly string _dropGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Drop.sql");

        public override void Up()
        {
            AddColumn("dbo.SystemItem", "IsExtended", c => c.Boolean(nullable: false));

            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterGetElementListPage));
            Sql(File.ReadAllText(_alterGetMatchmakerSearchPage));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SystemItem", "IsExtended");

            Sql(File.ReadAllText(_revertGetReviewQueuePage));
            Sql(File.ReadAllText(_revertGetElementListPage));

            Sql(File.ReadAllText(_dropGetMatchmakerSearchPage));
            Sql(File.ReadAllText(_createGetMatchmakerSearchPage));
        }

    }
}
