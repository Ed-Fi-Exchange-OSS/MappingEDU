// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Tests.Business.Builders
{
    public class SystemItemVersionDeltaBuilder
    {
        private readonly SystemItemVersionDelta _systemItemVersionDelta;

        public SystemItemVersionDeltaBuilder(SystemItemVersionDelta systemItemVersionDelta)
        {
            _systemItemVersionDelta = systemItemVersionDelta;
        }

        public SystemItemVersionDeltaBuilder()
        {
            _systemItemVersionDelta = BuildDefault();
        }

        private static SystemItemVersionDelta BuildDefault()
        {
            return new SystemItemVersionDelta {SystemItemVersionDeltaId = Guid.NewGuid()};
        }

        public SystemItemVersionDeltaBuilder WithId(Guid systemItemVersionDeltaId)
        {
            _systemItemVersionDelta.SystemItemVersionDeltaId = systemItemVersionDeltaId;
            return this;
        }

        public SystemItemVersionDeltaBuilder WithNewSystemItem(SystemItem systemItem)
        {
            _systemItemVersionDelta.NewSystemItem = systemItem;
            _systemItemVersionDelta.NewSystemItemId = systemItem.SystemItemId;

            systemItem.NewSystemItemVersionDeltas.Add(_systemItemVersionDelta);
            return this;
        }

        public SystemItemVersionDeltaBuilder WithOldSystemItem(SystemItem systemItem)
        {
            _systemItemVersionDelta.OldSystemItem = systemItem;
            _systemItemVersionDelta.OldSystemItemId = systemItem.SystemItemId;

            systemItem.OldSystemItemVersionDeltas.Add(_systemItemVersionDelta);
            return this;
        }

        public SystemItemVersionDeltaBuilder WithChangeType(int changeTypeId)
        {
            _systemItemVersionDelta.ItemChangeTypeId = changeTypeId;
            return this;
        }

        public static implicit operator SystemItemVersionDelta(SystemItemVersionDeltaBuilder builder)
        {
            return builder._systemItemVersionDelta;
        }
    }
}