// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public abstract class EntityMappingBase<T> : EntityTypeConfiguration<T> where T : Entity
    {
        protected EntityMappingBase()
        {
            MapAuditable();
        }

        /// <summary>
        ///     Maps the auditable fields.
        /// </summary>
        protected void MapAuditable()
        {
            Property(x => x.CreateDate).IsRequired();
            Property(x => x.CreateBy).HasMaxLength(120).IsRequired();
            Property(x => x.UpdateDate).IsOptional();
            Property(x => x.UpdateBy).HasMaxLength(120).IsOptional();
        }
    }
}