// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class AddWorkflowStatusAndMappingMethod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MappingMethodType",
                c => new
                {
                    MappingMethodTypeId = c.Int(false, true),
                    MappingMethodTypeName = c.String(false, 30)
                })
                .PrimaryKey(t => t.MappingMethodTypeId);

            CreateTable(
                "dbo.WorkflowStatusType",
                c => new
                {
                    WorkflowStatusTypeId = c.Int(false, true),
                    WorkflowStatusTypeName = c.String(false, 20)
                })
                .PrimaryKey(t => t.WorkflowStatusTypeId);

            AddColumn("dbo.SystemItemMap", "WorkflowStatusTypeId", c => c.Int());
            AddColumn("dbo.SystemItemMap", "MappingMethodTypeId", c => c.Int());
            CreateIndex("dbo.SystemItemMap", "WorkflowStatusTypeId");
            CreateIndex("dbo.SystemItemMap", "MappingMethodTypeId");
            AddForeignKey("dbo.SystemItemMap", "MappingMethodTypeId", "dbo.MappingMethodType", "MappingMethodTypeId");
            AddForeignKey("dbo.SystemItemMap", "WorkflowStatusTypeId", "dbo.WorkflowStatusType", "WorkflowStatusTypeId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.SystemItemMap", "WorkflowStatusTypeId", "dbo.WorkflowStatusType");
            DropForeignKey("dbo.SystemItemMap", "MappingMethodTypeId", "dbo.MappingMethodType");
            DropIndex("dbo.SystemItemMap", new[] {"MappingMethodTypeId"});
            DropIndex("dbo.SystemItemMap", new[] {"WorkflowStatusTypeId"});
            DropColumn("dbo.SystemItemMap", "MappingMethodTypeId");
            DropColumn("dbo.SystemItemMap", "WorkflowStatusTypeId");
            DropTable("dbo.WorkflowStatusType");
            DropTable("dbo.MappingMethodType");
        }
    }
}