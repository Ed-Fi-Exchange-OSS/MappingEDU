// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Common;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Services;

namespace MappingEdu.Core.DataAccess.Services
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly EntityContext _databaseContext;

        public DatabaseConnection(EntityContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public DbConnection Connection
        {
            get { return _databaseContext.Database.Connection; }
        }

        public string Database
        {
            get { return Connection.Database; }
        }

        public string DataSource
        {
            get { return Connection.DataSource; }
        }
    }
}