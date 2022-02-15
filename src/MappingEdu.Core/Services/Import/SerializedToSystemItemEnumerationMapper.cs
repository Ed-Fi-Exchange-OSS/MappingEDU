// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Import
{
    public interface ISerializedToSystemItemEnumerationMapper
    {
        void Map(SerializedEnumerationValue input, SystemItem parentSystemItem, ImportOptions options);
    }

    public class SerializedToSystemItemEnumerationMapper : ISerializedToSystemItemEnumerationMapper
    {
        public void Map(SerializedEnumerationValue input, SystemItem parentSystemItem, ImportOptions options)
        {
            SystemEnumerationItem systemEnumerationItemModel = null;
            if (options.UpsertBasedOnName)
                systemEnumerationItemModel = parentSystemItem.SystemEnumerationItems.FirstOrDefault(x => x.CodeValue == input.CodeValue);

            if (systemEnumerationItemModel == null)
            {
                systemEnumerationItemModel = new SystemEnumerationItem
                {
                    SystemItem = parentSystemItem
                };
                parentSystemItem.SystemEnumerationItems.Add(systemEnumerationItemModel);
            }

            systemEnumerationItemModel.CodeValue = input.CodeValue;
            systemEnumerationItemModel.ShortDescription = input.ShortDescription;
            systemEnumerationItemModel.Description = input.Description;
        }
    }
}