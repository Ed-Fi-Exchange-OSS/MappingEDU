// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using MappingEdu.Core.Domain.System;

namespace MappingEdu.Core.DataAccess.Util
{
    public class PagedResult<T> where T : class
    {
        public PagedResult(ICollection<T> items)
        {
            Items = items;
        }

        public PagedResult()
        {
            Items = new HashSet<T>();
        }

        public ICollection<T> Items { get; set; }

        public int TotalFiltered { get; set; }

        public int TotalRecords { get; set; }
    }
}