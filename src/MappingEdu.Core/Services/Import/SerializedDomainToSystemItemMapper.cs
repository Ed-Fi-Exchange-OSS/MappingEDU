// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using MappingEdu.Common.Extensions;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingEdu.Core.Services.Auditing;
using ItemType = MappingEdu.Core.Domain.Enumerations.ItemType;

namespace MappingEdu.Core.Services.Import
{
    public interface ISerializedDomainToSystemItemMapper
    {
        void Map(SerializedDomain input, MappedSystem mappedSystem, ImportOptions options);
    }

    public class SerializedDomainToSystemItemMapper : ISerializedDomainToSystemItemMapper
    {
        private readonly IAuditor _auditor;
        private readonly ISerializedElementToEnumerationTypeMapper _elementEnumerationMapper;
        private readonly ISerializedEntityToSystemItemMapper _entityMapper;
        private readonly ISerializedEnumerationToSystemItemMapper _enumerationMapper;

        public SerializedDomainToSystemItemMapper(
            ISerializedEntityToSystemItemMapper entityMapper,
            ISerializedEnumerationToSystemItemMapper enumerationMapper,
            ISerializedElementToEnumerationTypeMapper elementEnumerationMapper,
            IAuditor auditor)
        {
            _entityMapper = entityMapper;
            _enumerationMapper = enumerationMapper;
            _elementEnumerationMapper = elementEnumerationMapper;
            _auditor = auditor;
        }

        public void Map(SerializedDomain input, MappedSystem mappedSystem, ImportOptions options)
        {
            // block if item name is over 500
            if (input.Name.Length > 500)
            {
                _auditor.Warn("Unable to add element group {0} because the name exceeds 500 characters", input.Name);
                return;
            }

            SystemItem domainModel = null;
            if (options.UpsertBasedOnName)
                domainModel = mappedSystem.SystemItems.FirstOrDefault(x => x.ParentSystemItemId == null
                                                                           && x.ItemName == input.Name);

            if (domainModel == null)
            {
                domainModel = new SystemItem
                {
                    ItemType = ItemType.Domain,
                    MappedSystem = mappedSystem,
                    IsActive = true
                };
                mappedSystem.SystemItems.Add(domainModel);
            }

            domainModel.ItemName = input.Name;
            domainModel.Definition = input.Definition;
            domainModel.IsExtended = input.IsExtended != null && (input.IsExtended.ToLower() == "true" || input.IsExtended == "1");

            // entities

            if (input.Entities != null)
                input.Entities.Do(x => _entityMapper.Map(x, domainModel, options));

            // enumerations

            if (input.Enumerations != null)
                input.Enumerations.Do(x => _enumerationMapper.Map(x, domainModel, options));

            // elements to enumeration

            _elementEnumerationMapper.Map(domainModel, options);
        }
    }
}