// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class Base : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuildVersion",
                c => new
                {
                    BuildVersionId = c.String(false, 128),
                    BuildDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.BuildVersionId);

            CreateTable(
                "dbo.CompleteStatusType",
                c => new
                {
                    CompleteStatusTypeId = c.Int(false, true),
                    CompleteStatusTypeName = c.String(false, 20)
                })
                .PrimaryKey(t => t.CompleteStatusTypeId);

            CreateTable(
                "dbo.SystemItemMap",
                c => new
                {
                    SystemItemMapId = c.Guid(false, true),
                    SourceSystemItemId = c.Guid(false),
                    MappedSystemId = c.Guid(false),
                    BusinessLogic = c.String(),
                    DeferredMapping = c.Boolean(false),
                    OmissionReason = c.String(),
                    MappingStatusTypeId = c.Int(),
                    CompleteStatusTypeId = c.Int(),
                    MappingStatusReasonTypeId = c.Int(),
                    ExcludeInVendorSpecification = c.Boolean(false),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.SystemItemMapId)
                .ForeignKey("dbo.CompleteStatusType", t => t.CompleteStatusTypeId)
                .ForeignKey("dbo.MappedSystem", t => t.MappedSystemId)
                .ForeignKey("dbo.MappingStatusReasonType", t => t.MappingStatusReasonTypeId)
                .ForeignKey("dbo.MappingStatusType", t => t.MappingStatusTypeId)
                .ForeignKey("dbo.SystemItem", t => t.SourceSystemItemId)
                .Index(t => t.SourceSystemItemId)
                .Index(t => t.MappedSystemId)
                .Index(t => t.MappingStatusTypeId)
                .Index(t => t.CompleteStatusTypeId)
                .Index(t => t.MappingStatusReasonTypeId);

            CreateTable(
                "dbo.MapNote",
                c => new
                {
                    MapNoteId = c.Guid(false, true),
                    SystemItemMapId = c.Guid(false),
                    Title = c.String(false, 100),
                    Notes = c.String(false),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.MapNoteId)
                .ForeignKey("dbo.SystemItemMap", t => t.SystemItemMapId, true)
                .Index(t => t.SystemItemMapId);

            CreateTable(
                "dbo.MappedSystem",
                c => new
                {
                    MappedSystemId = c.Guid(false, true),
                    SystemName = c.String(false, 50),
                    SystemVersion = c.String(false, 6),
                    PreviousMappedSystemId = c.Guid(),
                    IsActive = c.Boolean(false),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.MappedSystemId)
                .ForeignKey("dbo.MappedSystem", t => t.PreviousMappedSystemId)
                .Index(t => t.PreviousMappedSystemId);

            CreateTable(
                "dbo.CustomDetailMetadata",
                c => new
                {
                    CustomDetailMetadataId = c.Guid(false, true),
                    MappedSystemId = c.Guid(false),
                    DisplayName = c.String(false, 100),
                    IsBoolean = c.Boolean(false),
                    IsCoreDetail = c.Boolean(false),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.CustomDetailMetadataId)
                .ForeignKey("dbo.MappedSystem", t => t.MappedSystemId, true)
                .Index(t => t.MappedSystemId);

            CreateTable(
                "dbo.SystemItemCustomDetailMapping",
                c => new
                {
                    SystemItemCustomDetailMappingId = c.Guid(false, true),
                    SystemItemId = c.Guid(false),
                    CustomDetailsMetadataId = c.Guid(false),
                    Value = c.String(),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.SystemItemCustomDetailMappingId)
                .ForeignKey("dbo.CustomDetailMetadata", t => t.CustomDetailsMetadataId, true)
                .ForeignKey("dbo.SystemItem", t => t.SystemItemId)
                .Index(t => t.SystemItemId)
                .Index(t => t.CustomDetailsMetadataId);

            CreateTable(
                "dbo.SystemItem",
                c => new
                {
                    SystemItemId = c.Guid(false, true),
                    MappedSystemId = c.Guid(false),
                    ParentSystemItemId = c.Guid(),
                    ItemName = c.String(false, 255),
                    Definition = c.String(),
                    ItemTypeId = c.Int(false),
                    ItemDataTypeId = c.Int(),
                    DataTypeSource = c.String(maxLength: 255),
                    FieldLength = c.Int(),
                    ItemUrl = c.String(maxLength: 255),
                    TechnicalName = c.String(maxLength: 255),
                    IsActive = c.Boolean(false),
                    EnumerationSystemItemId = c.Guid(),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.SystemItemId)
                .ForeignKey("dbo.SystemItem", t => t.EnumerationSystemItemId)
                .ForeignKey("dbo.ItemDataType", t => t.ItemDataTypeId)
                .ForeignKey("dbo.ItemType", t => t.ItemTypeId)
                .ForeignKey("dbo.MappedSystem", t => t.MappedSystemId, true)
                .ForeignKey("dbo.SystemItem", t => t.ParentSystemItemId)
                .Index(t => t.MappedSystemId)
                .Index(t => t.ParentSystemItemId)
                .Index(t => t.ItemTypeId)
                .Index(t => t.ItemDataTypeId)
                .Index(t => t.EnumerationSystemItemId);

            CreateTable(
                "dbo.ItemDataType",
                c => new
                {
                    ItemDataTypeId = c.Int(false, true),
                    ItemDataTypeName = c.String(false, 50)
                })
                .PrimaryKey(t => t.ItemDataTypeId);

            CreateTable(
                "dbo.ItemType",
                c => new
                {
                    ItemTypeId = c.Int(false, true),
                    ItemTypeName = c.String(false, 50)
                })
                .PrimaryKey(t => t.ItemTypeId);

            CreateTable(
                "dbo.SystemItemVersionDelta",
                c => new
                {
                    SystemItemVersionDeltaId = c.Guid(false, true),
                    OldSystemItemId = c.Guid(),
                    NewSystemItemId = c.Guid(),
                    ItemChangeTypeId = c.Int(false),
                    Description = c.String(maxLength: 255),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.SystemItemVersionDeltaId)
                .ForeignKey("dbo.ItemChangeType", t => t.ItemChangeTypeId)
                .ForeignKey("dbo.SystemItem", t => t.NewSystemItemId, true)
                .ForeignKey("dbo.SystemItem", t => t.OldSystemItemId)
                .Index(t => t.OldSystemItemId)
                .Index(t => t.NewSystemItemId)
                .Index(t => t.ItemChangeTypeId);

            CreateTable(
                "dbo.ItemChangeType",
                c => new
                {
                    ItemChangeTypeId = c.Int(false, true),
                    ItemChangeTypeName = c.String(false, 30)
                })
                .PrimaryKey(t => t.ItemChangeTypeId);

            CreateTable(
                "dbo.Note",
                c => new
                {
                    NoteId = c.Guid(false, true),
                    SystemItemId = c.Guid(false),
                    Title = c.String(false, 100),
                    Notes = c.String(false),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.NoteId)
                .ForeignKey("dbo.SystemItem", t => t.SystemItemId, true)
                .Index(t => t.SystemItemId);

            CreateTable(
                "dbo.SystemEnumerationItem",
                c => new
                {
                    SystemEnumerationItemId = c.Guid(false, true),
                    SystemItemId = c.Guid(false),
                    CodeValue = c.String(false, 1024),
                    Description = c.String(maxLength: 1024),
                    ShortDescription = c.String(maxLength: 1024),
                    Category = c.String(maxLength: 1024),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.SystemEnumerationItemId)
                .ForeignKey("dbo.SystemItem", t => t.SystemItemId)
                .Index(t => t.SystemItemId);

            CreateTable(
                "dbo.SystemEnumerationItemMap",
                c => new
                {
                    SystemEnumerationItemMapId = c.Guid(false, true),
                    SourceSystemEnumerationItemId = c.Guid(false),
                    MappedSystemId = c.Guid(false),
                    TargetSystemEnumerationItemId = c.Guid(),
                    DeferredMapping = c.Boolean(false),
                    EnumerationMappingStatusTypeId = c.Int(),
                    EnumerationMappingStatusReasonTypeId = c.Int(),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.SystemEnumerationItemMapId)
                .ForeignKey("dbo.EnumerationMappingStatusReasonType", t => t.EnumerationMappingStatusReasonTypeId)
                .ForeignKey("dbo.EnumerationMappingStatusType", t => t.EnumerationMappingStatusTypeId)
                .ForeignKey("dbo.MappedSystem", t => t.MappedSystemId)
                .ForeignKey("dbo.SystemEnumerationItem", t => t.SourceSystemEnumerationItemId)
                .ForeignKey("dbo.SystemEnumerationItem", t => t.TargetSystemEnumerationItemId)
                .Index(t => t.SourceSystemEnumerationItemId)
                .Index(t => t.MappedSystemId)
                .Index(t => t.TargetSystemEnumerationItemId)
                .Index(t => t.EnumerationMappingStatusTypeId)
                .Index(t => t.EnumerationMappingStatusReasonTypeId);

            CreateTable(
                "dbo.EnumerationMappingStatusReasonType",
                c => new
                {
                    EnumerationMappingStatusReasonTypeId = c.Int(false, true),
                    EnumerationMappingStatusReasonTypeName = c.String(false, 50)
                })
                .PrimaryKey(t => t.EnumerationMappingStatusReasonTypeId);

            CreateTable(
                "dbo.EnumerationMappingStatusType",
                c => new
                {
                    EnumerationMappingStatusTypeId = c.Int(false, true),
                    EnumerationMappingStatusTypeName = c.String()
                })
                .PrimaryKey(t => t.EnumerationMappingStatusTypeId);

            CreateTable(
                "dbo.MappingStatusReasonType",
                c => new
                {
                    MappingStatusReasonTypeId = c.Int(false, true),
                    MappingStatusReasonTypeName = c.String(false, 50)
                })
                .PrimaryKey(t => t.MappingStatusReasonTypeId);

            CreateTable(
                "dbo.MappingStatusType",
                c => new
                {
                    MappingStatusTypeId = c.Int(false, true),
                    MappingStatusTypeName = c.String(false, 50)
                })
                .PrimaryKey(t => t.MappingStatusTypeId);

            CreateTable(
                "dbo.CustomMigration",
                c => new
                {
                    CustomMigrationId = c.Int(false, true),
                    Name = c.String(maxLength: 255),
                    CreateDate = c.DateTime(false),
                    UpdateDate = c.DateTime(false)
                })
                .PrimaryKey(t => t.CustomMigrationId);

            CreateTable(
                "dbo.SystemItemMapAssociation",
                c => new
                {
                    SystemItemMapId = c.Guid(false),
                    SystemItemId = c.Guid(false)
                })
                .PrimaryKey(t => new {t.SystemItemMapId, t.SystemItemId})
                .ForeignKey("dbo.SystemItemMap", t => t.SystemItemMapId, true)
                .ForeignKey("dbo.SystemItem", t => t.SystemItemId, true)
                .Index(t => t.SystemItemMapId)
                .Index(t => t.SystemItemId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.SystemItemMapAssociation", "SystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemItemMapAssociation", "SystemItemMapId", "dbo.SystemItemMap");
            DropForeignKey("dbo.SystemItemMap", "SourceSystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemItemMap", "MappingStatusTypeId", "dbo.MappingStatusType");
            DropForeignKey("dbo.SystemItemMap", "MappingStatusReasonTypeId", "dbo.MappingStatusReasonType");
            DropForeignKey("dbo.SystemItemMap", "MappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.MappedSystem", "PreviousMappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.SystemItemCustomDetailMapping", "SystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemEnumerationItem", "SystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemEnumerationItemMap", "TargetSystemEnumerationItemId", "dbo.SystemEnumerationItem");
            DropForeignKey("dbo.SystemEnumerationItemMap", "SourceSystemEnumerationItemId", "dbo.SystemEnumerationItem");
            DropForeignKey("dbo.SystemEnumerationItemMap", "MappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.SystemEnumerationItemMap", "EnumerationMappingStatusTypeId", "dbo.EnumerationMappingStatusType");
            DropForeignKey("dbo.SystemEnumerationItemMap", "EnumerationMappingStatusReasonTypeId", "dbo.EnumerationMappingStatusReasonType");
            DropForeignKey("dbo.SystemItem", "ParentSystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.Note", "SystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemItemVersionDelta", "OldSystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemItemVersionDelta", "NewSystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemItemVersionDelta", "ItemChangeTypeId", "dbo.ItemChangeType");
            DropForeignKey("dbo.SystemItem", "MappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.SystemItem", "ItemTypeId", "dbo.ItemType");
            DropForeignKey("dbo.SystemItem", "ItemDataTypeId", "dbo.ItemDataType");
            DropForeignKey("dbo.SystemItem", "EnumerationSystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.SystemItemCustomDetailMapping", "CustomDetailsMetadataId", "dbo.CustomDetailMetadata");
            DropForeignKey("dbo.CustomDetailMetadata", "MappedSystemId", "dbo.MappedSystem");
            DropForeignKey("dbo.MapNote", "SystemItemMapId", "dbo.SystemItemMap");
            DropForeignKey("dbo.SystemItemMap", "CompleteStatusTypeId", "dbo.CompleteStatusType");
            DropIndex("dbo.SystemItemMapAssociation", new[] {"SystemItemId"});
            DropIndex("dbo.SystemItemMapAssociation", new[] {"SystemItemMapId"});
            DropIndex("dbo.SystemEnumerationItemMap", new[] {"EnumerationMappingStatusReasonTypeId"});
            DropIndex("dbo.SystemEnumerationItemMap", new[] {"EnumerationMappingStatusTypeId"});
            DropIndex("dbo.SystemEnumerationItemMap", new[] {"TargetSystemEnumerationItemId"});
            DropIndex("dbo.SystemEnumerationItemMap", new[] {"MappedSystemId"});
            DropIndex("dbo.SystemEnumerationItemMap", new[] {"SourceSystemEnumerationItemId"});
            DropIndex("dbo.SystemEnumerationItem", new[] {"SystemItemId"});
            DropIndex("dbo.Note", new[] {"SystemItemId"});
            DropIndex("dbo.SystemItemVersionDelta", new[] {"ItemChangeTypeId"});
            DropIndex("dbo.SystemItemVersionDelta", new[] {"NewSystemItemId"});
            DropIndex("dbo.SystemItemVersionDelta", new[] {"OldSystemItemId"});
            DropIndex("dbo.SystemItem", new[] {"EnumerationSystemItemId"});
            DropIndex("dbo.SystemItem", new[] {"ItemDataTypeId"});
            DropIndex("dbo.SystemItem", new[] {"ItemTypeId"});
            DropIndex("dbo.SystemItem", new[] {"ParentSystemItemId"});
            DropIndex("dbo.SystemItem", new[] {"MappedSystemId"});
            DropIndex("dbo.SystemItemCustomDetailMapping", new[] {"CustomDetailsMetadataId"});
            DropIndex("dbo.SystemItemCustomDetailMapping", new[] {"SystemItemId"});
            DropIndex("dbo.CustomDetailMetadata", new[] {"MappedSystemId"});
            DropIndex("dbo.MappedSystem", new[] {"PreviousMappedSystemId"});
            DropIndex("dbo.MapNote", new[] {"SystemItemMapId"});
            DropIndex("dbo.SystemItemMap", new[] {"MappingStatusReasonTypeId"});
            DropIndex("dbo.SystemItemMap", new[] {"CompleteStatusTypeId"});
            DropIndex("dbo.SystemItemMap", new[] {"MappingStatusTypeId"});
            DropIndex("dbo.SystemItemMap", new[] {"MappedSystemId"});
            DropIndex("dbo.SystemItemMap", new[] {"SourceSystemItemId"});
            DropTable("dbo.SystemItemMapAssociation");
            DropTable("dbo.CustomMigration");
            DropTable("dbo.MappingStatusType");
            DropTable("dbo.MappingStatusReasonType");
            DropTable("dbo.EnumerationMappingStatusType");
            DropTable("dbo.EnumerationMappingStatusReasonType");
            DropTable("dbo.SystemEnumerationItemMap");
            DropTable("dbo.SystemEnumerationItem");
            DropTable("dbo.Note");
            DropTable("dbo.ItemChangeType");
            DropTable("dbo.SystemItemVersionDelta");
            DropTable("dbo.ItemType");
            DropTable("dbo.ItemDataType");
            DropTable("dbo.SystemItem");
            DropTable("dbo.SystemItemCustomDetailMapping");
            DropTable("dbo.CustomDetailMetadata");
            DropTable("dbo.MappedSystem");
            DropTable("dbo.MapNote");
            DropTable("dbo.SystemItemMap");
            DropTable("dbo.CompleteStatusType");
            DropTable("dbo.BuildVersion");
        }
    }
}