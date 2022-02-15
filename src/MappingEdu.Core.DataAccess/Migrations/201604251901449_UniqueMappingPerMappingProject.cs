// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueMappingPerMappingProject : DbMigration
    {
        public override void Up()
        {
            Sql(@"  DELETE SystemItemMap 
                    WHERE SystemItemMapId not in (
	                      SELECT TOP 1 sim.SystemItemMapId
	                      FROM SystemItemMap as sim
	                      INNER JOIN (
	                      SELECT MAX(UpdateDate) as UpdateDate,
	                             SourceSystemItemId,
			                     MappingProjectId
	                      FROM SystemItemMap
	                      GROUP BY SourceSystemItemId, MappingProjectId) as s
	                      ON s.MappingProjectId = sim.MappingProjectId 
	                         AND s.SourceSystemItemId = sim.SourceSystemItemId 
		                     AND s.UpdateDate = sim.UpdateDate);
            ");
            DropIndex("dbo.SystemItemMap", new[] { "MappingProjectId" });
            DropIndex("dbo.SystemItemMap", new[] { "SourceSystemItemId" });
            CreateIndex("dbo.SystemItemMap", new[] { "SourceSystemItemId", "MappingProjectId" }, unique: true, name: "IX_UniqueMapping");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SystemItemMap", "IX_UniqueMapping");
            CreateIndex("dbo.SystemItemMap", "SourceSystemItemId");
            CreateIndex("dbo.SystemItemMap", "MappingProjectId");
        }
    }
}
