// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using DocumentFormat.OpenXml.Packaging;
using MappingEdu.Core.Services.Import;

namespace MappingEdu.Tests.Business.Core.Services.ImportExport
{
    public class TestSpreadsheetDocumentWrapper : ISpreadsheetDocumentWrapper
    {
        public SpreadsheetDocument SpreadsheetDocument { get; set; }
    }
}