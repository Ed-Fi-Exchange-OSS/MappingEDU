// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Import
{
    public interface ISerializedElementToEnumerationTypeMapper
    {
        void Map(SystemItem domain, ImportOptions options);
    }

    public class SerializedElementToEnumerationTypeMapper : ISerializedElementToEnumerationTypeMapper
    {
        private SystemItem _domain;

        public void Map(SystemItem domain, ImportOptions options)
        {
            if (null == domain)
                return;

            _domain = domain;

            foreach (var systemItem in domain.ChildSystemItems)
            {
                MapChildren(systemItem);
            }
        }

        private void MapChildren(SystemItem systemItem)
        {
            if (systemItem.ItemType == ItemType.Element)
            {
                var systemItemEnumerationType = _domain.ChildSystemItems.SingleOrDefault(x => x.ItemName == systemItem.DataTypeSource &&
                                                                                              x.ItemType == ItemType.Enumeration);

                // assign enumation if found

                if (null != systemItemEnumerationType)
                {
                    systemItem.EnumerationSystemItem = systemItemEnumerationType;
                    systemItem.ItemDataType = ItemDataType.Enumeration;
                }
            }

            if (systemItem.ChildSystemItems.Any())
                systemItem.ChildSystemItems.Do(x => MapChildren(x));
        }
    }
}