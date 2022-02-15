// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    // ReSharper disable once InconsistentNaming
    public partial class IX_SystemItemMap_MappingProjectId : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE NONCLUSTERED INDEX IX_SystemItemMap_MappingProjectId
            ON[dbo].[SystemItemMap]([MappingProjectId])
            INCLUDE([UpdateById], [CreateById])");
        }
        
        public override void Down()
        {
            Sql(@"DROP INDEX [dbo].[SystemItemMap].IX_SystemItemMap_MappingProjectId");
        }
    }
}
