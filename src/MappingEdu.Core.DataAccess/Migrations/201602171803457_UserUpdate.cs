// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MappedSystemUpdate",
                c => new
                    {
                        MappedSystemId = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MappedSystemId, t.UserId })
                .ForeignKey("dbo.MappedSystem", t => t.MappedSystemId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MappedSystemId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.MappingProjectUpdate",
                c => new
                    {
                        MappingProjectId = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.MappingProjectId, t.UserId })
                .ForeignKey("dbo.MappingProject", t => t.MappingProjectId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MappingProjectId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MappingProjectUpdate", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MappingProjectUpdate", "MappingProjectId", "dbo.MappingProject");
            DropForeignKey("dbo.MappedSystemUpdate", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MappedSystemUpdate", "MappedSystemId", "dbo.MappedSystem");
            DropIndex("dbo.MappingProjectUpdate", new[] { "UserId" });
            DropIndex("dbo.MappingProjectUpdate", new[] { "MappingProjectId" });
            DropIndex("dbo.MappedSystemUpdate", new[] { "UserId" });
            DropIndex("dbo.MappedSystemUpdate", new[] { "MappedSystemId" });
            DropTable("dbo.MappingProjectUpdate");
            DropTable("dbo.MappedSystemUpdate");
        }
    }
}
