// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Public : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MappingProject", "IsPublic", c => c.Boolean(nullable: false));
            AddColumn("dbo.MappedSystem", "IsPublic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MappedSystem", "IsPublic");
            DropColumn("dbo.MappingProject", "IsPublic");
        }
    }
}
