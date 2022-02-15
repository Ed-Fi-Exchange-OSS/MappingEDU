// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class UserNotificationMapping : EntityTypeConfiguration<UserNotification>
    {
        public UserNotificationMapping()
        {
            HasKey(n => n.UserNotificationId);
            ToTable("UserNotification");

            Property(t => t.UserNotificationId).HasColumnName("UserNotificationId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(n => n.HasSeen).HasColumnName("HasSeen");
            Property(n => n.IsDismissed).HasColumnName("IsDismissed");
            Property(n => n.Notification).HasColumnName("Notification");
            Property(n => n.MappingProjectId).IsOptional();
            Property(n => n.SystemItemMapId).IsOptional();
            Property(n => n.MapNoteId).IsOptional();

            HasOptional(n => n.MappingProject)
                .WithMany(m => m.UserNotifications)
                .HasForeignKey(n => n.MappingProjectId)
                .WillCascadeOnDelete(false);

            HasOptional(n => n.SystemItemMap)
                .WithMany(m => m.UserNotifications)
                .HasForeignKey(n => n.SystemItemMapId)
                .WillCascadeOnDelete(true);

            HasOptional(n => n.MapNote)
                .WithMany(m => m.UserNotifications)
                .HasForeignKey(n => n.MapNoteId)
                .WillCascadeOnDelete(false);

            HasRequired(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .WillCascadeOnDelete(true);
        }
    }
}