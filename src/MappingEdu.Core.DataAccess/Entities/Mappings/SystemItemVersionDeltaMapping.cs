// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class SystemItemVersionDeltaMapping : EntityMappingBase<SystemItemVersionDelta>
    {
        public SystemItemVersionDeltaMapping()
        {
            HasKey(t => t.SystemItemVersionDeltaId);
            ToTable("SystemItemVersionDelta");
            Property(t => t.SystemItemVersionDeltaId).HasColumnName("SystemItemVersionDeltaId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.OldSystemItemId).HasColumnName("OldSystemItemId").IsOptional();
            Property(t => t.NewSystemItemId).HasColumnName("NewSystemItemId").IsOptional();
            Property(t => t.ItemChangeTypeId).HasColumnName("ItemChangeTypeId");
            Property(t => t.Description).HasColumnName("Description").HasMaxLength(255);

            HasOptional(t => t.NewSystemItem)
                .WithMany(t => t.NewSystemItemVersionDeltas)
                .HasForeignKey(t => t.NewSystemItemId)
                .WillCascadeOnDelete(true);

            HasOptional(t => t.OldSystemItem)
                .WithMany(t => t.OldSystemItemVersionDeltas)
                .HasForeignKey(t => t.OldSystemItemId)
                .WillCascadeOnDelete(false);

            HasRequired(t => t.ItemChangeType_DoNotUse)
                .WithMany(t => t.SystemItemVersionDeltas)
                .HasForeignKey(t => t.ItemChangeTypeId)
                .WillCascadeOnDelete(false);

            Ignore(t => t.ItemChangeType);
        }
    }
}