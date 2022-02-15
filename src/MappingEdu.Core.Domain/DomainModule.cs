// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Autofac;
using MappingEdu.Common;
using MappingEdu.Core.Domain.Security;
using System.Reflection;
using Module = Autofac.Module;

namespace MappingEdu.Core.Domain
{
    /// <summary>
    ///     Module for the domain tier
    /// </summary>
    public class DomainModule : Module
    {
        /// <summary>
        ///     Loads the module
        /// </summary>
        /// <param name="builder">The container builder</param>
        protected override void Load(ContainerBuilder builder)
        {
            // modules

            builder.RegisterModule(new CommonModule());

            // factories

            builder.RegisterType<IdentityFactory>().As<IIdentityFactory>().InstancePerLifetimeScope();
        }
    }
}