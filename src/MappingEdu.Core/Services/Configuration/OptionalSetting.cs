// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Services.Configuration
{
    public class OptionalSetting<T>
    {
        public bool IsSet { get; private set; }

        public T Value { get; private set; }

        private OptionalSetting()
        {
        }

        public OptionalSetting(T value)
        {
            Value = value;
            IsSet = true;
        }

        public static OptionalSetting<T> Empty()
        {
            return new OptionalSetting<T> {IsSet = false};
        }
    }
}