// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Repositories;

namespace MappingEdu.Core.DataAccess.Repositories
{
    public class LoggingRepository: ILoggingRepository
    {
        public EntityContext _databaseContext; //TODO: FIx me

        public LoggingRepository(EntityContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public virtual Log[] GetAll()
        {
            var logs = _databaseContext.Set<Log>();
            return logs.ToArray();
        }

        public IQueryable<Log> GetAllQueryable()
        {
            return _databaseContext.Set<Log>();
        }

        public void DeleteRange(IEnumerable<Log> logs)
        {
            _databaseContext.Set<Log>().RemoveRange(logs);
        }

        public void SaveChanges()
        {
            _databaseContext.SaveChanges();
        }
    }
}