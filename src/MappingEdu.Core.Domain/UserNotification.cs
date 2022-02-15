// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using CuttingEdge.Conditions;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class UserNotification : Entity
    {

        public UserNotification()
        {
        }

        public bool HasSeen { get; set; }

        protected override Guid Id
        {
            get { return UserNotificationId; }
        }

        public bool IsDismissed { get; set; }

        public virtual MappingProject MappingProject { get; set; }

        public Guid? MappingProjectId { get; set; }

        public string Notification { get; set; }

        public virtual SystemItemMap SystemItemMap { get; set; }

        public Guid? SystemItemMapId { get; set; }

        public virtual MapNote MapNote { get; set; }

        public Guid? MapNoteId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }

        public Guid UserNotificationId { get; set; }
    }
}
