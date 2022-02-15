// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemConstantGroupAndPrivate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemConstant", "Group", c => c.String());
            AddColumn("dbo.SystemConstant", "IsPrivate", c => c.Boolean(nullable: false));

            Sql("INSERT INTO SystemConstantType (SystemConstantTypeId, SystemConstantTypeName) Values (4, 'TextArea')");
        }

        public override void Down()
        {
            DropColumn("dbo.SystemConstant", "IsPrivate");
            DropColumn("dbo.SystemConstant", "Group");

            Sql("DELETE FROM SystemConstantType WHERE SystemConstantTypeId = 4");
        }
    }
}
