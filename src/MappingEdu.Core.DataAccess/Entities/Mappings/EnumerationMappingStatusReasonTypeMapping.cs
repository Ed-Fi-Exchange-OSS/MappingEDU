// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class EnumerationMappingStatusReasonTypeMapping : EntityTypeConfiguration<EnumerationMappingStatusReasonType>
    {
        public EnumerationMappingStatusReasonTypeMapping()
        {
            HasKey(t => t.EnumerationMappingStatusReasonTypeId);
            ToTable("EnumerationMappingStatusReasonType");
            Property(t => t.EnumerationMappingStatusReasonTypeId).HasColumnName("EnumerationMappingStatusReasonTypeId");
            Property(t => t.EnumerationMappingStatusReasonTypeName).HasColumnName("EnumerationMappingStatusReasonTypeName").IsRequired().HasMaxLength(50);
        }
    }
}