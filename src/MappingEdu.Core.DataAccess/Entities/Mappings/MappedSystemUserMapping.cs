// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class MappedSystemUserMapping : EntityTypeConfiguration<MappedSystemUser>
    {
        public MappedSystemUserMapping()
        {
            HasKey(m => new {m.MappedSystemId, m.UserId});
            ToTable("MappedSystemUser");

            Property(m => m.Role).IsRequired();

            HasRequired(m => m.MappedSystem).WithMany(m => m.Users).WillCascadeOnDelete(false);
            HasRequired(m => m.User).WithMany(m => m.MappedSystems).WillCascadeOnDelete(false);
        }
    }
}