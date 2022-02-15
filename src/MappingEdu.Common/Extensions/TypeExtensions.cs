// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsConcrete(this Type t)
        {
            return !t.IsAbstract;
        }

        public static bool CanBeCastTo<TOther>(this Type t)
        {
            return typeof (TOther).IsAssignableFrom(t);
        }

        public static bool InstanceCanBeCastTo<TOther>(this object o)
        {
            return typeof (TOther).IsAssignableFrom(o.GetType());
        }
    }
}