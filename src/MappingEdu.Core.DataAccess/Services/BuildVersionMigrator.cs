// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using MappingEdu.Core.DataAccess.Entities;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Services;

namespace MappingEdu.Core.DataAccess.Services
{
    public class BuildVersionMigrator : IBuildVersionMigrator
    {
        private readonly EntityContext _context;

        public BuildVersionMigrator(EntityContext context)
        {
            _context = context;
        }

        public bool BuildVersionIsUpToDate(string buildVersion, DateTime buildDate)
        {
            var latestBuild = _context.BuildVersions.OrderByDescending(x => x.BuildDate).FirstOrDefault();

            if (latestBuild != null)
            {
                if (latestBuild.BuildVersionId == buildVersion)
                {
                    if (latestBuild.BuildDate != buildDate)
                    {
                        latestBuild.BuildDate = buildDate;
                        _context.SaveChanges();
                    }
                    return true;
                }

                if (latestBuild.BuildDate > buildDate)
                    return false;
            }

            var newBuildVersion = new BuildVersion
            {
                BuildDate = buildDate,
                BuildVersionId = buildVersion
            };

            _context.BuildVersions.Add(newBuildVersion);
            _context.SaveChanges();
            return true;
        }
    }
}