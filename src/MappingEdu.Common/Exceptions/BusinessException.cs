// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Common.Exceptions
{
    /// <summary>
    ///     Exception used for business logic errors.
    /// </summary>
    /// <remarks>
    ///     Handled upstream in API application to return 400 (BadRequest) responses.
    /// </remarks>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
