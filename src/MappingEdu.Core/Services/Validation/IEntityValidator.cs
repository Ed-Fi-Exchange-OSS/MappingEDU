// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MappingEdu.Core.Domain;

namespace MappingEdu.Core.Services.Validation
{
    public interface IEntityValidator<TEntity, TFailure>
        where TEntity : IEntity
        where TFailure : IValidationFailure
    {
        bool IsValid(TEntity entity);

        TFailure[] Validate(TEntity entity);

        TFailure[] ValidateForAdd(TEntity entity);

        TFailure[] ValidateForUpdate(TEntity entity);

        ValidationRuleDescription[] GetAllRuleDescriptions();
    }
}