// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class FixSystemItemCustomDetail : DbMigration
    {
        public override void Up()
        {
            RenameTable("dbo.SystemItemCustomDetailMapping", "SystemItemCustomDetail");
            RenameColumn("dbo.SystemItemCustomDetail", "SystemItemCustomDetailMappingId", "SystemItemCustomDetailId");
            RenameColumn("dbo.SystemItemCustomDetail", "CustomDetailsMetadataId", "CustomDetailMetadataId");
            RenameIndex("dbo.SystemItemCustomDetail", "IX_CustomDetailsMetadataId", "IX_CustomDetailMetadataId");
        }

        public override void Down()
        {
            RenameIndex("dbo.SystemItemCustomDetail", "IX_CustomDetailMetadataId", "IX_CustomDetailsMetadataId");
            RenameColumn("dbo.SystemItemCustomDetail", "CustomDetailMetadataId", "CustomDetailsMetadataId");
            RenameColumn("dbo.SystemItemCustomDetail", "SystemItemCustomDetailId", "SystemItemCustomDetailMappingId");
            RenameTable("dbo.SystemItemCustomDetail", "SystemItemCustomDetailMapping");
        }
    }
}