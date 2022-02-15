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
    
    public partial class IsExtendedSystemItemSearch : DbMigration
    {
        private readonly string _alterSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Alter3.sql");
        private readonly string _revertSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Alter2.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterSystemItemSearch));
        }
        
        public override void Down()
        {
            Sql(File.ReadAllText(_revertSystemItemSearch));
        }
    }
}
