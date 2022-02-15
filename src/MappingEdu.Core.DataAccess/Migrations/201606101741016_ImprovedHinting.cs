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
    
    public partial class ImprovedHinting : DbMigration
    {
        private readonly string _createGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Create.sql");
        private readonly string _dropGetMatchmakerSearchPage = DataAccessHelper.GetStoredProcedurePath("GetMatchmakerSearchPage", "Drop.sql");

        public override void Up()
        {
            CreateTable(
                "dbo.EntityHint",
                c => new
                    {
                        EntityHintId = c.Guid(nullable: false, identity: true),
                        MappingProjectId = c.Guid(nullable: false),
                        SourceEntityId = c.Guid(nullable: false),
                        TargetEntityId = c.Guid(nullable: false),
                        CreateBy = c.String(),
                        UpdateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EntityHintId)
                .ForeignKey("dbo.MappingProject", t => t.MappingProjectId)
                .ForeignKey("dbo.SystemItem", t => t.SourceEntityId)
                .ForeignKey("dbo.SystemItem", t => t.TargetEntityId)
                .Index(t => new { t.SourceEntityId, t.MappingProjectId }, unique: true, name: "IX_UniqueEntityHint")
                .Index(t => t.TargetEntityId);
            
            CreateTable(
                "dbo.MappingProjectSynonym",
                c => new
                    {
                        MappingProjectSynonymId = c.Guid(nullable: false, identity: true),
                        MappingProjectId = c.Guid(nullable: false),
                        SourceWord = c.String(),
                        TargetWord = c.String(),
                        CreateBy = c.String(),
                        UpdateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MappingProjectSynonymId)
                .ForeignKey("dbo.MappingProject", t => t.MappingProjectId, cascadeDelete: true)
                .Index(t => t.MappingProjectId);
            
            Sql(File.ReadAllText(_createGetMatchmakerSearchPage));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MappingProjectSynonym", "MappingProjectId", "dbo.MappingProject");
            DropForeignKey("dbo.EntityHint", "TargetEntityId", "dbo.SystemItem");
            DropForeignKey("dbo.EntityHint", "SourceEntityId", "dbo.SystemItem");
            DropForeignKey("dbo.EntityHint", "MappingProjectId", "dbo.MappingProject");
            DropIndex("dbo.MappingProjectSynonym", new[] { "MappingProjectId" });
            DropIndex("dbo.EntityHint", new[] { "TargetEntityId" });
            DropIndex("dbo.EntityHint", "IX_UniqueEntityHint");
            DropTable("dbo.MappingProjectSynonym");
            DropTable("dbo.EntityHint");

            Sql(File.ReadAllText(_dropGetMatchmakerSearchPage));
        }
        
    }
}
