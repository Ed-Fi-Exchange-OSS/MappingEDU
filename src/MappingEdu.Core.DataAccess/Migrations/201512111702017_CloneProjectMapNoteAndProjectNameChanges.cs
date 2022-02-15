// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class CloneProjectMapNoteAndProjectNameChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MapNote", "Title", c => c.String(false, 100));
            //CreateIndex("dbo.MappingProject", "IX_ProjectName", unique: true);
        }

        public override void Down()
        {
            //DropIndex("dbo.MappingProject", new[] { "IX_ProjectName" });
            AlterColumn("dbo.MapNote", "Title", c => c.String(maxLength: 100));
        }
    }
}