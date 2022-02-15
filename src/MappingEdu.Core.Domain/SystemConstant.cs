// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain
{
    public class SystemConstant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Group { get; set; }

        public bool IsPrivate { get; set; }

        public Enumerations.SystemConstantType SystemConstantType
        {
            get { return Enumerations.SystemConstantType.GetByIdOrDefault(SystemConstantTypeId); }
            set { SystemConstantTypeId = value.Id; }
        }

        public int SystemConstantTypeId { get; set; }
    }
}