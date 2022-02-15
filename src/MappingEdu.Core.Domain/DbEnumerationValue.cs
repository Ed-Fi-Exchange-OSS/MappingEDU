// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain
{
    public class DbEnumerationValue
    {
        public object Id { get; private set; }

        public string Name { get; private set; }

        public DbEnumerationValue(object id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}