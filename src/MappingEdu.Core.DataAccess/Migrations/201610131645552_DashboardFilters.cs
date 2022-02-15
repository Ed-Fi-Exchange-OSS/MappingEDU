// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.IO;
using System.Linq;

namespace MappingEdu.Core.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DashboardFilters : DbMigration
    {
        private readonly string _createGetDashboardQueueFilters = DataAccessHelper.GetStoredProcedurePath("GetDashboardQueueFilters", "Create.sql");
        private readonly string _dropGetDashboardQueueFilters = DataAccessHelper.GetStoredProcedurePath("GetDashboardQueueFilters", "Drop.sql");

        public override void Up()
        {
            RenameTable(name: "dbo.MappingProjectQueueWorkflowStatusType", newName: "MappingProjectQueueFilterWorkflowStatusType");

            Sql(@"
                SET IDENTITY_INSERT WorkflowStatusType ON
                GO

                DELETE FROM WorkflowStatusType WHERE WorkflowStatusTypeId=0
                GO

                INSERT INTO WorkflowStatusType(WorkflowStatusTypeId, WorkflowStatusTypeName) VALUES(0,'Unmapped')
                GO

                SET IDENTITY_INSERT WorkflowStatusType OFF
            ");

            Sql(File.ReadAllText(_createGetDashboardQueueFilters));
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.MappingProjectQueueFilterWorkflowStatusType", newName: "MappingProjectQueueWorkflowStatusType");

            Sql(File.ReadAllText(_dropGetDashboardQueueFilters));
        }
    }
}
