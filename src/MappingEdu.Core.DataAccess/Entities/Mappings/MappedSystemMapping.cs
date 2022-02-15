// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class MappedSystemMapping : EntityMappingBase<MappedSystem>
    {
        public MappedSystemMapping()
        {
            HasKey(t => t.MappedSystemId);

            ToTable("MappedSystem");
            Property(t => t.MappedSystemId).HasColumnName("MappedSystemId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.SystemName).HasColumnName("SystemName").IsRequired().HasMaxLength(50);
            Property(t => t.SystemVersion).HasColumnName("SystemVersion").IsRequired().HasMaxLength(50);
            Property(t => t.PreviousMappedSystemId).HasColumnName("PreviousMappedSystemId").IsOptional();
            Property(t => t.ClonedFromMappedSystemId).HasColumnName("ClonedFromMappedSystemId").IsOptional();
            Property(t => t.IsActive).HasColumnName("IsActive").IsRequired();
            Property(t => t.IsPublic).HasColumnName("IsPublic").IsRequired();

            HasOptional(t => t.PreviousVersionMappedSystem)
                .WithMany(x => x.NextVersionMappedSystems)
                .HasForeignKey(x => x.PreviousMappedSystemId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.ClonedFromMappedSystem)
                .WithMany(x => x.Clones)
                .HasForeignKey(x => x.ClonedFromMappedSystemId);
        }
    }
}