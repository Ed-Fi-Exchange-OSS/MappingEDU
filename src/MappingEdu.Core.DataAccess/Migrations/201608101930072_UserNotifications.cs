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
    
    public partial class UserNotifications : DbMigration
    {
        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter8.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter7.sql");

        public override void Up()
        {
            CreateTable(
                "dbo.UserNotification",
                c => new
                    {
                        UserNotificationId = c.Guid(nullable: false, identity: true),
                        HasSeen = c.Boolean(nullable: false),
                        IsDismissed = c.Boolean(nullable: false),
                        MappingProjectId = c.Guid(),
                        Notification = c.String(),
                        SystemItemMapId = c.Guid(),
                        MapNoteId = c.Guid(),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CreateBy = c.String(),
                        CreateById = c.Guid(),
                        UpdateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdateById = c.Guid(),
                    })
                .PrimaryKey(t => t.UserNotificationId)
                .ForeignKey("dbo.MapNote", t => t.MapNoteId)
                .ForeignKey("dbo.MappingProject", t => t.MappingProjectId)
                .ForeignKey("dbo.SystemItemMap", t => t.SystemItemMapId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.MappingProjectId)
                .Index(t => t.SystemItemMapId)
                .Index(t => t.MapNoteId)
                .Index(t => t.UserId);

            Sql(File.ReadAllText(_alterGetReviewQueuePage));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserNotification", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserNotification", "SystemItemMapId", "dbo.SystemItemMap");
            DropForeignKey("dbo.UserNotification", "MappingProjectId", "dbo.MappingProject");
            DropForeignKey("dbo.UserNotification", "MapNoteId", "dbo.MapNote");
            DropIndex("dbo.UserNotification", new[] { "UserId" });
            DropIndex("dbo.UserNotification", new[] { "MapNoteId" });
            DropIndex("dbo.UserNotification", new[] { "SystemItemMapId" });
            DropIndex("dbo.UserNotification", new[] { "MappingProjectId" });
            DropTable("dbo.UserNotification");

            Sql(File.ReadAllText(_revertGetReviewQueuePage));
        }
    }
}
