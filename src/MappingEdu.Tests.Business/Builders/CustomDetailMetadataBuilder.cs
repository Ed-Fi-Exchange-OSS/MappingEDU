// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;

namespace MappingEdu.Tests.Business.Builders
{
    public class CustomDetailMetadataBuilder
    {
        private readonly CustomDetailMetadata _customDetailMetadata;

        public CustomDetailMetadataBuilder()
        {
            _customDetailMetadata = BuildDefault();
        }

        private static CustomDetailMetadata BuildDefault()
        {
            return new CustomDetailMetadata {CustomDetailMetadataId = Guid.NewGuid()};
        }

        public CustomDetailMetadataBuilder WithId(Guid id)
        {
            _customDetailMetadata.CustomDetailMetadataId = id;
            return this;
        }

        public CustomDetailMetadataBuilder WithDisplayName(string displayName)
        {
            _customDetailMetadata.DisplayName = displayName;
            return this;
        }

        public CustomDetailMetadataBuilder WithIsBoolean(bool isBoolean)
        {
            _customDetailMetadata.IsBoolean = isBoolean;
            return this;
        }

        public CustomDetailMetadataBuilder WithIsCoreDetail(bool isCoreDetail)
        {
            _customDetailMetadata.IsCoreDetail = isCoreDetail;
            return this;
        }

        public CustomDetailMetadataBuilder WithMappedSystem(MappedSystem mappedSystem)
        {
            _customDetailMetadata.MappedSystem = mappedSystem;
            _customDetailMetadata.MappedSystemId = mappedSystem.MappedSystemId;
            mappedSystem.CustomDetailMetadata.Add(_customDetailMetadata);
            return this;
        }

        public static implicit operator CustomDetailMetadata(CustomDetailMetadataBuilder builder)
        {
            return builder._customDetailMetadata;
        }
    }
}