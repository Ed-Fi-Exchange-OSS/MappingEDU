// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Service.Model.Logging
{
    public enum SeverityLevel
    {
        Info,
        Debug,
        Warn,
        Error,
        Fatal
    }

    public class LoggingCreateModel
    {
        public SeverityLevel Level { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }
    }
}