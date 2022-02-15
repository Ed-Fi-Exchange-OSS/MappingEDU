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

    public partial class ExportAndPerformanceProcedures : DbMigration
    {
        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter1.sql");
        private readonly string _createGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Create.sql");
        private readonly string _dropGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Drop.sql");

        private readonly string _alterSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Alter1.sql");
        private readonly string _createSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Create.sql");
        private readonly string _dropSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterSystemItemSearch));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_dropGetReviewQueuePage));
            Sql(File.ReadAllText(_createGetReviewQueuePage));

            Sql(File.ReadAllText(_dropSystemItemSearch));
            Sql(File.ReadAllText(_createSystemItemSearch));
        }

    }
}
