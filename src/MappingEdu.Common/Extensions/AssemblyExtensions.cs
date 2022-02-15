// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Reflection;

namespace MappingEdu.Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] GetAllTypesClosing(this Assembly assembly, Type genericType)
        {
            var types = assembly.GetTypes()
                .Where(t => t.BaseType != null)
                .Where(t => t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == genericType)
                .ToArray();

            return types;
        }

        public static Type[] GetTypesWithAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
        {
            return assembly
                .GetTypes()
                .Where(t => t.GetCustomAttributes(typeof (TAttribute), true).Length > 0)
                .ToArray();
        }
    }
}