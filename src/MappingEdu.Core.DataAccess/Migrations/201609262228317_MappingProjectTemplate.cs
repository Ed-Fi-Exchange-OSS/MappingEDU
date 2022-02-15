// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MappingProjectTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MappingProjectTemplate",
                c => new
                    {
                        MappingProjectTemplateId = c.Guid(nullable: false, identity: true),
                        MappingProjectId = c.Guid(nullable: false),
                        Template = c.String(),
                        Title = c.String(),
                        CreateBy = c.String(),
                        CreateById = c.Guid(),
                        UpdateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdateById = c.Guid(),
                    })
                .PrimaryKey(t => t.MappingProjectTemplateId)
                .ForeignKey("dbo.MappingProject", t => t.MappingProjectId, cascadeDelete: true)
                .Index(t => t.MappingProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MappingProjectTemplate", "MappingProjectId", "dbo.MappingProject");
            DropIndex("dbo.MappingProjectTemplate", new[] { "MappingProjectId" });
            DropTable("dbo.MappingProjectTemplate");
        }
    }
}
