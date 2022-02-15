// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueMappingProjectNameWhenActive : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.MappingProject", new[] { "ProjectName" });
            Sql("CREATE UNIQUE INDEX IDX_ProjectName on dbo.MappingProject(ProjectName, IsActive) WHERE IsActive = 1");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.MappingProject", "ProjectName", unique: true);
        }
    }
}
