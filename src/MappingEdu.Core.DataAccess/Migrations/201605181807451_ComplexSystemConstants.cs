// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplexSystemConstants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemConstant", "IsComplex", c => c.Boolean(nullable: false));
            AlterColumn("dbo.SystemConstant", "Name", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.SystemConstant", "Name", unique: true, name: "UX_Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SystemConstant", "UX_Name");
            AlterColumn("dbo.SystemConstant", "Name", c => c.String());
            DropColumn("dbo.SystemConstant", "IsComplex");
        }
    }
}
