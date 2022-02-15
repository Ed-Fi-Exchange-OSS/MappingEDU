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
    public class MappingProjectSynonymMapping : EntityTypeConfiguration<MappingProjectSynonym>
    {
        public MappingProjectSynonymMapping()
        {
            HasKey(m => m.MappingProjectSynonymId);
            ToTable("MappingProjectSynonym");
            Property(t => t.MappingProjectSynonymId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasRequired(m => m.MappingProject).WithMany(m => m.MappingProjectSynonyms).WillCascadeOnDelete(true);
        }
    }
}