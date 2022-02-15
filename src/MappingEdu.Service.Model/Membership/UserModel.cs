// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Service.Model.Membership
{
    public class UserModel
    {
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string FirstName { get; set; }

        public string Id { get; set; }

        public bool IsActive { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsGuest { get; set; }

        public string LastName { get; set; }

        public string[] Roles { get; set; }

        public string UserName { get; set; }
    }
}