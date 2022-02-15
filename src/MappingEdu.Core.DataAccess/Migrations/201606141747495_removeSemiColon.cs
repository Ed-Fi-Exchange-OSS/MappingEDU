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
    
    public partial class removeSemiColon : DbMigration
    {
        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter5.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter4.sql");

        private readonly string _alterGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter2.sql");
        private readonly string _reverGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter1.sql");

        private readonly string _alterGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Alter1.sql");
        private readonly string _createGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Create.sql");
        private readonly string _dropGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterGetElementListPage));
            Sql(File.ReadAllText(_alterGetElementListForDeltaPage));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_revertGetReviewQueuePage));
            Sql(File.ReadAllText(_reverGetElementListPage));

            Sql(File.ReadAllText(_dropGetElementListForDeltaPage));
            Sql(File.ReadAllText(_createGetElementListForDeltaPage));
        }
    }
}
