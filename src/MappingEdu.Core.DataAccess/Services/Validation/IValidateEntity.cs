// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Validation;
using MappingEdu.Core.Domain;

namespace MappingEdu.Core.DataAccess.Services.Validation
{
    public interface IValidateEntity
    {
        /// <summary>
        ///     Validates an entity before adding it to the data context.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>An array of <see cref="DbValidationError" /> objects.</returns>
        DbValidationError[] ValidateForAdd(IEntity entity);

        /// <summary>
        ///     Validates an entity before updating it in the data context.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>An array of <see cref="DbValidationError" /> objects.</returns>
        DbValidationError[] ValidateForUpdate(IEntity entity);

        /// <summary>
        ///     Determines whether this instance can validate the specified entity.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>A boolean indicating if the entity can be validated.</returns>
        bool CanValidate(IEntity entity);
    }
}