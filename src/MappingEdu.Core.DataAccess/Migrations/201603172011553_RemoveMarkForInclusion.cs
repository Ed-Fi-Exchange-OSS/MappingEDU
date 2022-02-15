// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMarkForInclusion : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.SystemItemMap SET MappingMethodTypeId=3 WHERE MappingMethodTypeId=2");
        }

        public override void Down()
        {
        }
    }
}
