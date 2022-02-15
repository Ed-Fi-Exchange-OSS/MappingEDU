// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Services.Validation
{
    public interface IValidationFailure
    {
        Type FailingRule { get; }

        string FullMessage { get; }

        Guid Id { get; }

        string PropertyName { get; }

        string ShortMessage { get; }

        string ValidationError { get; }

        Type GetType();
    }
}