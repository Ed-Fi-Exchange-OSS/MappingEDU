// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using Autofac;
using MappingEdu.Common.Configuration;
using Module = Autofac.Module;

namespace MappingEdu.Common
{
    /// <summary>
    ///     Module for the Common tier
    /// </summary>
    public class CommonModule : Module
    {
        /// <summary>
        ///     Loads the module
        /// </summary>
        /// <param name="builder">The container builder</param>
        protected override void Load(ContainerBuilder builder)
        {
            // configurationManager store

            builder.RegisterType<ConfigurationManagerStore>().As<IConfigurationStore>().InstancePerLifetimeScope();

        }
    }
}