// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.EnumerationItemMapping;
using MappingEdu.Service.Model.MapNote;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Service.Model.EntityHints
{
    public class EntityHintEntityModel
    {
        public string DomainItemPath { get; set; }

        public int ItemTypeId { get; set; }

        public Guid SystemItemId { get; set; }
    }

    public class EntityHintViewModel
    {
        public Guid EntityHintId { get; set; }

        public Guid MappingProjectId { get; set; }

        public Guid SourceEntityId { get; set; }

        public EntityHintEntityModel SourceEntity { get; set; }

        public Guid TargetEntityId { get; set; }

        public EntityHintEntityModel TargetEntity { get; set; }
    }
}