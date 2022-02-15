// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MappingEdu.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> UnionElement<T>(this IEnumerable<T> enumerable, T element)
        {
            var list = new List<T> {element};
            return enumerable.Union(list);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            return new List<T> {element};
        }

        public static void Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T element)
        {
            var list = new List<T> {element};
            return enumerable.Except(list);
        }

        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> enumerable, Func<T, bool> exclude)
        {
            return enumerable.Where(x => !exclude(x));
        }

        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> search)
        {
            return !enumerable.Any(search);
        }
    }
}