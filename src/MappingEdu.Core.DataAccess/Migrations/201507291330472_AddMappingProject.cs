// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class AddMappingProject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MappingProject",
                c => new
                {
                    MappingProjectId = c.Guid(false, true),
                    ProjectName = c.String(false, 50),
                    Description = c.String(false, 1000),
                    SourceDataStandardMappedSystemId = c.Guid(false),
                    TargetDataStandardMappedSystemId = c.Guid(false),
                    ProjectStatusTypeId = c.Int(false),
                    IsActive = c.Boolean(false),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.MappingProjectId)
                .ForeignKey("dbo.MappedSystem", t => t.SourceDataStandardMappedSystemId)
                .ForeignKey("dbo.MappedSystem", t => t.TargetDataStandardMappedSystemId)
                .ForeignKey("dbo.ProjectStatusType", t => t.ProjectStatusTypeId, true)
                .Index(t => t.SourceDataStandardMappedSystemId)
                .Index(t => t.TargetDataStandardMappedSystemId)
                .Index(t => t.ProjectStatusTypeId);

            CreateTable(
                "dbo.ProjectStatusType",
                c => new
                {
                    ProjectStatusTypeId = c.Int(false, true),
                    ProjectStatusTypeName = c.String(false, 20)
                })
                .PrimaryKey(t => t.ProjectStatusTypeId);

            DropForeignKey("dbo.SystemItemMap", "MappedSystemId", "dbo.MappedSystem");
            DropIndex("dbo.SystemItemMap", new[] {"MappedSystemId"});
            AddColumn("dbo.SystemItemMap", "MappingProjectId", c => c.Guid(true));
            CreateIndex("dbo.SystemItemMap", "MappingProjectId");
            AddForeignKey("dbo.SystemItemMap", "MappingProjectId", "dbo.MappingProject", "MappingProjectId");
            DropColumn("dbo.SystemItemMap", "MappedSystemId");
        }

        public override void Down()
        {
            AddColumn("dbo.SystemItemMap", "MappedSystemId", c => c.Guid(false));
            DropForeignKey("dbo.MappingProject", "ProjectStatusTypeId", "dbo.ProjectStatusType");
            DropForeignKey("dbo.SystemItemMap", "MappingProjectId", "dbo.MappingProject");
            DropForeignKey("dbo.MappingProject", "TargetDataStandardMappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.MappingProject", "SourceDataStandardMappedSystemId", "dbo.MappedSystem");
            DropIndex("dbo.MappingProject", new[] {"ProjectStatusTypeId"});
            DropIndex("dbo.MappingProject", new[] {"TargetDataStandardMappedSystemId"});
            DropIndex("dbo.MappingProject", new[] {"SourceDataStandardMappedSystemId"});
            DropIndex("dbo.SystemItemMap", new[] {"MappingProjectId"});
            DropColumn("dbo.SystemItemMap", "MappingProjectId");
            DropTable("dbo.ProjectStatusType");
            DropTable("dbo.MappingProject");
            CreateIndex("dbo.SystemItemMap", "MappedSystemId");
            AddForeignKey("dbo.SystemItemMap", "MappedSystemId", "dbo.MappedSystem", "MappedSystemId");
        }
    }
}