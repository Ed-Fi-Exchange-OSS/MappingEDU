// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class SystemItemMapMapping : EntityMappingBase<SystemItemMap>
    {
        public SystemItemMapMapping()
        {
            HasKey(t => t.SystemItemMapId);
            ToTable("SystemItemMap");
            Property(t => t.SystemItemMapId).HasColumnName("SystemItemMapId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.SourceSystemItemId).HasColumnName("SourceSystemItemId").HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UniqueMapping", 1) { IsUnique = true }));
            Property(t => t.MappingProjectId).HasColumnName("MappingProjectId").HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UniqueMapping", 2) { IsUnique = true })); ;
            Property(t => t.BusinessLogic).HasColumnName("BusinessLogic");
            Property(t => t.DeferredMapping).HasColumnName("DeferredMapping");
            Property(t => t.OmissionReason).HasColumnName("OmissionReason");
            Property(t => t.MappingStatusTypeId).HasColumnName("MappingStatusTypeId");
            Property(t => t.CompleteStatusTypeId).HasColumnName("CompleteStatusTypeId");
            Property(t => t.MappingStatusReasonTypeId).HasColumnName("MappingStatusReasonTypeId");
            Property(t => t.ExcludeInExternalReports).HasColumnName("ExcludeInExternalReports");
            Property(t => t.WorkflowStatusTypeId).HasColumnName("WorkflowStatusTypeId");
            Property(t => t.MappingMethodTypeId).HasColumnName("MappingMethodTypeId");
            Property(t => t.Flagged).HasColumnName("Flagged");
            Property(t => t.StatusNote).HasColumnName("StatusNote");

            HasRequired(t => t.SourceSystemItem)
                .WithMany(t => t.SourceSystemItemMaps)
                .HasForeignKey(t => t.SourceSystemItemId)
                .WillCascadeOnDelete(false);

            HasRequired(t => t.MappingProject)
                .WithMany(t => t.SystemItemMaps)
                .HasForeignKey(t => t.MappingProjectId)
                .WillCascadeOnDelete(false);

            HasMany(t => t.TargetSystemItems)
                .WithMany(t => t.TargetSystemItemMaps)
                .Map(m =>
                {
                    m.MapLeftKey("SystemItemMapId");
                    m.MapRightKey("SystemItemId");
                    m.ToTable("SystemItemMapAssociation");
                });

            HasOptional(t => t.MappingStatusReasonType_DoNotUse)
                .WithMany(t => t.SystemItemMaps)
                .HasForeignKey(t => t.MappingStatusReasonTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.MappingStatusType_DoNotUse)
                .WithMany(t => t.SystemItemMaps)
                .HasForeignKey(t => t.MappingStatusTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.CompleteStatusType_DoNotUse)
                .WithMany(t => t.SystemItemMaps)
                .HasForeignKey(t => t.CompleteStatusTypeId)
                .WillCascadeOnDelete(false);

            HasRequired(t => t.WorkflowStatusType_DoNotUse)
                .WithMany(t => t.SystemItemMaps)
                .HasForeignKey(t => t.WorkflowStatusTypeId)
                .WillCascadeOnDelete(false);

            HasRequired(t => t.MappingMethodType_DoNotUse)
                .WithMany(t => t.SystemItemMaps)
                .HasForeignKey(t => t.MappingMethodTypeId)
                .WillCascadeOnDelete(false);

            Ignore(t => t.MappingStatusReasonType);
            Ignore(t => t.MappingStatusType);
            Ignore(t => t.CompleteStatusType);
            Ignore(t => t.WorkflowStatusType);
            Ignore(t => t.MappingMethodType);
        }
    }
}