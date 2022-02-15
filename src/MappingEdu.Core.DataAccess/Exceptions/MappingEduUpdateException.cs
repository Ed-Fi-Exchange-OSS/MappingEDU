// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace MappingEdu.Core.DataAccess.Exceptions
{
    [Serializable]
    public class MappingEduUpdateException : DbUpdateException
    {
        public IEnumerable<DbEntityEntry> DbEntityEntries { get; set; }

        public override string Message
        {
            get
            {
                var newMessage = new StringBuilder();
                Exception lastException = this;
                while (lastException.InnerException != null)
                {
                    lastException = lastException.InnerException;
                }
                newMessage.AppendLine(lastException.Message);

                foreach (var dbEntityEntry in DbEntityEntries)
                {
                    if (newMessage.Length > 0)
                        newMessage.AppendLine();

                    var entityType = dbEntityEntry.Entity.GetType();
                    var entityTypeName = entityType.Name;
                    if (entityTypeName.Contains("_") && entityType.BaseType != null)
                        entityTypeName = entityType.BaseType.Name;

                    newMessage.AppendFormat("Entity of type '{0}' in state '{1}' could not be saved to the database.", entityTypeName, dbEntityEntry.State);
                    newMessage.AppendLine();
                }
                return newMessage.ToString();
            }
        }

        public MappingEduUpdateException(DbUpdateException exception) : base(exception.Message, exception)
        {
            DbEntityEntries = exception.Entries;
        }
    }
}