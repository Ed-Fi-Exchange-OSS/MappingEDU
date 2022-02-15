// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Tests.Business.Builders
{
    public class SystemItemCustomDetailBuilder
    {
        private readonly SystemItemCustomDetail _systemItemCustomDetail;

        public SystemItemCustomDetailBuilder(SystemItemCustomDetail systemItemCustomDetail)
        {
            _systemItemCustomDetail = systemItemCustomDetail;
        }

        public SystemItemCustomDetailBuilder()
        {
            _systemItemCustomDetail = BuildDefault();
        }

        private static SystemItemCustomDetail BuildDefault()
        {
            return new SystemItemCustomDetail {SystemItemCustomDetailId = Guid.NewGuid()};
        }

        public SystemItemCustomDetailBuilder WithId(Guid id)
        {
            _systemItemCustomDetail.SystemItemCustomDetailId = id;
            return this;
        }

        public SystemItemCustomDetail WithValue(string value)
        {
            _systemItemCustomDetail.Value = value;
            return this;
        }

        public SystemItemCustomDetailBuilder WithSystemItem(SystemItem systemItem)
        {
            _systemItemCustomDetail.SystemItem = systemItem;
            _systemItemCustomDetail.SystemItemId = systemItem.SystemItemId;
            systemItem.SystemItemCustomDetails.Add(_systemItemCustomDetail);
            return this;
        }

        public SystemItemCustomDetailBuilder WithCustomDetailMetadata(CustomDetailMetadata customDetailMetadata)
        {
            _systemItemCustomDetail.CustomDetailMetadata = customDetailMetadata;
            _systemItemCustomDetail.CustomDetailMetadataId = customDetailMetadata.CustomDetailMetadataId;
            customDetailMetadata.SystemItemCustomDetails.Add(_systemItemCustomDetail);
            return this;
        }

        public static implicit operator SystemItemCustomDetail(SystemItemCustomDetailBuilder builder)
        {
            return builder._systemItemCustomDetail;
        }
    }
}