// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MappingEdu.Core.Domain;
using SystemConstantType = MappingEdu.Core.Domain.Enumerations.SystemConstantType;

namespace MappingEdu.Service.Model.Admin
{
    public class SystemConstantModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public bool BooleanValue { get; set; }

        public bool IsPrivate { get; set; }

        public string TypeName { get; set; }
        
        public string Group { get; set; }

        public SystemConstantModel() { }

        public SystemConstantModel(SystemConstant constant, bool includePrivateValue = false)
        {
            var boolValue = false;

            if (constant.SystemConstantType == SystemConstantType.Boolean)
                if (!bool.TryParse(constant.Value, out boolValue))
                    boolValue = false;

            Id = constant.Id;
            Name = constant.Name;
            Value = (!constant.IsPrivate || (includePrivateValue && constant.IsPrivate)) ? constant.Value : null;
            BooleanValue = boolValue;
            IsPrivate = constant.IsPrivate;
            Group = constant.Group;
            TypeName = constant.SystemConstantType.DisplayText;
        }
    }
}
