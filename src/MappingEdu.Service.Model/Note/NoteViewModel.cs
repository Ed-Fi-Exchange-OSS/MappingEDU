// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.Note
{
    public class NoteViewModel
    {
        public DateTime CreateDate { get; set; }

        public string CreateBy { get; set; }

        public Guid? CreateById { get; set; }

        public bool IsEdited { get; set; }

        public Guid NoteId { get; set; }

        public string Notes { get; set; }

        public string Title { get; set; }
    }
}