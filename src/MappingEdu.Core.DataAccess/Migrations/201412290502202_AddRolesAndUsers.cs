// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Migrations;

namespace MappingEdu.Core.DataAccess.Migrations
{
    public partial class AddRolesAndUsers : DbMigration
    {
        public override void Up()
        {
            // create new users
            Sql(@"IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MappingEduUser')
                    IF EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MappingEduUser')
                        CREATE USER [MappingEduUser] FOR LOGIN [MappingEduUser] WITH DEFAULT_SCHEMA=[dbo]");

            Sql(@"IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MappingEduUser')
                    IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'MappingEduUser')
                        CREATE USER [MappingEduUser] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]");

            // create new roles
            Sql(@"CREATE ROLE [MappingEdu_User] AUTHORIZATION [dbo]");

            // add new users to roles
            Sql(@"EXEC sp_addrolemember N'MappingEdu_User', N'MappingEduUser'");

            // grant and deny access to schemas for new roles
            Sql(@"GRANT ALTER ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"REVOKE CONTROL ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"GRANT DELETE ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"GRANT EXECUTE ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"GRANT INSERT ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"REVOKE REFERENCES ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"GRANT SELECT ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"REVOKE TAKE OWNERSHIP ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"GRANT UPDATE ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"REVOKE VIEW CHANGE TRACKING ON SCHEMA :: dbo TO MappingEdu_User");

            Sql(@"GRANT VIEW DEFINITION ON SCHEMA :: dbo TO MappingEdu_User");
        }

        public override void Down()
        {
            // remove user from role
            Sql(@"EXEC sp_droprolemember N'MappingEdu_User', N'MappingEduUser'");

            // if the role has no more members, drop it
            Sql(@"IF NOT EXISTS (SELECT * FROM sys.database_role_members WHERE role_principal_id =
                                    (SELECT principal_id FROM sys.database_principals WHERE type = 'R' AND name = 'MappingEdu_User'))
                    DROP ROLE [MappingEdu_User]");

            // remove the user
            Sql(@"DROP USER MappingEduUser");
        }
    }
}