// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace MappingEdu.Core.DataAccess.Util
{
    internal static class MappingExtensions
    {
        /// <summary>
        ///     Maps an index to a property
        /// </summary>
        /// <param name="configuration">The property configuration</param>
        /// <param name="name">The index name</param>
        /// <param name="isUnique">Indicates if the index is unique</param>
        public static StringPropertyConfiguration HasIndex(this StringPropertyConfiguration configuration, string name, bool isUnique = false)
        {
            return configuration.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute(name) {IsUnique = isUnique}));
        }
    }
}

