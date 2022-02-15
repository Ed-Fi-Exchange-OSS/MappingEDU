// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemConstantType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemConstantType",
                c => new
                {
                    SystemConstantTypeId = c.Int(nullable: false),
                    SystemConstantTypeName = c.String(nullable: false, maxLength: 20),
                });

            AddColumn("dbo.SystemConstant", "SystemConstantTypeId", c => c.Int(nullable: true));

            // 2 is ComplexText, 1 is Text
            Sql(@"Update constant
                  SET constant.SystemConstantTypeId = CASE WHEN constant.IsComplex = 1 THEN 2 ELSE 1 END
                  FROM SystemConstant constant");

            AlterColumn("dbo.SystemConstant", "SystemConstantTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.SystemConstant", "SystemConstantTypeId");
            DropColumn("dbo.SystemConstant", "IsComplex");

            Sql("INSERT INTO SystemConstantType (SystemConstantTypeId, SystemConstantTypeName) Values (0, 'Unknown')");
            Sql("INSERT INTO SystemConstantType (SystemConstantTypeId, SystemConstantTypeName) Values (1, 'Text')");
            Sql("INSERT INTO SystemConstantType (SystemConstantTypeId, SystemConstantTypeName) Values (2, 'ComplexText')");
            Sql("INSERT INTO SystemConstantType (SystemConstantTypeId, SystemConstantTypeName) Values (3, 'Boolean')");

            AlterColumn("dbo.SystemConstantType", "SystemConstantTypeId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.SystemConstantType", "SystemConstantTypeId");

            AddForeignKey("dbo.SystemConstant", "SystemConstantTypeId", "dbo.SystemConstantType", "SystemConstantTypeId");
        }

        public override void Down()
        {
            AddColumn("dbo.SystemConstant", "IsComplex", c => c.Boolean(nullable: false));

            Sql(@"Update constant
                  SET constant.IsComplex = CASE WHEN constant.SystemConstantTypeId = 2 THEN 1 ELSE 0 END
                  FROM SystemConstant constant");

            DropForeignKey("dbo.SystemConstant", "SystemConstantTypeId", "dbo.SystemConstantType");
            DropIndex("dbo.SystemConstant", new[] { "SystemConstantTypeId" });
            DropColumn("dbo.SystemConstant", "SystemConstantTypeId");
            DropTable("dbo.SystemConstantType");
        }
    }
}
