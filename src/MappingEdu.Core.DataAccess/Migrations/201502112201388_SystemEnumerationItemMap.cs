// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class SystemEnumerationItemMap : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SystemEnumerationItemMap", "MappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.SystemEnumerationItemMap", "SourceSystemEnumerationItemId", "dbo.SystemEnumerationItem");
            DropIndex("dbo.SystemEnumerationItemMap", new[] {"MappedSystemId"});
            AddColumn("dbo.SystemEnumerationItemMap", "SystemItemMapId", c => c.Guid(true));
            CreateIndex("dbo.SystemEnumerationItemMap", "SystemItemMapId");
            AddForeignKey("dbo.SystemEnumerationItemMap", "SystemItemMapId", "dbo.SystemItemMap", "SystemItemMapId", true);
            AddForeignKey("dbo.SystemEnumerationItemMap", "SourceSystemEnumerationItemId", "dbo.SystemEnumerationItem", "SystemEnumerationItemId", true);
            DropColumn("dbo.SystemEnumerationItemMap", "MappedSystemId");
        }

        public override void Down()
        {
            AddColumn("dbo.SystemEnumerationItemMap", "MappedSystemId", c => c.Guid(false));
            DropForeignKey("dbo.SystemEnumerationItemMap", "SourceSystemEnumerationItemId", "dbo.SystemEnumerationItem");
            DropForeignKey("dbo.SystemEnumerationItemMap", "SystemItemMapId", "dbo.SystemItemMap");
            DropIndex("dbo.SystemEnumerationItemMap", new[] {"SystemItemMapId"});
            DropColumn("dbo.SystemEnumerationItemMap", "SystemItemMapId");
            CreateIndex("dbo.SystemEnumerationItemMap", "MappedSystemId");
            AddForeignKey("dbo.SystemEnumerationItemMap", "SourceSystemEnumerationItemId", "dbo.SystemEnumerationItem", "SystemEnumerationItemId");
            AddForeignKey("dbo.SystemEnumerationItemMap", "MappedSystemId", "dbo.MappedSystem", "MappedSystemId");
        }
    }
}