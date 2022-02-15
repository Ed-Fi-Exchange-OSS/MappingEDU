// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class MapNote : Entity, ICloneable<MapNote>
    {
        protected override Guid Id
        {
            get { return MapNoteId; }
        }

        public Guid MapNoteId { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public virtual SystemItemMap SystemItemMap { get; set; }

        public Guid SystemItemMapId { get; set; }

        public string Title { get; set; }

        public MapNote Clone()
        {
            var clone = new MapNote
            {
                Notes = Notes,
                Title = Title
            };
            return clone;
        }
    }
}