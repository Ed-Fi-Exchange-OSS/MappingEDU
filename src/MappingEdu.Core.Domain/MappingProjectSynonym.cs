// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Domain
{
    public class MappingProjectSynonym : Entity, ICloneable<MappingProjectSynonym>
    {
        protected override Guid Id
        {
            get { return MappingProjectSynonymId; }
        }

        public Guid MappingProjectSynonymId { get; set; }

        public Guid MappingProjectId { get; set; }

        public virtual MappingProject MappingProject { get; set; }

        public string SourceWord { get; set; }

        public string TargetWord { get; set; }

        public MappingProjectSynonym Clone()
        {
            var clone = new MappingProjectSynonym
            {
                SourceWord = SourceWord,
                TargetWord = TargetWord
            };

            return clone;
        }
    }
}
