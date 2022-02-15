// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MatchmakerOptimization : DbMigration
    {
        private readonly string _createSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Create.sql");
        private readonly string _dropSystemItemSearch = DataAccessHelper.GetStoredProcedurePath("SystemItemSearch", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_createSystemItemSearch));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_dropSystemItemSearch));
        }
    }
}
