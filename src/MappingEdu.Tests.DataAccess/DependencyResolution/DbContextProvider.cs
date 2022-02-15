// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Data.Entities;
using MappingEdu.Data.Entities.Services.Validation;

namespace MappingEdu.Tests.DataAccess.DependencyResolution
{
    public class DbContextProvider
    {
        private static Func<IContext, DatabaseContext> _databaseContext = context => { throw new Exception("DbContextProvider not initialized."); };

        public static void AlwaysUseInstance(DatabaseContext instance)
        {
            _databaseContext = context => instance;
        }

        public static void AlwaysCreateNewInstance()
        {
            _databaseContext = context => new DatabaseContext(context.GetAllInstances<IValidateEntity>().ToArray());
        }

        public static DatabaseContext DatabaseContext(IContext context)
        {
            return _databaseContext(context);
        }
    }
}