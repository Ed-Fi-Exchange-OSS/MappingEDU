// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;

namespace MappingEdu.Core.DataAccess
{
    internal static class DataAccessHelper
    {
        public static string GetStoredProcedurePath(string folder, string file)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin" }, StringSplitOptions.None).First(), "bin/StoredProcedures/", folder, file);
        }

        public static string GetSqlTypesPath(string folder, string file)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin" }, StringSplitOptions.None).First(), "bin/SqlTypes/", folder, file);
        }
    }
}