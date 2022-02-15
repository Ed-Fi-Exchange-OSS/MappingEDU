// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MappingEdu.Service.Model.Membership
{
    public class CurrentUserModel : UserModel
    {
        public CurrentUserModel(UserModel user)
        {
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;
            FirstName = user.FirstName;
            Id = user.Id;
            IsAdministrator = user.IsAdministrator;
            IsGuest = user.IsGuest;
            LastName = user.LastName;
            Roles = user.Roles;
            UserName = user.UserName;
        }
        
        public ICollection<UserMappedSystemModel> MappedSystems { get; set; }

        public ICollection<UserMappingProjectModel> Projects { get; set; }
    }
}