// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class OrganizationMapping : EntityMappingBase<Organization>
    {
        public OrganizationMapping()
        {
            HasKey(t => t.OrganizationId);
            Property(t => t.Description);
            Property(t => t.Domains).HasMaxLength(1000);
            Property(t => t.OrganizationId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).HasMaxLength(75).IsRequired().HasIndex("UX_Name", true);
            ToTable("Organization");

            HasMany(o => o.Users).WithMany(p => p.Organizations).Map(m =>
            {
                m.MapLeftKey("OrganizationId");
                m.MapRightKey("UserId");
                m.ToTable("OrganizationUsers");
            });
        }
    }
}