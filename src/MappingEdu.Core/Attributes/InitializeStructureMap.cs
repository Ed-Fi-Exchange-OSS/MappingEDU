// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MappingEdu.Core.Attributes
{
    /// <summary>
    ///     Used to mark an assembly that structure map will use to find interfaces
    ///     to provide concrete implementations for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class InitializeStructureMap : Attribute
    {
    }
}