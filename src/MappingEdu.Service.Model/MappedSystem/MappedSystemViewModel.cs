// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.Domain;

namespace MappingEdu.Service.Model.MappedSystem
{
    public class MappedSystemViewModel
    {
        public DataStandardViewModel DataStandard { get; set; }

        public DomainViewModel[] Domains { get; set; }

        public Guid MappedSystemId { get; set; }
    }
}