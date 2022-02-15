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
    public interface ISerializedElementToSystemItemMapper
    {
        void Map(SerializedElement input, SystemItem parentSystemItem, ImportOptions options);
    }

    public class SerializedElementToSystemItemMapper : ISerializedElementToSystemItemMapper
    {
        private readonly IAuditor _auditor;
        private readonly ISerializedToSystemItemCustomDetailMapper _customDetailMapper;

        public SerializedElementToSystemItemMapper(ISerializedToSystemItemCustomDetailMapper customDetailMapper, IAuditor auditor)
        {
            _auditor = auditor;
            _customDetailMapper = customDetailMapper;
        }

        public void Map(SerializedElement input, SystemItem parentSystemItem, ImportOptions options)
        {
            // block if item name is over 500
            if (input.Name.Length > 500)
            {
                _auditor.Warn("Unable to add element {0} because the name exceeds 500 characters", input.Name);
                return;
            }

            // block if data type > 255
            if (input.DataType != null && input.DataType.Length > 255)
            {
                _auditor.Warn("Unable to add element {0} because the data type exceeds 255 characters", input.Name);
                return;
            }

            // block if Technical Name > 255
            if (input.TechnicalName != null && input.TechnicalName.Length > 255)
            {
                _auditor.Warn("Unable to add element {0} because the technical name exceeds 255 characters", input.Name);
                return;
            }

            // block if Item Url > 255
            if (input.ItemUrl != null && input.ItemUrl.Length > 255)
            {
                _auditor.Warn("Unable to add element {0} because the item url exceeds 255 characters", input.Name);
                return;
            }

            SystemItem elementModel = null;
            if (options.UpsertBasedOnName)
                elementModel = parentSystemItem.ChildSystemItems.FirstOrDefault(x => x.ItemType == ItemType.Element && x.ItemName == input.Name);

            if (elementModel == null)
            {
                elementModel = new SystemItem
                {
                    ItemType = ItemType.Element,
                    MappedSystem = parentSystemItem.MappedSystem,
                    ParentSystemItem = parentSystemItem,
                    IsActive = true
                };
                parentSystemItem.ChildSystemItems.Add(elementModel);
                parentSystemItem.MappedSystem.SystemItems.Add(elementModel);
            }

            // block if in use
            if (!options.OverrideData && (elementModel.TargetSystemItemMaps.Any() || elementModel.SourceSystemItemMaps.Any()))
            {
                foreach (var sourceSystemItemMap in elementModel.SourceSystemItemMaps)
                {
                    _auditor.Warn("Unable to update entity '{0}' because it is used in source project '{1}'", elementModel.ItemName, sourceSystemItemMap.MappingProject.ProjectName);
                }
                foreach (var targetSystemItemMap in elementModel.TargetSystemItemMaps)
                {
                    _auditor.Warn("Unable to update entity '{0}' because it is used in target project '{1}'", elementModel.ItemName, targetSystemItemMap.MappingProject.ProjectName);
                }
            }
            else
            { 
                elementModel.ItemName = input.Name;
                elementModel.Definition = input.Definition;
                elementModel.DataTypeSource = input.DataType;
                elementModel.FieldLength = input.FieldLength;
                elementModel.TechnicalName = input.TechnicalName;
                elementModel.ItemUrl = input.ItemUrl;
                elementModel.TechnicalName = input.TechnicalName;
                elementModel.IsExtended = input.IsExtended != null && (input.IsExtended.ToLower() == "true" || input.IsExtended == "1");
            }

            // map custom details

            if (input.CustomDetails != null)
                input.CustomDetails.Do(x => _customDetailMapper.Map(x, elementModel, options));
        }
    }
}