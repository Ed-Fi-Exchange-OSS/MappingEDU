// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Models;
using System;

namespace UnitTests.Builders
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

        public MappedSystemBuilder WithMinimalPersistanceProperties(string systemName, string systemVersion)
        {
            _system.SystemName = systemName;
            _system.SystemVersion = systemVersion;
            _system.CreateDate = DateTime.Now;
            return this;
        }

        public MappedSystemBuilder WithId(Guid id)
        {
            _system.SystemId = id;
            return this;
        }

        public MappedSystemBuilder WithUpdateDate(DateTime date)
        {
            _system.UpdateDate = date;
            return this;
        }

        public MappedSystemBuilder WithSystemCategory(SystemCategory category)
        {
            _system.SystemCategories.Add(category);
            return this;
        }
    }
}
