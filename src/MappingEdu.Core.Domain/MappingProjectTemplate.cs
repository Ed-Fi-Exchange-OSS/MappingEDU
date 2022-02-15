// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Domain
{
    public class MappingProjectTemplate : Entity, ICloneable<MappingProjectTemplate>
    {
        protected override Guid Id
        {
            get { return MappingProjectTemplateId; }
        }

        public Guid MappingProjectTemplateId { get; set; }

        public virtual MappingProject MappingProject { get; set; }

        public Guid MappingProjectId { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public MappingProjectTemplate Clone()
        {
            var clone = new MappingProjectTemplate
            {
                Template = Template,
                Title = Title
            };

            return clone;
        }
    }
}
