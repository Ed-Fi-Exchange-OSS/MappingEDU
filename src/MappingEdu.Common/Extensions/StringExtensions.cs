// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Text.RegularExpressions;

namespace MappingEdu.Common.Extensions
{
    public static class StringExtensions
    {
        public static string SpaceCamelHumps(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                    ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
                );
        }
        public static bool EqualsIgnoreCase(this string str, string comparison)
        {
            return str.Equals(comparison, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}