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
    
    public partial class ElementListProcedures : DbMigration
    {
        private readonly string _createGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Create.sql");
        private readonly string _dropGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Drop.sql");

        private readonly string _createGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Create.sql");
        private readonly string _dropGetElementListForDeltaPage = DataAccessHelper.GetStoredProcedurePath("GetElementListForDeltaPage", "Drop.sql");

        private readonly string _createGetDashboardElementGroups = DataAccessHelper.GetStoredProcedurePath("GetDashboardElementGroups", "Create.sql");
        private readonly string _dropGetDashboardElementGroups = DataAccessHelper.GetStoredProcedurePath("GetDashboardElementGroups", "Drop.sql");

        private readonly string _createGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Create.sql");
        private readonly string _dropGetDashboard = DataAccessHelper.GetStoredProcedurePath("GetDashboard", "Drop.sql");

        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter2.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter1.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_createGetElementListPage));
            Sql(File.ReadAllText(_createGetElementListForDeltaPage));
            Sql(File.ReadAllText(_createGetDashboard));

            Sql(File.ReadAllText(_alterGetReviewQueuePage));

            Sql(File.ReadAllText(_dropGetDashboardElementGroups));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_dropGetElementListPage));
            Sql(File.ReadAllText(_dropGetElementListForDeltaPage));
            Sql(File.ReadAllText(_dropGetDashboard));

            Sql(File.ReadAllText(_revertGetReviewQueuePage));

            Sql(File.ReadAllText(_createGetDashboardElementGroups));
        }
    }
}
