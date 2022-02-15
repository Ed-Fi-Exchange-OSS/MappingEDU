// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.DataAccess.Migrations;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Services;
using MappingEdu.Core.Services.Configuration;

namespace MappingEdu.Core.DataAccess.Services
{
    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly EntityContext _context;
        private readonly IIdentityFactory _identityFactory;
        private DbMigrator _migrator;

        private DbMigrator DbMigrator
        {
            get
            {
                if (_migrator == null)
                {
                    var config = new Migrations.Configuration(_identityFactory)
                    {
                        MigrationsNamespace = typeof (Base).Namespace,
                        MigrationsAssembly = typeof (Base).Assembly,
                        TargetDatabase = new DbConnectionInfo(MappingEduConfiguration.ProdAdminConnectionStringNameKey)
                    };
                    _migrator = new DbMigrator(config);
                }
                return _migrator;
            }
        }

        public DatabaseMigrator(EntityContext context, IIdentityFactory identityFactory)
        {
            _context = context;
            _identityFactory = identityFactory;
        }

        public bool HasMigrationsApplied
        {
            get
            {
                const string sql =
                    @"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__MigrationHistory]') AND type in (N'U'))
                        SELECT 1 as res ELSE SELECT 0 as res;";
                var result = _context.Database.SqlQuery<int>(sql).Single();
                return result == 1;
            }
        }

        public bool DatabaseIsUpToDate
        {
            get { return DbMigrator.GetPendingMigrations().ToArray().Length == 0; }
        }

        public string CurrentVersion
        {
            get { return AppliedMigrations.Last(); }
        }

        public string[] LocalMigrations
        {
            get { return DbMigrator.GetLocalMigrations().OrderBy(x => x).ToArray(); }
        }

        public string[] AppliedMigrations
        {
            get { return DbMigrator.GetDatabaseMigrations().OrderBy(x => x).ToArray(); }
        }

        public string[] PendingMigrations
        {
            get { return DbMigrator.GetPendingMigrations().OrderBy(x => x).ToArray(); }
        }

        public void ApplyPendingMigrations()
        {
            DbMigrator.Update();
        }
    }
}