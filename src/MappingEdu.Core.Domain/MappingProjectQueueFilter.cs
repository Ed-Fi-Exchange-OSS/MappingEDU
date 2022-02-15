// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain {

    public class MappingProjectQueueFilter : Entity, ICloneable<MappingProjectQueueFilter>
    {
        public bool AutoMapped { get; set; }

        public bool Base { get; set; }

        public bool CreatedByColumn { get; set; }

        public bool CreationDateColumn { get; set; }

        public virtual ICollection<ApplicationUser> CreatedByUsers { get; set; } 

        public bool Extended { get; set; }

        public bool Flagged { get; set; }

        protected override Guid Id
        {
            get { return MappingProjectQueueFilterId; }
        }

        public virtual ICollection<ItemType> ItemTypes { get; set; }

        public int Length { get; set; }

        public bool MappedByColumn { get; set; }

        public Guid MappingProjectQueueFilterId { get; set; }

        public virtual MappingProject MappingProject { get; set; }

        public Guid MappingProjectId { get; set; }

        public virtual ICollection<MappingMethodType> MappingMethodTypes { get; set; }

        public string Name { get; set; }

        public int OrderColumn { get; set; }

        public string OrderDirection { get; set; }

        public virtual ICollection<SystemItem> ParentSystemItems { get; set; }

        public string Search { get; set; }

        public bool ShowInDashboard { get; set; }

        public bool Unflagged { get; set; }

        public bool UpdatedByColumn { get; set; }

        public bool UpdateDateColumn { get; set; }

        public virtual ICollection<ApplicationUser> UpdatedByUsers { get; set; } 

        public virtual ApplicationUser User { get; set; }
       
        public bool UserMapped { get; set; }

        public Guid UserId { get; set; }

        public virtual ICollection<WorkflowStatusType> WorkflowStatusTypes { get; set; }

        public MappingProjectQueueFilter Clone()
        {
            var clone = new MappingProjectQueueFilter
            {
                UserId = UserId
            };

            return clone;
        }
    }
}
