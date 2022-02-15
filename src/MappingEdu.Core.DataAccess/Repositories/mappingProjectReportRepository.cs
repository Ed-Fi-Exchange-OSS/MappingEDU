// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Repositories;
using MappingEdu.Service.Model.MappingProject;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class MappingProjectReportRepository : Repository<MappingProject>, IMappingProjectReportRepository
    {
        public MappingProjectReportRepository(EntityContext databaseContext) : base(databaseContext)
        {
        }

        public List<dynamic> GetElementList(Guid dataStandardId)
        {
            var rows = new List<dynamic>();
            _databaseContext.Database.Connection.Open();
            var cmd = _databaseContext.Database.Connection.CreateCommand();

            cmd.CommandText = "[MappingProjectReportElementList] @DataStandardId";
            cmd.Parameters.Add(new SqlParameter {ParameterName = "DataStandardId", Value = dataStandardId});

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
                        ((IDictionary<string, object>) row).Add(columnNames[i], reader[i].ToString());
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

                cmd.CommandText = "[MappingProjectReportEnumerationItems] @DataStandardId";
                cmd.Parameters.Add(new SqlParameter {ParameterName = "DataStandardId", Value = dataStandardId});

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
                            ((IDictionary<string, object>) row).Add(columnNames[i], reader[i].ToString());
                        rows.Add(row);
                    }
                }
                _databaseContext.Database.Connection.Close();
                return rows;
            }
        }

        public List<dynamic> GetSourceEnumerationItemMaps(Guid mappingProjectId, ICollection<int> enumerationStatuses, ICollection<int> enumerationStatusReasons, bool includeCustomDetails)
        {
            {
                var rows = new List<dynamic>();
                _databaseContext.Database.Connection.Open();
                var cmd = _databaseContext.Database.Connection.CreateCommand();

                cmd.CommandText = "[MappingProjectReportSourceEnumerationItemMappings] @MappingProjectId, @EnumerationStatuses, @EnumerationStatusReasons, @IncludeCustomDetails";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingProjectId", Value = mappingProjectId});
                cmd.Parameters.Add(new SqlParameter { ParameterName = "EnumerationStatuses", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(enumerationStatuses), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "EnumerationStatusReasons", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(enumerationStatusReasons), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "IncludeCustomDetails", Value = includeCustomDetails});

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
                            ((IDictionary<string, object>) row).Add(columnNames[i], reader[i].ToString());
                        rows.Add(row);
                    }
                }
                _databaseContext.Database.Connection.Close();
                return rows;
            }
        }

        public List<dynamic> GetSourceMappings(Guid mappingProjectId, ICollection<int> mappingMethods, ICollection<int> workflowStatuses, bool includeCustomDetails, bool includeTargetItems)
        {
            var rows = new List<dynamic>();
            _databaseContext.Database.Connection.Open();
            var cmd = _databaseContext.Database.Connection.CreateCommand();

            cmd.CommandText = "[MappingProjectReportSourceElementMappings] @MappingProjectId, @MappingMethods, @WorkflowStatuses, @IncludeCustomDetails";
            cmd.Parameters.Add(new SqlParameter {ParameterName = "MappingProjectId", Value = mappingProjectId});
            cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingMethods", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(mappingMethods), TypeName = "dbo.IntId" });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "WorkflowStatuses", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(workflowStatuses), TypeName = "dbo.IntId" });
            cmd.Parameters.Add(new SqlParameter {ParameterName = "IncludeCustomDetails", Value = includeCustomDetails});

            using (var reader = cmd.ExecuteReader())
            {
                // Get the column names
                var columnNames = new string[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    columnNames[i] = reader.GetName(i);
                }

                dynamic row = new ExpandoObject();
                // Get the results
                var rowIndex = 0;
                while (reader.Read())
                {
                    dynamic result = new ExpandoObject();
                    result.TargetSystemItems = new List<dynamic>();
                    dynamic targetItem = new ExpandoObject();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader[i].ToString();
                        if ((columnNames[i] == "UpdateDate" || columnNames[i] == "CreateDate") && value != "")
                            ((IDictionary<string, object>) result).Add(columnNames[i], (DateTime) reader[i]);
                        else if (columnNames[i] == "TargetItemName")
                            targetItem.ItemName = value;
                        else if (columnNames[i] == "TargetDomainItemPath")
                            targetItem.DomainItemPath = value;
                        else
                            ((IDictionary<string, object>) result).Add(columnNames[i], value);
                    }

                    if (rowIndex != 0 && row.DomainItemPath == result.DomainItemPath)
                    {
                        if(includeTargetItems) row.TargetSystemItems.Add(targetItem);
                    }
                    else
                    {
                        if (rowIndex != 0) rows.Add(row);

                        if (targetItem.DomainItemPath != "" && includeTargetItems)
                            result.TargetSystemItems.Add(targetItem);

                        row = result;
                    }
                    rowIndex++;
                }
                rows.Add(row);
            }
            _databaseContext.Database.Connection.Close();
            return rows;
        }

        public List<dynamic> GetTargetEnumerationItemMaps(Guid mappingProjectId, ICollection<int> enumerationStatuses, ICollection<int> enumerationStatusReasons, bool includeCustomDetails)
        {
            {
                var rows = new List<dynamic>();
                _databaseContext.Database.Connection.Open();
                var cmd = _databaseContext.Database.Connection.CreateCommand();

                cmd.CommandText = "[MappingProjectReportTargetEnumerationItemMappings] @MappingProjectId, @EnumerationStatuses, @EnumerationStatusReasons, @IncludeCustomDetails";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingProjectId", Value = mappingProjectId });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "EnumerationStatuses", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(enumerationStatuses), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "EnumerationStatusReasons", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(enumerationStatusReasons), TypeName = "dbo.IntId" });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "IncludeCustomDetails", Value = includeCustomDetails });

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

        public List<dynamic> GetTargetMappings(Guid mappingProjectId, ICollection<int> mappingMethods, ICollection<int> workflowStatuses, bool includeCustomDetails)
        {
            var rows = new List<dynamic>();
            _databaseContext.Database.Connection.Open();
            var cmd = _databaseContext.Database.Connection.CreateCommand();

            cmd.CommandText = "[MappingProjectReportTargetElementMappings] @MappingProjectId, @MappingMethods, @WorkflowStatuses, @IncludeCustomDetails";
            cmd.Parameters.Add(new SqlParameter {ParameterName = "MappingProjectId", Value = mappingProjectId});
            cmd.Parameters.Add(new SqlParameter { ParameterName = "MappingMethods", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(mappingMethods), TypeName = "dbo.IntId" });
            cmd.Parameters.Add(new SqlParameter { ParameterName = "WorkflowStatuses", SqlDbType = SqlDbType.Structured, Value = CreateDataTable(workflowStatuses), TypeName = "dbo.IntId" });
            cmd.Parameters.Add(new SqlParameter {ParameterName = "IncludeCustomDetails", Value = includeCustomDetails});

            using (var reader = cmd.ExecuteReader())
            {
                // Get the column names
                var columnNames = new string[reader.FieldCount];
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    columnNames[i] = reader.GetName(i);
                }

                dynamic row = new ExpandoObject();
                // Get the results
                var rowIndex = 0;
                while (reader.Read())
                {
                    dynamic result = new ExpandoObject();
                    result.SourceMappings = new List<dynamic>();
                    dynamic sourceMapping = new ExpandoObject();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader[i].ToString();
                        if ((columnNames[i] == "UpdateDate" || columnNames[i] == "CreateDate") && value != "")
                            ((IDictionary<string, object>) sourceMapping).Add(columnNames[i], (DateTime) reader[i]);
                        else if (columnNames[i] == "SourceItemName")
                            sourceMapping.ItemName = value;
                        else if (columnNames[i] == "SourceDomainItemPath")
                            sourceMapping.DomainItemPath = value;
                        else if (columnNames[i] == "MappingMethodTypeId" || columnNames[i] == "WorkflowStatusTypeId" || columnNames[i] == "BusinessLogic" || columnNames[i] == "OmissionReason" || columnNames[i] == "CreatedBy" || columnNames[i] == "UpdatedBy")
                            ((IDictionary<string, object>)sourceMapping).Add(columnNames[i], value);
                        else 
                            ((IDictionary<string, object>) result).Add(columnNames[i], value);
                    }

                    if (rowIndex != 0 && row.DomainItemPath == result.DomainItemPath)
                    {
                        row.SourceMappings.Add(sourceMapping);
                    }
                    else
                    {
                        if (rowIndex != 0) rows.Add(row);

                        if (sourceMapping.DomainItemPath != "")
                            result.SourceMappings.Add(sourceMapping);

                        row = result;
                    }
                    rowIndex++;
                }
                rows.Add(row);
            }
            _databaseContext.Database.Connection.Close();
            return rows;
        }

        public MappingProjectReportEnumerationItemMap[] EnumerationItemMaps(Guid mappingProjectId)
        {
            var clientIdParameter = new SqlParameter("@MappingProjectId", mappingProjectId);
            return _databaseContext.Database.SqlQuery<MappingProjectReportEnumerationItemMap>("[GetMappingProjectReportEnumerationItemMaps] @MappingProjectId", clientIdParameter).ToArray();
        }

        private DataTable CreateDataTable<T>(ICollection<T> ids)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(T));

            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var row = dataTable.NewRow();
                    row.SetField("Id", id);
                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }
    }
}