// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class SystemItemMapping : EntityMappingBase<SystemItem>
    {
        public SystemItemMapping()
        {
            HasKey(t => t.SystemItemId);
            ToTable("SystemItem");
            Property(t => t.SystemItemId).HasColumnName("SystemItemId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.MappedSystemId).HasColumnName("MappedSystemId");
            Property(t => t.ParentSystemItemId).HasColumnName("ParentSystemItemId").IsOptional();
            Property(t => t.ItemName).HasColumnName("ItemName").IsRequired().HasMaxLength(500);
            Property(t => t.Definition).HasColumnName("Definition");
            Property(t => t.ItemTypeId).HasColumnName("ItemTypeId");
            Property(t => t.ItemDataTypeId).HasColumnName("ItemDataTypeId");
            Property(t => t.DataTypeSource).HasColumnName("DataTypeSource").HasMaxLength(255);
            Property(t => t.FieldLength).HasColumnName("FieldLength");
            Property(t => t.ItemUrl).HasColumnName("ItemUrl").HasMaxLength(255);
            Property(t => t.TechnicalName).HasColumnName("TechnicalName").HasMaxLength(255);
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.DomainItemPath).HasColumnName("DomainItemPath");
            Property(t => t.DomainItemPathIds).HasColumnName("DomainItemPathIds");
            Property(t => t.IsExtendedPath).HasColumnName("IsExtendedPath");


            HasRequired(t => t.MappedSystem)
                .WithMany(t => t.SystemItems)
                .HasForeignKey(t => t.MappedSystemId)
                .WillCascadeOnDelete(true);

            HasOptional(t => t.ParentSystemItem)
                .WithMany(t => t.ChildSystemItems)
                .HasForeignKey(t => t.ParentSystemItemId)
                .WillCascadeOnDelete(false);

            HasRequired(t => t.ItemType_DoNotUse)
                .WithMany(t => t.SystemItems)
                .HasForeignKey(t => t.ItemTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.ItemDataType_DoNotUse)
                .WithMany(t => t.SystemItems)
                .HasForeignKey(t => t.ItemDataTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.EnumerationSystemItem)
                .WithMany(t => t.EnumerationUsages)
                .HasForeignKey(t => t.EnumerationSystemItemId)
                .WillCascadeOnDelete(false);
       
            HasOptional(t => t.ElementGroupSystemItem)
                .WithMany(t => t.ElementGroupChildItems)
                .HasForeignKey(t => t.ElementGroupSystemItemId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.MappedSystemExtension)
                .WithMany(t => t.SystemItems)
                .HasForeignKey(t => t.MappedSystemExtensionId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.CopiedFromSystemItem)
                .WithMany(t => t.SystemItemCopies)
                .HasForeignKey(t => t.CopiedFromSystemItemId)
                .WillCascadeOnDelete(false);

            Property(t => t.DomainItemPath);
            Property(t => t.DomainItemPathIds);
            Property(t => t.IsExtendedPath);

            Ignore(t => t.ItemType);
            Ignore(t => t.ItemDataType);
            Ignore(t => t.NextSystemItemVersionDeltas);
            Ignore(t => t.PreviousSystemItemVersionDeltas);
        }
    }
}