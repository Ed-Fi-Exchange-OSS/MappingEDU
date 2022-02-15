// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Domain
{
    public class Log
    {
        public DateTime Date { get; set; }

        public string Exception { get; set; }

        public string Level { get; set; }

        public string Logger { get; set; }

        public int LogId { get; set; }

        public string Message { get; set; }

        public string Thread { get; set; }

        public string User { get; set; }
    }
}