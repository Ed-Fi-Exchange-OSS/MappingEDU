// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class EntityHintMapping : EntityTypeConfiguration<EntityHint>
    {
        public EntityHintMapping()
        {
            HasKey(m => m.EntityHintId);
            ToTable("EntityHint");
            Property(t => t.EntityHintId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.SourceEntityId).HasColumnName("SourceEntityId").HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UniqueEntityHint", 1) { IsUnique = true }));
            Property(t => t.MappingProjectId).HasColumnName("MappingProjectId").HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UniqueEntityHint", 2) { IsUnique = true }));

            HasRequired(t => t.SourceEntity)
                .WithMany(t => t.SourceEntityHints)
                .HasForeignKey(t => t.SourceEntityId)
                .WillCascadeOnDelete(false);

            HasRequired(t => t.TargetEntity)
                .WithMany(t => t.TargetEntityHints)
                .HasForeignKey(t => t.TargetEntityId)
                .WillCascadeOnDelete(false);

            HasRequired(m => m.MappingProject).WithMany(m => m.EntityHints).WillCascadeOnDelete(false);
        }
    }
}