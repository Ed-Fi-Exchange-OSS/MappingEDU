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
    
    public partial class AddItemTypeToQueue : DbMigration
    {
        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter3.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter2.sql");

        private readonly string _alterGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter1.sql");
        private readonly string _createGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Create.sql");
        private readonly string _dropGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterGetElementListPage));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_revertGetReviewQueuePage));

            Sql(File.ReadAllText(_dropGetElementListPage));
            Sql(File.ReadAllText(_createGetElementListPage));
        }

    }
}
