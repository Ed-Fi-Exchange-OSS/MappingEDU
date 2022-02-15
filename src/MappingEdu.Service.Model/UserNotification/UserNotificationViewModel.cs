// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Service.Model.ElementList;

namespace MappingEdu.Service.Model.UserNotification
{
    public class UserNotificationViewModel
    {
        public ElementListViewModel.ElementPathViewModel.ElementSegment Element { get; set; }

        public ElementListViewModel.ElementPathViewModel.PathSegment[] PathSegments { get; set; }

        public Guid SystemItemId { get; set; }

        public Guid MappingProjectId { get; set; }

        public bool HasSeen { get; set; }

        public string Notification { get; set; }

        public ICollection<Guid> UserNotificationIds { get; set; } 

        public DateTime NotificationDate { get; set; }
    }
}