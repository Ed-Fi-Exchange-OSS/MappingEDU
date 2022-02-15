// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Tests.Business.Builders
{
    public class EnumerationItemMapBuilder
    {
        private readonly SystemEnumerationItemMap _enumerationItemMap;

        public EnumerationItemMapBuilder()
        {
            _enumerationItemMap = BuildDefault();
        }

        private static SystemEnumerationItemMap BuildDefault()
        {
            return new SystemEnumerationItemMap {SystemEnumerationItemMapId = Guid.NewGuid()};
        }

        public EnumerationItemMapBuilder WithId(Guid id)
        {
            _enumerationItemMap.SystemEnumerationItemMapId = id;
            return this;
        }

        public EnumerationItemMapBuilder WithSourceSystemEnumerationItem(SystemEnumerationItem systemEnumerationItem)
        {
            _enumerationItemMap.SourceSystemEnumerationItem = systemEnumerationItem;
            _enumerationItemMap.SourceSystemEnumerationItemId = systemEnumerationItem.SystemEnumerationItemId;
            systemEnumerationItem.SourceSystemEnumerationItemMaps.Add(_enumerationItemMap);
            return this;
        }

        public static implicit operator SystemEnumerationItemMap(EnumerationItemMapBuilder builder)
        {
            return builder._enumerationItemMap;
        }

        public EnumerationItemMapBuilder WithTargetSystemEnumerationItem(SystemEnumerationItem targetEnumerationItem)
        {
            _enumerationItemMap.TargetSystemEnumerationItem = targetEnumerationItem;
            _enumerationItemMap.TargetSystemEnumerationItemId = targetEnumerationItem.SystemEnumerationItemId;
            targetEnumerationItem.TargetSystemEnumerationItemMaps.Add(_enumerationItemMap);
            return this;
        }

        public EnumerationItemMapBuilder WithSystemItemMap(SystemItemMap systemItemMap)
        {
            _enumerationItemMap.SystemItemMap = systemItemMap;
            _enumerationItemMap.SystemItemMapId = systemItemMap.SystemItemMapId;
            systemItemMap.SystemEnumerationItemMaps.Add(_enumerationItemMap);
            return this;
        }
    }
}