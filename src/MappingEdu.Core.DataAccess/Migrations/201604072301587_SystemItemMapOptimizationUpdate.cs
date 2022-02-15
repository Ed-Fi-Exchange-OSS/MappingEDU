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

    public partial class SystemItemMapOptimizationUpdate : DbMigration
    {
        private readonly string _alterGetMappingProjectItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectItemMaps", "Alter1.sql");
        private readonly string _dropGetMappingProjectItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectItemMaps", "Drop.sql");
        private readonly string _createGetMappingProjectItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectItemMaps", "Create.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_alterGetMappingProjectItemMaps));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_dropGetMappingProjectItemMaps));
            Sql(File.ReadAllText(_createGetMappingProjectItemMaps));
        }
    }
}
