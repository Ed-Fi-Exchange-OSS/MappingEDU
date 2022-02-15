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
    
    public partial class ElementDashboardProcedures : DbMigration
    {
        private readonly string _createGetUnmappedAndIncompleteByMappingProjectAndDomain = DataAccessHelper.GetStoredProcedurePath("GetUnmappedAndIncompleteByMappingProjectAndDomain", "Create.sql");
        private readonly string _dropGetUnmappedAndIncompleteByMappingProjectAndDomain = DataAccessHelper.GetStoredProcedurePath("GetUnmappedAndIncompleteByMappingProjectAndDomain", "Drop.sql");

        private readonly string _createGetDashboardElementGroups = DataAccessHelper.GetStoredProcedurePath("GetDashboardElementGroups", "Create.sql");
        private readonly string _dropGetDashboardElementGroups = DataAccessHelper.GetStoredProcedurePath("GetDashboardElementGroups", "Drop.sql");

        public override void Up()
        {
            Sql(File.ReadAllText(_createGetUnmappedAndIncompleteByMappingProjectAndDomain));
            Sql(File.ReadAllText(_createGetDashboardElementGroups));
        }

        public override void Down()
        {
            Sql(File.ReadAllText(_dropGetUnmappedAndIncompleteByMappingProjectAndDomain));
            Sql(File.ReadAllText(_dropGetDashboardElementGroups));
        }
    }
}
