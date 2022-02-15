// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class MappingProjectQueueFilterMapping : EntityTypeConfiguration<MappingProjectQueueFilter>
    {
        public MappingProjectQueueFilterMapping()
        {
            HasKey(m => m.MappingProjectQueueFilterId);
            ToTable("MappingProjectQueueFilter");
            Property(t => t.MappingProjectQueueFilterId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasMany(x => x.CreatedByUsers).WithMany(x => x.CreatedByFilters).Map(configuration => configuration.MapLeftKey("MappingProjectQueueFilterId").MapRightKey("CreatedByUserId").ToTable("MappingProjectQueueFilterCreatedByUser"));
            HasMany(x => x.ItemTypes).WithMany(x => x.MappingProjectQueueFilters).Map(configuration => configuration.MapLeftKey("MappingProjectQueueFilterId").MapRightKey("ItemTypeId").ToTable("MappingProjectQueueFilterItemType"));
            HasMany(x => x.MappingMethodTypes).WithMany(x => x.MappingProjectQueueFilters).Map(configuration => configuration.MapLeftKey("MappingProjectQueueFilterId").MapRightKey("MappingMethodTypeId").ToTable("MappingProjectQueueFilterMappingMethodType"));
            HasRequired(m => m.MappingProject).WithMany(m => m.MappingProjectQueueFilters).WillCascadeOnDelete(true);
            HasMany(x => x.ParentSystemItems).WithMany(x => x.MappingProjectQueueFilters).Map(configuration => configuration.MapLeftKey("MappingProjectQueueFilterId").MapRightKey("SystemItemId").ToTable("MappingProjectQueueFilterParentSystemItem"));
            HasMany(x => x.WorkflowStatusTypes).WithMany(x => x.MappingProjectQueueFilters).Map(configuration => configuration.MapLeftKey("MappingProjectQueueFilterId").MapRightKey("WorkflowStatusTypeId").ToTable("MappingProjectQueueFilterWorkflowStatusType"));
            HasMany(x => x.UpdatedByUsers).WithMany(x => x.UpdatedByFilters).Map(configuration => configuration.MapLeftKey("MappingProjectQueueFilterId").MapRightKey("UpdatedByUserId").ToTable("MappingProjectQueueFilterUpdatedByUser"));
            HasRequired(m => m.User).WithMany(m => m.MappingProjectQueueFilters).WillCascadeOnDelete(false);
        }
    }
}