// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutoMappingReasonTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AutoMappingReasonType",
                c => new
                    {
                        AutoMappingReasonTypeId = c.Int(nullable: false, identity: true),
                        AutoMappingReasonTypeName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.AutoMappingReasonTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AutoMappingReasonType");
        }
    }
}
