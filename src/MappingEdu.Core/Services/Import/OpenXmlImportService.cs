// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using ClosedXML.Excel;

namespace MappingEdu.Core.Services.Import
{
    public interface IOpenXmlImportService
    {
        ImportResult Import(Guid mappedSystemId, Stream stream, bool overrideData);
    }

    public class OpenXmlImportService : IOpenXmlImportService
    {
        private readonly IImportService _importService;
        private readonly IOpenXmlToSerializedDomainMapper _openXmlToSerializedDomainMapper;

        public OpenXmlImportService(IOpenXmlToSerializedDomainMapper openXmlToSerializedDomainMapper, IImportService importService)
        {
            _openXmlToSerializedDomainMapper = openXmlToSerializedDomainMapper;
            _importService = importService;
        }

        public ImportResult Import(Guid mappedSystemId, Stream stream, bool overrideData)
        {
            var serializedMappedSystem = new SerializedMappedSystem
            {
                Id = mappedSystemId
            };

            using (var workbook = new XLWorkbook(stream))
            {
                _openXmlToSerializedDomainMapper.Map(workbook, serializedMappedSystem, new ImportOptions { UpsertBasedOnName = true });
            }

            return _importService.Import(serializedMappedSystem, new ImportOptions {UpsertBasedOnName = true, OverrideData = overrideData});
        }
    }
}