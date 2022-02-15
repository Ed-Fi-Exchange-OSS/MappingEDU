// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class SystemEnumerationItemMapMapping : EntityMappingBase<SystemEnumerationItemMap>
    {
        public SystemEnumerationItemMapMapping()
        {
            HasKey(t => t.SystemEnumerationItemMapId);
            ToTable("SystemEnumerationItemMap");
            Property(t => t.SystemEnumerationItemMapId).HasColumnName("SystemEnumerationItemMapId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.SourceSystemEnumerationItemId).HasColumnName("SourceSystemEnumerationItemId");
            Property(t => t.SystemItemMapId).HasColumnName("SystemItemMapId");
            Property(t => t.DeferredMapping).HasColumnName("DeferredMapping");
            Property(t => t.EnumerationMappingStatusTypeId).HasColumnName("EnumerationMappingStatusTypeId");
            Property(t => t.EnumerationMappingStatusReasonTypeId).HasColumnName("EnumerationMappingStatusReasonTypeId");

            HasRequired(t => t.SystemItemMap)
                .WithMany(t => t.SystemEnumerationItemMaps)
                .HasForeignKey(t => t.SystemItemMapId)
                .WillCascadeOnDelete(true);

            HasRequired(t => t.SourceSystemEnumerationItem)
                .WithMany(t => t.SourceSystemEnumerationItemMaps)
                .HasForeignKey(t => t.SourceSystemEnumerationItemId)
                .WillCascadeOnDelete(true);

            HasOptional(t => t.TargetSystemEnumerationItem)
                .WithMany(t => t.TargetSystemEnumerationItemMaps)
                .HasForeignKey(t => t.TargetSystemEnumerationItemId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.EnumerationMappingStatusType_DoNotUse)
                .WithMany(t => t.SystemEnumerationItemMaps)
                .HasForeignKey(t => t.EnumerationMappingStatusTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.EnumerationMappingStatusReasonType_DoNotUse)
                .WithMany(t => t.SystemEnumerationItemMaps)
                .HasForeignKey(t => t.EnumerationMappingStatusReasonTypeId)
                .WillCascadeOnDelete(false);

            Ignore(t => t.EnumerationMappingStatusReasonType);
            Ignore(t => t.EnumerationMappingStatusType);
        }
    }
}