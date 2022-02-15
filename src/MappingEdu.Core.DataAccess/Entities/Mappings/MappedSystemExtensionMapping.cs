// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    internal class MappedSystemExtensionMapping : EntityMappingBase<MappedSystemExtension>
    {
        public MappedSystemExtensionMapping()
        {
            HasKey(x => x.MappedSystemExtensionId);
            ToTable("MappedSystemExtension");
            Property(t => t.MappedSystemExtensionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.ShortName).HasMaxLength(5);

            HasRequired(t => t.MappedSystem)
                .WithMany(t => t.Extensions)
                .HasForeignKey(t => t.MappedSystemId)
                .WillCascadeOnDelete(false);

            HasOptional(t => t.ExtensionMappedSystem)
                .WithMany(t => t.ExtensionOfs)
                .HasForeignKey(t => t.ExtensionMappedSystemId)
                .WillCascadeOnDelete(false);
        }
    }
}