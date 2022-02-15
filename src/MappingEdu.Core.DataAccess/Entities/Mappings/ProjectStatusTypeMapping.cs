// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class ProjectStatusTypeMapping : EntityTypeConfiguration<ProjectStatusType>
    {
        public ProjectStatusTypeMapping()
        {
            HasKey(t => t.ProjectStatusTypeId);
            ToTable("ProjectStatusType");
            Property(t => t.ProjectStatusTypeId).HasColumnName("ProjectStatusTypeId");
            Property(t => t.ProjectStatusTypeName).HasColumnName("ProjectStatusTypeName").IsRequired().HasMaxLength(20);
        }
    }
}