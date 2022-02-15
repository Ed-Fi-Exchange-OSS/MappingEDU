// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MappingEdu.Core.DataAccess.Util;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Entities.Mappings
{
    public class SystemContantMapping : EntityTypeConfiguration<SystemConstant>
    {
        public SystemContantMapping()
        {
            Property(x => x.Name).HasMaxLength(100).IsRequired().HasIndex("UX_Name", true);

            Property(t => t.SystemConstantTypeId).HasColumnName("SystemConstantTypeId");

            Ignore(t => t.SystemConstantType);
        }
    }
}
