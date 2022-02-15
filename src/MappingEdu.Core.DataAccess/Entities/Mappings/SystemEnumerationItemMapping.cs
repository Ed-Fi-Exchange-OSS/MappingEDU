// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class SystemEnumerationItemMapping : EntityMappingBase<SystemEnumerationItem>
    {
        public SystemEnumerationItemMapping()
        {
            HasKey(t => t.SystemEnumerationItemId);
            ToTable("SystemEnumerationItem");
            Property(t => t.SystemEnumerationItemId).HasColumnName("SystemEnumerationItemId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.SystemItemId).HasColumnName("SystemItemId");
            Property(t => t.CodeValue).HasColumnName("CodeValue").IsRequired().HasMaxLength(1024);
            Property(t => t.Description).HasColumnName("Description").HasMaxLength(1024);
            Property(t => t.ShortDescription).HasColumnName("ShortDescription").HasMaxLength(1024);

            HasRequired(t => t.SystemItem)
                .WithMany(t => t.SystemEnumerationItems)
                .HasForeignKey(t => t.SystemItemId)
                .WillCascadeOnDelete(true);
        }
    }
}