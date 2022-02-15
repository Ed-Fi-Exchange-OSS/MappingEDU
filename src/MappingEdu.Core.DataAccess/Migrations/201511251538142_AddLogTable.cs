// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class AddLogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Log",
                c => new
                {
                    LogId = c.Int(false, true),
                    Date = c.DateTime(false),
                    User = c.String(false, 50),
                    Thread = c.String(false, 255),
                    Level = c.String(false, 50),
                    Logger = c.String(false, 255),
                    Message = c.String(false, 4000),
                    Exception = c.String(maxLength: 2000)
                })
                .PrimaryKey(t => t.LogId);
        }

        public override void Down()
        {
            DropTable("dbo.Log");
        }
    }
}