// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using DocumentFormat.OpenXml.Packaging;

namespace MappingEdu.Core.Services.Import
{
    public interface ISpreadsheetDocumentWrapper
    {
        SpreadsheetDocument SpreadsheetDocument { get; set; }
    }

    public class SpreadsheetDocumentWrapper : ISpreadsheetDocumentWrapper
    {
        public SpreadsheetDocument SpreadsheetDocument { get; set; }
    }
}