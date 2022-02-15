// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Common.Exceptions
{
    /// <summary>
    ///     Exception used when the requested business object cannot be found.
    /// </summary>
    /// <remarks>
    ///     Handled upstream in API application to return 404 (NotFound) responses.
    /// </remarks>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, params object[] args) : base(string.Format(message, args))
        {
        }
    }
}