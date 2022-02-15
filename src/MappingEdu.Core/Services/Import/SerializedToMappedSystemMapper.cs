// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Import
{
    public interface ISerializedToMappedSystemMapper
    {
        void Map(SerializedMappedSystem input, MappedSystem mappedSystem, ImportOptions importOptions);
    }

    public class SerializedToMappedSystemMapper : ISerializedToMappedSystemMapper
    {
        private readonly ISerializedToCustomDetailMetadataMapper _customDetailMapper;
        private readonly ISerializedDomainToSystemItemMapper _domainMapper;

        public SerializedToMappedSystemMapper(ISerializedDomainToSystemItemMapper domainMapper, ISerializedToCustomDetailMetadataMapper customDetailMapper)
        {
            _domainMapper = domainMapper;
            _customDetailMapper = customDetailMapper;
        }

        public void Map(SerializedMappedSystem input, MappedSystem mappedSystem, ImportOptions importOptions)
        {
            if (input.CustomDetails != null)
                input.CustomDetails.Do(x => _customDetailMapper.Map(x, mappedSystem, importOptions));

            if (input.Domains != null)
                input.Domains.Do(x => _domainMapper.Map(x, mappedSystem, importOptions));
        }
    }
}