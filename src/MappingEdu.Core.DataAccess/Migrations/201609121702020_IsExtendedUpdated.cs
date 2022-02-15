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
    
    public partial class IsExtendedUpdated : DbMigration
    {
        private readonly string _alterGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter4.sql");
        private readonly string _revertGetElementListPage = DataAccessHelper.GetStoredProcedurePath("GetElementListPage", "Alter3.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetElementListPage));
        }
        
        public override void Down()
        {
            Sql(File.ReadAllText(_revertGetElementListPage));
        }
    }
}