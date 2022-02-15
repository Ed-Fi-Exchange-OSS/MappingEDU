// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using MappingEdu.Core.Domain.System;
using MappingEdu.Service.Model.ElementList;

namespace MappingEdu.Service.Util
{
    public static class PathSegmentHelper
    {
        public static ElementListViewModel.ElementPathViewModel.PathSegment[] GetPathSegments(SystemItem item,
            List<ElementListViewModel.ElementPathViewModel.PathSegment> parents = null)
        {
            if (item.ParentSystemItem == null)
                return parents != null ? parents.ToArray() : null;

            if (parents == null)
                parents = new List<ElementListViewModel.ElementPathViewModel.PathSegment>();

            parents.Insert(0, new ElementListViewModel.ElementPathViewModel.PathSegment
            {
                SystemItemId = item.ParentSystemItem.SystemItemId,
                Name = item.ParentSystemItem.ItemName,
                Definition = item.ParentSystemItem.Definition
            });
            return GetPathSegments(item.ParentSystemItem, parents);
        }
    }
}