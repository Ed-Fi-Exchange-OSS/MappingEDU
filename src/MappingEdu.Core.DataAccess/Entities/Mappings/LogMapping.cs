// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class LogMapping : EntityTypeConfiguration<Log>
    {
        public LogMapping()
        {
            HasKey(t => t.LogId);
            ToTable("Log");
            Property(t => t.LogId).HasColumnName("LogId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Date).HasColumnName("Date").IsRequired();
            Property(t => t.Thread).HasColumnName("Thread").IsRequired().HasMaxLength(255);
            Property(t => t.User).HasColumnName("User").IsRequired().HasMaxLength(50);
            Property(t => t.Level).HasColumnName("Level").IsRequired().HasMaxLength(50);
            Property(t => t.Logger).HasColumnName("Logger").IsRequired().HasMaxLength(255);
            Property(t => t.Message).HasColumnName("Message").IsRequired().HasMaxLength(4000);
            Property(t => t.Exception).HasColumnName("Exception").HasMaxLength(2000);
        }
    }
}