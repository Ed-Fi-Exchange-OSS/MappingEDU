// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.Services.Import
{
    public interface ISerializedToSystemItemCustomDetailMapper
    {
        void Map(SerializedElementCustomDetail input, SystemItem systemItem, ImportOptions options);
    }

    public class SerializedToSystemItemCustomDetailMapper : ISerializedToSystemItemCustomDetailMapper
    {
        public void Map(SerializedElementCustomDetail input, SystemItem systemItem, ImportOptions options)
        {
            // may want to add the custom detail metadata id to the SerializedElementCustomDetail in the future
            var customDetailMetaData = systemItem.MappedSystem.CustomDetailMetadata.FirstOrDefault(x => x.DisplayName == input.Name);
            if (customDetailMetaData == null)
                return;

            //var systemItemCustomDetailModel = systemItem.SystemItemCustomDetails.FirstOrDefault(x => x.CustomDetailMetadataId == customDetailMetaData.CustomDetailMetadataId);
            var systemItemCustomDetailModel = systemItem.SystemItemCustomDetails.FirstOrDefault(x => x.CustomDetailMetadata.DisplayName == customDetailMetaData.DisplayName);
            if (systemItemCustomDetailModel == null)
            {
                systemItemCustomDetailModel = new SystemItemCustomDetail
                {
                    CustomDetailMetadata = customDetailMetaData,
                    SystemItem = systemItem
                };

                systemItem.SystemItemCustomDetails.Add(systemItemCustomDetailModel);
            }

            systemItemCustomDetailModel.Value = input.Value;
        }
    }
}