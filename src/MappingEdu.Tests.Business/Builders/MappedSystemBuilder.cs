// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Tests.Business.Builders
{
    public class MappedSystemBuilder
    {
        private readonly MappedSystem _system;

        public MappedSystemBuilder(MappedSystem system)
        {
            _system = system;
        }

        public MappedSystemBuilder()
        {
            _system = new MappedSystem();
        }

        public static implicit operator MappedSystem(MappedSystemBuilder builder)
        {
            return builder._system;
        }

        public MappedSystemBuilder WithId(Guid id)
        {
            _system.MappedSystemId = id;
            return this;
        }

        public MappedSystemBuilder WithCustomDetails(List<CustomDetailMetadata> customDetails)
        {
            _system.CustomDetailMetadata = customDetails;
            return this;
        }

        public MappedSystemBuilder WithUpdateDate(DateTime date)
        {
            _system.UpdateDate = date;
            return this;
        }

        public MappedSystemBuilder IsActive(bool isActive)
        {
            _system.IsActive = isActive;
            return this;
        }

        public MappedSystemBuilder IsActive()
        {
            _system.IsActive = true;
            return this;
        }

        public MappedSystemBuilder NotActive()
        {
            _system.IsActive = false;
            return this;
        }

        public MappedSystemBuilder WithSystemName(string systemName)
        {
            _system.SystemName = systemName;
            return this;
        }

        public MappedSystemBuilder WithSystemVersion(string systemVersion)
        {
            _system.SystemVersion = systemVersion;
            return this;
        }

        public MappedSystemBuilder WithSystemItem(SystemItem systemItem)
        {
            _system.SystemItems.Add(systemItem);
            systemItem.MappedSystemId = _system.MappedSystemId;
            systemItem.MappedSystem = _system;
            return this;
        }

        public MappedSystemBuilder WithPreviousVersion(MappedSystem oldMappedSystem)
        {
            _system.PreviousMappedSystemId = oldMappedSystem.MappedSystemId;
            _system.PreviousVersionMappedSystem = oldMappedSystem;
            oldMappedSystem.NextVersionMappedSystems.Add(_system);
            return this;
        }
    }
}