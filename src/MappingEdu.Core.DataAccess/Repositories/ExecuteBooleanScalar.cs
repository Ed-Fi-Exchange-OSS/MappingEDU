// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data;
using System.Data.SqlClient;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class ExecuteBooleanScalar : IExecuteBooleanScalar
    {
        public bool Execute(string connectionString, string sql)
        {
            var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    var result = cmd.ExecuteScalar();
                    return result != null && result.Equals(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}