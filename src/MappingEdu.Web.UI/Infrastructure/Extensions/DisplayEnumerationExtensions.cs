// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.Common;

namespace MappingEdu.Web.UI.Infrastructure.Extensions
{
    public static class DisplayEnumerationExtensions
    {
        public static DisplayEnumerationElement[] ForDisplay<T>(this IEnumerable<IDisplayEnumeration<T>> values)
        {
            return values.Select(ForDisplay).ToArray();
        }

        public static DisplayEnumerationElement ForDisplay<T>(this IDisplayEnumeration<T> value)
        {
            return new DisplayEnumerationElement {DisplayText = value.DisplayText, Id = value.Id};
        }
    }
}