// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class SystemItemCustomDetailMapping : EntityMappingBase<SystemItemCustomDetail>
    {
        public SystemItemCustomDetailMapping()
        {
            HasKey(t => t.SystemItemCustomDetailId);
            ToTable("SystemItemCustomDetail");
            Property(t => t.SystemItemCustomDetailId).HasColumnName("SystemItemCustomDetailId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Value).HasColumnName("Value");

            HasRequired(t => t.CustomDetailMetadata)
                .WithMany(t => t.SystemItemCustomDetails)
                .HasForeignKey(t => t.CustomDetailMetadataId)
                .WillCascadeOnDelete(true);

            HasRequired(t => t.SystemItem)
                .WithMany(t => t.SystemItemCustomDetails)
                .HasForeignKey(t => t.SystemItemId)
                .WillCascadeOnDelete(false);
        }
    }
}