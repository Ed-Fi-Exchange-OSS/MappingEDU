// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class DataStandardExportRepository : Repository<MappingProject>, IDataStandardExportRepository
    {
        public DataStandardExportRepository(EntityContext databaseContext) : base(databaseContext)
        {
        }

        public List<dynamic> GetSystemItems(Guid dataStandardId) {
            var rows = new List<dynamic>();
            _databaseContext.Database.Connection.Open();
            var cmd = _databaseContext.Database.Connection.CreateCommand();

            cmd.CommandText = "[DataStandardExportSystemItems] @DataStandardId";
            cmd.Parameters.Add(new SqlParameter { ParameterName = "DataStandardId", Value = dataStandardId });

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
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        if (columnNames[i] == "IsExtended")
                            ((IDictionary<string, object>)row).Add(columnNames[i], reader[i].ToString() == "True");
                        else if (columnNames[i] == "ItemTypeId")
                            row.ItemTypeId = int.Parse(reader[i].ToString());
                        else
                            ((IDictionary<string, object>)row).Add(columnNames[i], reader[i].ToString());

                    }
                    rows.Add(row);
                }
            }
            _databaseContext.Database.Connection.Close();
            return rows;
        }

        public List<dynamic> GetEnumerationItems(Guid dataStandardId)
        {

            {
                var rows = new List<dynamic>();
                _databaseContext.Database.Connection.Open();
                var cmd = _databaseContext.Database.Connection.CreateCommand();

                cmd.CommandText = "[DataStandardExportEnumerationItems] @DataStandardId";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "DataStandardId", Value = dataStandardId });

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
                        for (var i = 0; i < reader.FieldCount; i++)
                            ((IDictionary<string, object>)row).Add(columnNames[i], reader[i].ToString());
                        rows.Add(row);
                    }
                }
                _databaseContext.Database.Connection.Close();
                return rows;
            }
        }
    }
}