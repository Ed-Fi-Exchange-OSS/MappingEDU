// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class CustomDetailMetadataMapping : EntityMappingBase<CustomDetailMetadata>
    {
        public CustomDetailMetadataMapping()
        {
            HasKey(t => t.CustomDetailMetadataId);
            ToTable("CustomDetailMetadata");
            Property(t => t.CustomDetailMetadataId).HasColumnName("CustomDetailMetadataId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.DisplayName).HasColumnName("DisplayName").IsRequired().HasMaxLength(100);
            Property(t => t.IsBoolean).HasColumnName("IsBoolean");
            Property(t => t.IsCoreDetail).HasColumnName("IsCoreDetail");

            HasRequired(t => t.MappedSystem)
                .WithMany(t => t.CustomDetailMetadata)
                .HasForeignKey(t => t.MappedSystemId)
                .WillCascadeOnDelete(true);
        }
    }
}