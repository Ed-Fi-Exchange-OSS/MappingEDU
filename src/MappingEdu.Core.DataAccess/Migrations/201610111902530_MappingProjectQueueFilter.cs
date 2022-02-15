// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using System.Linq;

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MappingProjectQueueFilter : DbMigration
    {
        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter10.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter9.sql");

        public override void Up()
        {
            CreateTable(
                "dbo.MappingProjectQueueFilter",
                c => new
                    {
                        MappingProjectQueueFilterId = c.Guid(nullable: false, identity: true),
                        AutoMapped = c.Boolean(nullable: false),
                        Base = c.Boolean(nullable: false),
                        CreatedByColumn = c.Boolean(nullable: false),
                        CreationDateColumn = c.Boolean(nullable: false),
                        Extended = c.Boolean(nullable: false),
                        Flagged = c.Boolean(nullable: false),
                        Length = c.Int(nullable: false),
                        MappedByColumn = c.Boolean(nullable: false),
                        MappingProjectId = c.Guid(nullable: false),
                        Name = c.String(),
                        OrderColumn = c.Int(nullable: false),
                        OrderDirection = c.String(),
                        Search = c.String(),
                        ShowInDashboard = c.Boolean(nullable: false),
                        Unflagged = c.Boolean(nullable: false),
                        UpdatedByColumn = c.Boolean(nullable: false),
                        UpdateDateColumn = c.Boolean(nullable: false),
                        UserMapped = c.Boolean(nullable: false),
                        UserId = c.Guid(nullable: false),
                        CreateBy = c.String(),
                        CreateById = c.Guid(),
                        UpdateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdateById = c.Guid(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.MappingProjectQueueFilterId)
                .ForeignKey("dbo.MappingProject", t => t.MappingProjectId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.MappingProjectId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.MappingProjectQueueFilterCreatedByUser",
                c => new
                    {
                        MappingProjectQueueFilterId = c.Guid(nullable: false),
                        CreatedByUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.MappingProjectQueueFilterId, t.CreatedByUserId })
                .ForeignKey("dbo.MappingProjectQueueFilter", t => t.MappingProjectQueueFilterId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId, cascadeDelete: true)
                .Index(t => t.MappingProjectQueueFilterId)
                .Index(t => t.CreatedByUserId);
            
            CreateTable(
                "dbo.MappingProjectQueueFilterItemType",
                c => new
                    {
                        MappingProjectQueueFilterId = c.Guid(nullable: false),
                        ItemTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MappingProjectQueueFilterId, t.ItemTypeId })
                .ForeignKey("dbo.MappingProjectQueueFilter", t => t.MappingProjectQueueFilterId, cascadeDelete: true)
                .ForeignKey("dbo.ItemType", t => t.ItemTypeId, cascadeDelete: true)
                .Index(t => t.MappingProjectQueueFilterId)
                .Index(t => t.ItemTypeId);
            
            CreateTable(
                "dbo.MappingProjectQueueFilterMappingMethodType",
                c => new
                    {
                        MappingProjectQueueFilterId = c.Guid(nullable: false),
                        MappingMethodTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MappingProjectQueueFilterId, t.MappingMethodTypeId })
                .ForeignKey("dbo.MappingProjectQueueFilter", t => t.MappingProjectQueueFilterId, cascadeDelete: true)
                .ForeignKey("dbo.MappingMethodType", t => t.MappingMethodTypeId, cascadeDelete: true)
                .Index(t => t.MappingProjectQueueFilterId)
                .Index(t => t.MappingMethodTypeId);
            
            CreateTable(
                "dbo.MappingProjectQueueFilterParentSystemItem",
                c => new
                    {
                        MappingProjectQueueFilterId = c.Guid(nullable: false),
                        SystemItemId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.MappingProjectQueueFilterId, t.SystemItemId })
                .ForeignKey("dbo.MappingProjectQueueFilter", t => t.MappingProjectQueueFilterId, cascadeDelete: true)
                .ForeignKey("dbo.SystemItem", t => t.SystemItemId, cascadeDelete: true)
                .Index(t => t.MappingProjectQueueFilterId)
                .Index(t => t.SystemItemId);
            
            CreateTable(
                "dbo.MappingProjectQueueFilterUpdatedByUser",
                c => new
                    {
                        MappingProjectQueueFilterId = c.Guid(nullable: false),
                        UpdatedByUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.MappingProjectQueueFilterId, t.UpdatedByUserId })
                .ForeignKey("dbo.MappingProjectQueueFilter", t => t.MappingProjectQueueFilterId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedByUserId, cascadeDelete: true)
                .Index(t => t.MappingProjectQueueFilterId)
                .Index(t => t.UpdatedByUserId);
            
            CreateTable(
                "dbo.MappingProjectQueueWorkflowStatusType",
                c => new
                    {
                        MappingProjectQueueFilterId = c.Guid(nullable: false),
                        WorkflowStatusTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MappingProjectQueueFilterId, t.WorkflowStatusTypeId })
                .ForeignKey("dbo.MappingProjectQueueFilter", t => t.MappingProjectQueueFilterId, cascadeDelete: true)
                .ForeignKey("dbo.WorkflowStatusType", t => t.WorkflowStatusTypeId, cascadeDelete: true)
                .Index(t => t.MappingProjectQueueFilterId)
                .Index(t => t.WorkflowStatusTypeId);

            Sql(File.ReadAllText(_alterGetReviewQueuePage));

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MappingProjectQueueWorkflowStatusType", "WorkflowStatusTypeId", "dbo.WorkflowStatusType");
            DropForeignKey("dbo.MappingProjectQueueWorkflowStatusType", "MappingProjectQueueFilterId", "dbo.MappingProjectQueueFilter");
            DropForeignKey("dbo.MappingProjectQueueFilter", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.MappingProjectQueueFilterUpdatedByUser", "UpdatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MappingProjectQueueFilterUpdatedByUser", "MappingProjectQueueFilterId", "dbo.MappingProjectQueueFilter");
            DropForeignKey("dbo.MappingProjectQueueFilterParentSystemItem", "SystemItemId", "dbo.SystemItem");
            DropForeignKey("dbo.MappingProjectQueueFilterParentSystemItem", "MappingProjectQueueFilterId", "dbo.MappingProjectQueueFilter");
            DropForeignKey("dbo.MappingProjectQueueFilter", "MappingProjectId", "dbo.MappingProject");
            DropForeignKey("dbo.MappingProjectQueueFilterMappingMethodType", "MappingMethodTypeId", "dbo.MappingMethodType");
            DropForeignKey("dbo.MappingProjectQueueFilterMappingMethodType", "MappingProjectQueueFilterId", "dbo.MappingProjectQueueFilter");
            DropForeignKey("dbo.MappingProjectQueueFilterItemType", "ItemTypeId", "dbo.ItemType");
            DropForeignKey("dbo.MappingProjectQueueFilterItemType", "MappingProjectQueueFilterId", "dbo.MappingProjectQueueFilter");
            DropForeignKey("dbo.MappingProjectQueueFilterCreatedByUser", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MappingProjectQueueFilterCreatedByUser", "MappingProjectQueueFilterId", "dbo.MappingProjectQueueFilter");
            DropIndex("dbo.MappingProjectQueueWorkflowStatusType", new[] { "WorkflowStatusTypeId" });
            DropIndex("dbo.MappingProjectQueueWorkflowStatusType", new[] { "MappingProjectQueueFilterId" });
            DropIndex("dbo.MappingProjectQueueFilterUpdatedByUser", new[] { "UpdatedByUserId" });
            DropIndex("dbo.MappingProjectQueueFilterUpdatedByUser", new[] { "MappingProjectQueueFilterId" });
            DropIndex("dbo.MappingProjectQueueFilterParentSystemItem", new[] { "SystemItemId" });
            DropIndex("dbo.MappingProjectQueueFilterParentSystemItem", new[] { "MappingProjectQueueFilterId" });
            DropIndex("dbo.MappingProjectQueueFilterMappingMethodType", new[] { "MappingMethodTypeId" });
            DropIndex("dbo.MappingProjectQueueFilterMappingMethodType", new[] { "MappingProjectQueueFilterId" });
            DropIndex("dbo.MappingProjectQueueFilterItemType", new[] { "ItemTypeId" });
            DropIndex("dbo.MappingProjectQueueFilterItemType", new[] { "MappingProjectQueueFilterId" });
            DropIndex("dbo.MappingProjectQueueFilterCreatedByUser", new[] { "CreatedByUserId" });
            DropIndex("dbo.MappingProjectQueueFilterCreatedByUser", new[] { "MappingProjectQueueFilterId" });
            DropIndex("dbo.MappingProjectQueueFilter", new[] { "User_Id" });
            DropIndex("dbo.MappingProjectQueueFilter", new[] { "MappingProjectId" });
            DropTable("dbo.MappingProjectQueueWorkflowStatusType");
            DropTable("dbo.MappingProjectQueueFilterUpdatedByUser");
            DropTable("dbo.MappingProjectQueueFilterParentSystemItem");
            DropTable("dbo.MappingProjectQueueFilterMappingMethodType");
            DropTable("dbo.MappingProjectQueueFilterItemType");
            DropTable("dbo.MappingProjectQueueFilterCreatedByUser");
            DropTable("dbo.MappingProjectQueueFilter");

            Sql(File.ReadAllText(_revertGetReviewQueuePage));
        }
    }
}
