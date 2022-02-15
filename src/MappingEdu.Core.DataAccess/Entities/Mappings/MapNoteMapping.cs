// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class MapNoteMapping : EntityMappingBase<MapNote>
    {
        public MapNoteMapping()
        {
            HasKey(t => t.MapNoteId);
            ToTable("MapNote");
            Property(t => t.MapNoteId).HasColumnName("MapNoteId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.SystemItemMapId).HasColumnName("SystemItemMapId");
            Property(t => t.Title).HasColumnName("Title").IsRequired().HasMaxLength(100);
            Property(t => t.Notes).HasColumnName("Notes").IsRequired();

            HasRequired(t => t.SystemItemMap)
                .WithMany(t => t.MapNotes)
                .HasForeignKey(t => t.SystemItemMapId)
                .WillCascadeOnDelete(true);
        }
    }
}