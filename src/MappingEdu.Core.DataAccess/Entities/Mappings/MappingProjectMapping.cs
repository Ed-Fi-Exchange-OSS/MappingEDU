// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class MappingProjectMapping : EntityMappingBase<MappingProject>
    {
        public MappingProjectMapping()
        {
            HasKey(t => t.MappingProjectId);
            ToTable("MappingProject");
            Property(t => t.MappingProjectId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.ProjectName).HasColumnName("ProjectName").HasMaxLength(50).IsRequired();
            Property(t => t.Description).HasColumnName("Description").HasMaxLength(1000).IsRequired();
            Property(t => t.SourceDataStandardMappedSystemId).HasColumnName("SourceDataStandardMappedSystemId");
            Property(t => t.TargetDataStandardMappedSystemId).HasColumnName("TargetDataStandardMappedSystemId");
            Property(t => t.ProjectStatusTypeId).HasColumnName("ProjectStatusTypeId").IsRequired();
            Property(t => t.IsActive).HasColumnName("IsActive").IsRequired();
            Property(t => t.IsPublic).HasColumnName("IsPublic").IsRequired();

            HasRequired(t => t.TargetDataStandard)
                .WithMany(t => t.MappingProjectsWhereTarget)
                .HasForeignKey(t => t.TargetDataStandardMappedSystemId)
                .WillCascadeOnDelete(false);

            HasRequired(t => t.SourceDataStandard)
                .WithMany(t => t.MappingProjectsWhereSource)
                .HasForeignKey(t => t.SourceDataStandardMappedSystemId)
                .WillCascadeOnDelete(false);

            Ignore(t => t.ProjectStatusType);
        }
    }
}