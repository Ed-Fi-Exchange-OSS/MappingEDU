// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class ConvertScript : DbMigration
    {
        public override void Up()
        {
            // Unfortunately, we need this empty migration to correct a merge conflict issue
            // https://msdn.microsoft.com/en-us/data/dn481501.aspx
        }

        public override void Down()
        {
        }
    }
}