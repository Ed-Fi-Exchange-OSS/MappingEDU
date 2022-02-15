// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class MappedSystemUpdateMapping : EntityTypeConfiguration<MappedSystemUpdate>
    {
        public MappedSystemUpdateMapping()
        {
            HasKey(m => new { m.MappedSystemId, m.UserId });
            ToTable("MappedSystemUpdate");

            Property(m => m.UpdateDate).IsRequired();

            HasRequired(m => m.MappedSystem).WithMany(m => m.UserUpdates).WillCascadeOnDelete(false);
            HasRequired(m => m.User).WithMany(m => m.MappedSystemUpdates).WillCascadeOnDelete(false);
        }
    }
}