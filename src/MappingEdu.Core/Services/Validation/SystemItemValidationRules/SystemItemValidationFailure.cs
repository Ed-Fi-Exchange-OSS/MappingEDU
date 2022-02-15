// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;

namespace MappingEdu.Core.Services.Validation.SystemItemValidationRules
{
    public class SystemItemValidationFailure : IValidationFailure
    {
        public Guid MappedSystemId { get; set; }

        public string MappedSystemName { get; set; }

        public string SystemItemName { get; set; }

        public ItemType SystemItemType { get; set; }

        public SystemItemValidationFailure()
        {
            SystemItemType = ItemType.Unknown;
        }

        public string FullMessage
        {
            get { return string.Format("System Item [Mapped System Id: '{0}', System Item Id: '{1}']: {2}", MappedSystemId, Id, ValidationError); }
        }

        public string ShortMessage
        {
            get { return string.Format("'{0}' {1} for '{2}': {3}", SystemItemName, SystemItemType.Name, MappedSystemName, ValidationError); }
        }

        public string ValidationError { get; set; }

        public string PropertyName { get; set; }

        public Guid Id { get; set; }

        public Type FailingRule { get; set; }
    }
}