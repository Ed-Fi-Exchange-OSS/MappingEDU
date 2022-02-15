// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Tests.Business.Builders
{
    public class SystemEnumerationItemBuilder
    {
        private readonly SystemEnumerationItem _systemEnumerationItem;

        public SystemEnumerationItemBuilder()
        {
            _systemEnumerationItem = BuildDefault();
        }

        private static SystemEnumerationItem BuildDefault()
        {
            return new SystemEnumerationItem {SystemEnumerationItemId = Guid.NewGuid()};
        }

        public SystemEnumerationItemBuilder WithDescription(string description)
        {
            _systemEnumerationItem.Description = description;
            return this;
        }

        public SystemEnumerationItemBuilder WithShortDescription(string shortDescription)
        {
            _systemEnumerationItem.ShortDescription = shortDescription;
            return this;
        }

        public SystemEnumerationItemBuilder WithSystemItem(SystemItem systemItem)
        {
            _systemEnumerationItem.SystemItemId = systemItem.SystemItemId;
            _systemEnumerationItem.SystemItem = systemItem;
            systemItem.SystemEnumerationItems.Add(_systemEnumerationItem);
            return this;
        }

        public SystemEnumerationItemBuilder WithEnumerationItemMap(SystemEnumerationItemMap enumerationItemMap)
        {
            _systemEnumerationItem.SourceSystemEnumerationItemMaps.Add(enumerationItemMap);
            enumerationItemMap.SourceSystemEnumerationItemId = _systemEnumerationItem.SystemEnumerationItemId;
            enumerationItemMap.SourceSystemEnumerationItem = _systemEnumerationItem;
            return this;
        }

        public SystemEnumerationItemBuilder WithMinimalPersistenceProperties(SystemItem systemItem, string codeValue)
        {
            WithSystemItem(systemItem);
            _systemEnumerationItem.CodeValue = codeValue;
            return this;
        }

        public static implicit operator SystemEnumerationItem(SystemEnumerationItemBuilder builder)
        {
            return builder._systemEnumerationItem;
        }

        public SystemEnumerationItemBuilder WithCodeValue(string codeValue)
        {
            _systemEnumerationItem.CodeValue = codeValue;
            return this;
        }

        public SystemEnumerationItemBuilder WithId(Guid systemEnumerationItemId)
        {
            _systemEnumerationItem.SystemEnumerationItemId = systemEnumerationItemId;
            return this;
        }
    }
}