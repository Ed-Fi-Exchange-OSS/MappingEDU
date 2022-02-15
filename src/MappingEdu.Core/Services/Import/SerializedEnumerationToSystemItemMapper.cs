// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Auditing;

namespace MappingEdu.Core.Services.Import
{
    public interface ISerializedEnumerationToSystemItemMapper
    {
        void Map(SerializedEnumeration input, SystemItem parentSystemItem, ImportOptions options);
    }

    public class SerializedEnumerationToSystemItemMapper : ISerializedEnumerationToSystemItemMapper
    {
        private readonly ISerializedToSystemItemEnumerationMapper _systemItemEnumerationMapper;
        private readonly ISerializedToSystemItemCustomDetailMapper _customDetailMapper;
        private readonly IAuditor _auditor;

        public SerializedEnumerationToSystemItemMapper(ISerializedToSystemItemEnumerationMapper systemItemEnumerationMapper, ISerializedToSystemItemCustomDetailMapper customDetailMapper, IAuditor auditor)
        {
            _systemItemEnumerationMapper = systemItemEnumerationMapper;
            _customDetailMapper = customDetailMapper;
            _auditor = auditor;
        }

        public void Map(SerializedEnumeration input, SystemItem parentSystemItem, ImportOptions options)
        {
            // block if item name is over 500
            if (input.Name.Length > 500)
            {
                _auditor.Warn("Unable to add enumeration {0} because the name exceeds 500 characters", input.Name);
                return;
            }

            SystemItem enumerationModel = null;
            if (options.UpsertBasedOnName)
                enumerationModel = parentSystemItem.ChildSystemItems.FirstOrDefault(x => Equals(x.ItemType, ItemType.Enumeration)
                                                                                         && x.ItemName == input.Name);

            if (enumerationModel == null)
            {
                enumerationModel = new SystemItem
                {
                    ItemType = ItemType.Enumeration,
                    MappedSystem = parentSystemItem.MappedSystem,
                    ParentSystemItem = parentSystemItem,
                    IsActive = true
                };
                parentSystemItem.ChildSystemItems.Add(enumerationModel);
                parentSystemItem.MappedSystem.SystemItems.Add(enumerationModel);
            }

            enumerationModel.ItemName = input.Name;
            enumerationModel.Definition = input.Definition;
            enumerationModel.IsExtended = input.IsExtended != null && (input.IsExtended.ToLower() == "true" || input.IsExtended == "1");

            if (input.EnumerationValues != null)
                input.EnumerationValues.Do(x => _systemItemEnumerationMapper.Map(x, enumerationModel, options));

            if (input.CustomDetails != null)
                input.CustomDetails.Do(x => _customDetailMapper.Map(x, enumerationModel, options));
        }
    }
}