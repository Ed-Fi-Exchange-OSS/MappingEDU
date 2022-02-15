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
    public interface ISerializedEntityToSystemItemMapper
    {
        void Map(SerializedEntity input, SystemItem parentSystemItem, ImportOptions options);
    }

    public class SerializedEntityToSystemItemMapper : ISerializedEntityToSystemItemMapper
    {
        private readonly IAuditor _auditor;
        private readonly ISerializedElementToSystemItemMapper _elementMapper;

        public SerializedEntityToSystemItemMapper(ISerializedElementToSystemItemMapper elementMapper, IAuditor auditor)
        {
            _auditor = auditor;
            _elementMapper = elementMapper;
        }

        public void Map(SerializedEntity input, SystemItem parentSystemItem, ImportOptions options)
        {
            // block if item name is over 500
            if (input.Name.Length > 500)
            {
                _auditor.Warn("Unable to add entity {0} because the name exceeds 500 characters", input.Name);
                return;
            }

            SystemItem entityModel = null;
            if (options.UpsertBasedOnName)
                entityModel = parentSystemItem.ChildSystemItems.FirstOrDefault(x => (Equals(x.ItemType, ItemType.Entity) ||
                                                                                     Equals(x.ItemType, ItemType.SubEntity))
                                                                                    && x.ItemName == input.Name);

            if (entityModel == null)
            {
                entityModel = new SystemItem
                {
                    ItemType = Equals(parentSystemItem.ItemType, ItemType.Domain) ? ItemType.Entity : ItemType.SubEntity,
                    MappedSystem = parentSystemItem.MappedSystem,
                    ParentSystemItem = parentSystemItem,
                    IsActive = true
                };
                parentSystemItem.ChildSystemItems.Add(entityModel);
                parentSystemItem.MappedSystem.SystemItems.Add(entityModel);
            }

            // block if in use
            if (!options.OverrideData && (entityModel.TargetSystemItemMaps.Any() || entityModel.SourceSystemItemMaps.Any()))
            {
                _auditor.Warn("Unable to update entity {0} because it is used in a project", entityModel.ItemName);
                return;
            }

            entityModel.ItemName = input.Name;
            entityModel.Definition = input.Definition;
            entityModel.IsExtended = input.IsExtended != null && (input.IsExtended.ToLower() == "true" || input.IsExtended == "1");

            if (input.SubEntities != null)
                input.SubEntities.Do(x => Map(x, entityModel, options));

            if (input.Elements != null)
                input.Elements.Do(x => _elementMapper.Map(x, entityModel, options));
        }
    }
}