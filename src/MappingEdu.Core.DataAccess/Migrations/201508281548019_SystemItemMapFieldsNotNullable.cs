// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class SystemItemMapFieldsNotNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SystemItemMap", new[] {"WorkflowStatusTypeId"});
            DropIndex("dbo.SystemItemMap", new[] {"MappingMethodTypeId"});
            AlterColumn("dbo.SystemItemMap", "WorkflowStatusTypeId", c => c.Int(false));
            AlterColumn("dbo.SystemItemMap", "MappingMethodTypeId", c => c.Int(false));
            CreateIndex("dbo.SystemItemMap", "WorkflowStatusTypeId");
            CreateIndex("dbo.SystemItemMap", "MappingMethodTypeId");
        }

        public override void Down()
        {
            DropIndex("dbo.SystemItemMap", new[] {"MappingMethodTypeId"});
            DropIndex("dbo.SystemItemMap", new[] {"WorkflowStatusTypeId"});
            AlterColumn("dbo.SystemItemMap", "MappingMethodTypeId", c => c.Int());
            AlterColumn("dbo.SystemItemMap", "WorkflowStatusTypeId", c => c.Int());
            CreateIndex("dbo.SystemItemMap", "MappingMethodTypeId");
            CreateIndex("dbo.SystemItemMap", "WorkflowStatusTypeId");
        }
    }
}