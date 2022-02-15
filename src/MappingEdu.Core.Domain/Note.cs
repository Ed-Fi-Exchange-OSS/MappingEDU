// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class Note : Entity
    {
        protected override Guid Id
        {
            get { return NoteId; }
        }

        public Guid NoteId { get; set; }

        public string Notes { get; set; }

        public virtual SystemItem SystemItem { get; set; }

        public Guid SystemItemId { get; set; }

        public string Title { get; set; }
    }
}