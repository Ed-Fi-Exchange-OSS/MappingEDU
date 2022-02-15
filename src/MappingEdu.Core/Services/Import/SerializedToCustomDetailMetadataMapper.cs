// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Import
{
    public interface ISerializedToCustomDetailMetadataMapper
    {
        void Map(SerializedCustomDetailMetadata input, MappedSystem mappedSystem, ImportOptions options);
    }

    public class SerializedToCustomDetailMetadataMapper : ISerializedToCustomDetailMetadataMapper
    {
        public void Map(SerializedCustomDetailMetadata input, MappedSystem mappedSystem, ImportOptions options)
        {
            CustomDetailMetadata model = null;
            if (options.UpsertBasedOnName)
                model = mappedSystem.CustomDetailMetadata.FirstOrDefault(x => x.DisplayName == input.Name);

            if (model == null)
            {
                model = new CustomDetailMetadata
                {
                    MappedSystem = mappedSystem
                };
                mappedSystem.CustomDetailMetadata.Add(model);
            }

            model.DisplayName = input.Name;
            model.IsBoolean = input.IsBoolean;
        }
    }
}