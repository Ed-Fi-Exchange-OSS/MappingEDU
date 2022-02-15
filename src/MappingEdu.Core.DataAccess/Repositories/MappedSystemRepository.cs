// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.Security;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public interface IMappedSystemRepository : IRepository<MappedSystem>
    {
        List<dynamic> GetExtensionReportDetail(Guid dataStandardId, Guid? parentSystemItemId);
    }

    public class MappedSystemRepository : Repository<MappedSystem>, IMappedSystemRepository
    {
        public MappedSystemRepository(EntityContext databaseContext) : base(databaseContext)
        {
        }

        public override MappedSystem[] GetAll()
        {
            var entities = _databaseContext.MappedSystems.Where(x => x.IsActive);

            if (!Principal.Current.IsAdministrator)
            {
                entities = entities.Where(x => x.Users.Any(m => m.UserId == Principal.Current.UserId) || x.IsPublic);
            }

            return entities.ToArray();
        }

        public List<dynamic> GetExtensionReportDetail(Guid dataStandardId, Guid? parentSystemItemId)
        {
            {
                var rows = new List<dynamic>();
                _databaseContext.Database.Connection.Open();
                var cmd = _databaseContext.Database.Connection.CreateCommand();

                cmd.CommandText = "[dbo].[ExtensionReport] @DataStandardId";
                if (parentSystemItemId.HasValue) cmd.CommandText += ", @ParentSystemItemId";

                cmd.Parameters.Add(new SqlParameter { ParameterName = "DataStandardId", Value = dataStandardId });
                if (parentSystemItemId.HasValue) cmd.Parameters.Add(new SqlParameter { ParameterName = "ParentSystemItemId", Value = parentSystemItemId.Value });

                using (var reader = cmd.ExecuteReader())
                {
                    // Get the column names
                    var columnNames = new string[reader.FieldCount];
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames[i] = reader.GetName(i);
                    }

                    while (reader.Read())
                    {
                        dynamic row = new ExpandoObject();
                        row.Total = 0;

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader[i].ToString();
                            if (columnNames[i] == "ItemName" || columnNames[i] == "SystemItemId" || columnNames[i] == "ItemTypeId" ||
                                columnNames[i] == "ShortName" || columnNames[i] == "MappedSystemExtensionId")
                            {
                                ((IDictionary<string, object>)row).Add(columnNames[i], value);
                            }
                            else
                            {
                                int extensionsTotal;
                                if (int.TryParse(value, out extensionsTotal))
                                {
                                    ((IDictionary<string, object>) row).Add(columnNames[i], extensionsTotal);
                                    row.Total += extensionsTotal;
                                }
                                else
                                {
                                    ((IDictionary<string, object>)row).Add(columnNames[i], 0);
                                }
                            }
                        }
                        rows.Add(row);
                    }
                }
                _databaseContext.Database.Connection.Close();
                return rows;
            }
        }
    }
}