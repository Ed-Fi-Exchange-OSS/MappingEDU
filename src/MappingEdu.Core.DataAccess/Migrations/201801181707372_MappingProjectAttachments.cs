// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MappingProjectAttachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MappingProjectAttachment",
                c => new
                    {
                        MappingProjectAttachmentId = c.Guid(nullable: false, identity: true),
                        MappingProjectId = c.Guid(nullable: false),
                        AttachmentName = c.String(maxLength: 255),
                        Description = c.String(),
                        FileBytes = c.Binary(),
                        FileName = c.String(maxLength: 255),
                        MimeType = c.String(maxLength: 127),
                        CreateBy = c.String(),
                        CreateById = c.Guid(),
                        UpdateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdateById = c.Guid(),
                    })
                .PrimaryKey(t => t.MappingProjectAttachmentId)
                .ForeignKey("dbo.MappingProject", t => t.MappingProjectId, cascadeDelete: true)
                .Index(t => t.MappingProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MappingProjectAttachment", "MappingProjectId", "dbo.MappingProject");
            DropIndex("dbo.MappingProjectAttachment", new[] { "MappingProjectId" });
            DropTable("dbo.MappingProjectAttachment");
        }
    }
}
