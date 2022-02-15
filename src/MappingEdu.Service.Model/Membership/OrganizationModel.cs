﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Service.Model.Membership
{
    public class OrganizationModel
    {
        public string Description { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string[] Domains { get; set; }
    }
}