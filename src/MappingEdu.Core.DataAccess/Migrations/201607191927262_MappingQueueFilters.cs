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
    
    public partial class MappingQueueFilters : DbMigration
    {

        private readonly string _alterGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter7.sql");
        private readonly string _revertGetReviewQueuePage = DataAccessHelper.GetStoredProcedurePath("GetReviewQueuePage", "Alter6.sql");

        private readonly string _alterGetMappingProjectItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectItemMaps", "Alter3.sql");
        private readonly string _revertGetMappingProjectItemMaps = DataAccessHelper.GetStoredProcedurePath("GetMappingProjectItemMaps", "Alter2.sql");

        public override void Up()
        {
            AddColumn("dbo.SystemItemMap", "IsAutoMapped", c => c.Boolean(nullable: false));
            AddColumn("dbo.SystemItemMap", "CreateById", c => c.Guid());
            AddColumn("dbo.SystemItemMap", "UpdateById", c => c.Guid());
            AddColumn("dbo.MapNote", "CreateById", c => c.Guid());
            AddColumn("dbo.MapNote", "UpdateById", c => c.Guid());
            AddColumn("dbo.MappingProject", "CreateById", c => c.Guid());
            AddColumn("dbo.MappingProject", "UpdateById", c => c.Guid());
            AddColumn("dbo.EntityHint", "CreateById", c => c.Guid());
            AddColumn("dbo.EntityHint", "UpdateById", c => c.Guid());
            AddColumn("dbo.SystemItem", "CreateById", c => c.Guid());
            AddColumn("dbo.SystemItem", "UpdateById", c => c.Guid());
            AddColumn("dbo.MappedSystem", "CreateById", c => c.Guid());
            AddColumn("dbo.MappedSystem", "UpdateById", c => c.Guid());
            AddColumn("dbo.CustomDetailMetadata", "CreateById", c => c.Guid());
            AddColumn("dbo.CustomDetailMetadata", "UpdateById", c => c.Guid());
            AddColumn("dbo.SystemItemCustomDetail", "CreateById", c => c.Guid());
            AddColumn("dbo.SystemItemCustomDetail", "UpdateById", c => c.Guid());
            AddColumn("dbo.Organization", "CreateById", c => c.Guid());
            AddColumn("dbo.Organization", "UpdateById", c => c.Guid());
            AddColumn("dbo.SystemItemVersionDelta", "CreateById", c => c.Guid());
            AddColumn("dbo.SystemItemVersionDelta", "UpdateById", c => c.Guid());
            AddColumn("dbo.Note", "CreateById", c => c.Guid());
            AddColumn("dbo.Note", "UpdateById", c => c.Guid());
            AddColumn("dbo.SystemEnumerationItem", "CreateById", c => c.Guid());
            AddColumn("dbo.SystemEnumerationItem", "UpdateById", c => c.Guid());
            AddColumn("dbo.SystemEnumerationItemMap", "CreateById", c => c.Guid());
            AddColumn("dbo.SystemEnumerationItemMap", "UpdateById", c => c.Guid());
            AddColumn("dbo.MappingProjectSynonym", "CreateById", c => c.Guid());
            AddColumn("dbo.MappingProjectSynonym", "UpdateById", c => c.Guid());
            AddColumn("dbo.CustomMigration", "CreateById", c => c.Guid());
            AddColumn("dbo.CustomMigration", "UpdateById", c => c.Guid());

            var models = new[]
            {
                "SystemItemMap", "MapNote", "MappingProject", "EntityHint", "SystemItem", "MappedSystem", "Note",
                "SystemItemCustomDetail", "Organization", "SystemItemVersionDelta", "SystemEnumerationItem",
                "SystemEnumerationItemMap", "MappingProjectSynonym", "CustomMigration", "CustomDetailMetadata",
            };

            foreach (var model in models)
            {
                var createUserQuery = @"
                    UPDATE model
                    SET model.CreateById = u.Id
                    FROM ReplaceModel model
                    JOIN AspNetUsers u on u.UserName = CreateBy
                    WHERE model.CreateBy != ''
                ";

                var updateUserQuery = @"
                    UPDATE model
                    SET model.UpdateById = u.Id
                    FROM ReplaceModel model
                    JOIN AspNetUsers u on u.UserName = UpdateBy
                    WHERE model.UpdateBy != ''
                ";

                createUserQuery = createUserQuery.Replace("ReplaceModel", model);
                updateUserQuery = updateUserQuery.Replace("ReplaceModel", model);

                Sql(createUserQuery);
                Sql(updateUserQuery);
            }

            Sql(File.ReadAllText(_alterGetReviewQueuePage));
            Sql(File.ReadAllText(_alterGetMappingProjectItemMaps));

            Sql(@"UPDATE sim
                  SET sim.IsAutoMapped = 1
                  FROM SystemItemMap sim
                  JOIN MappingProject mp on mp.MappingProjectId = sim.MappingProjectId
                  WHERE sim.CreateDate >= mp.CreateDate AND sim.CreateDate <= DATEADD(MINUTE, 5, mp.CreateDate)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomMigration", "UpdateById");
            DropColumn("dbo.CustomMigration", "CreateById");
            DropColumn("dbo.MappingProjectSynonym", "UpdateById");
            DropColumn("dbo.MappingProjectSynonym", "CreateById");
            DropColumn("dbo.SystemEnumerationItemMap", "UpdateById");
            DropColumn("dbo.SystemEnumerationItemMap", "CreateById");
            DropColumn("dbo.SystemEnumerationItem", "UpdateById");
            DropColumn("dbo.SystemEnumerationItem", "CreateById");
            DropColumn("dbo.Note", "UpdateById");
            DropColumn("dbo.Note", "CreateById");
            DropColumn("dbo.SystemItemVersionDelta", "UpdateById");
            DropColumn("dbo.SystemItemVersionDelta", "CreateById");
            DropColumn("dbo.Organization", "UpdateById");
            DropColumn("dbo.Organization", "CreateById");
            DropColumn("dbo.SystemItemCustomDetail", "UpdateById");
            DropColumn("dbo.SystemItemCustomDetail", "CreateById");
            DropColumn("dbo.CustomDetailMetadata", "UpdateById");
            DropColumn("dbo.CustomDetailMetadata", "CreateById");
            DropColumn("dbo.MappedSystem", "UpdateById");
            DropColumn("dbo.MappedSystem", "CreateById");
            DropColumn("dbo.SystemItem", "UpdateById");
            DropColumn("dbo.SystemItem", "CreateById");
            DropColumn("dbo.EntityHint", "UpdateById");
            DropColumn("dbo.EntityHint", "CreateById");
            DropColumn("dbo.MappingProject", "UpdateById");
            DropColumn("dbo.MappingProject", "CreateById");
            DropColumn("dbo.MapNote", "UpdateById");
            DropColumn("dbo.MapNote", "CreateById");
            DropColumn("dbo.SystemItemMap", "UpdateById");
            DropColumn("dbo.SystemItemMap", "CreateById");
            DropColumn("dbo.SystemItemMap", "IsAutoMapped");

            Sql(File.ReadAllText(_revertGetReviewQueuePage));
            Sql(File.ReadAllText(_revertGetMappingProjectItemMaps));
        }
    }
}
