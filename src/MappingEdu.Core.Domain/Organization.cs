// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using CuttingEdge.Conditions;
using MappingEdu.Core.Domain.Security;

namespace MappingEdu.Core.Domain
{
    public class Organization : Entity
    {
        protected Organization()
        {
            Users = new HashSet<ApplicationUser>();
        }

        public Organization(string name) : this()
        {
            Condition.Requires(name, "name").IsNotNullOrEmpty().IsShorterOrEqual(75);
            Name = name;
        }

        public string Description { get; set; }

        public string Domains { get; set; }

        protected override Guid Id
        {
            get { return OrganizationId; }
        }

        public string Name { get; set; }

        public Guid OrganizationId { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}