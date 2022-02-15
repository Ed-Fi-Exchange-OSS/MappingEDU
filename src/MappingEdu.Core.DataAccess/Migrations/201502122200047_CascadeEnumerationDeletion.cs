// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class CascadeEnumerationDeletion : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SystemEnumerationItem", "SystemItemId", "dbo.SystemItem");
            AddForeignKey("dbo.SystemEnumerationItem", "SystemItemId", "dbo.SystemItem", "SystemItemId", true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.SystemEnumerationItem", "SystemItemId", "dbo.SystemItem");
            AddForeignKey("dbo.SystemEnumerationItem", "SystemItemId", "dbo.SystemItem", "SystemItemId");
        }
    }
}