// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClonedFrom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MappedSystem", "ClonedFromMappedSystemId", c => c.Guid());
            CreateIndex("dbo.MappedSystem", "ClonedFromMappedSystemId");
            AddForeignKey("dbo.MappedSystem", "ClonedFromMappedSystemId", "dbo.MappedSystem", "MappedSystemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MappedSystem", "ClonedFromMappedSystemId", "dbo.MappedSystem");
            DropIndex("dbo.MappedSystem", new[] { "ClonedFromMappedSystemId" });
            DropColumn("dbo.MappedSystem", "ClonedFromMappedSystemId");
        }
    }
}
