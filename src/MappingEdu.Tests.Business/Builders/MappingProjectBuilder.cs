// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;

namespace MappingEdu.Tests.Business.Builders
{
    public class MappingProjectBuilder
    {
        private readonly MappingProject _project;

        public MappingProjectBuilder(MappingProject project)
        {
            _project = project;
        }

        public MappingProjectBuilder()
        {
            _project = new MappingProject();
        }

        public static implicit operator MappingProject(MappingProjectBuilder builder)
        {
            return builder._project;
        }

        public MappingProjectBuilder WithCreatedById(Guid id)
        {
            _project.CreateById = id;
            return this;
        }

        public MappingProjectBuilder WithId(Guid id)
        {
            _project.MappingProjectId = id;
            return this;
        }

        public MappingProjectBuilder WithProjectName(string projectName)
        {
            _project.ProjectName = projectName;
            return this;
        }

        public MappingProjectBuilder WithDescription(string description)
        {
            _project.Description = description;
            return this;
        }

        public MappingProjectBuilder WithTarget(MappedSystem target)
        {
            _project.TargetDataStandard = target;
            _project.TargetDataStandardMappedSystemId = target.MappedSystemId;
            target.MappingProjectsWhereTarget.Add(this);
            return this;
        }

        public MappingProjectBuilder WithSource(MappedSystem source)
        {
            _project.SourceDataStandard = source;
            _project.SourceDataStandardMappedSystemId = source.MappedSystemId;
            source.MappingProjectsWhereSource.Add(this);
            return this;
        }

        public MappingProjectBuilder WithProjectStatusType(int projectStatusTypeId)
        {
            _project.ProjectStatusTypeId = projectStatusTypeId;
            return this;
        }

        public MappingProjectBuilder IsActive(bool isActive)
        {
            _project.IsActive = isActive;
            return this;
        }
    }
}