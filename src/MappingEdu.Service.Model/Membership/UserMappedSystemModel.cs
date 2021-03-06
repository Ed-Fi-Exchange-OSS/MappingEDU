// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;

namespace MappingEdu.Service.Model.Membership
{
    public class UserMappedSystemModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public MappedSystemUser.MappedSystemUserRole Role { get; set; }
    }
}
