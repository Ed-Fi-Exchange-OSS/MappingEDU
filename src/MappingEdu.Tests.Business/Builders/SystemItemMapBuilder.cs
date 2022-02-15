// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain;
using MappingEdu.Core.Domain.System;
using MappingMethodType = MappingEdu.Core.Domain.Enumerations.MappingMethodType;
using WorkflowStatusType = MappingEdu.Core.Domain.Enumerations.WorkflowStatusType;

namespace MappingEdu.Tests.Business.Builders
{
    public class SystemItemMapBuilder
    {
        private readonly SystemItemMap _systemItemMap;

        public SystemItemMapBuilder(SystemItemMap systemItemMap)
        {
            _systemItemMap = systemItemMap;
        }

        public SystemItemMapBuilder()
        {
            _systemItemMap = BuildDefault();
        }

        private static SystemItemMap BuildDefault()
        {
            return new SystemItemMap {SystemItemMapId = Guid.NewGuid()};
        }

        public SystemItemMapBuilder WithId(Guid id)
        {
            _systemItemMap.SystemItemMapId = id;
            return this;
        }

        public SystemItemMapBuilder WithMappingProject(MappingProject mappingProject)
        {
            _systemItemMap.MappingProject = mappingProject;
            _systemItemMap.MappingProjectId = mappingProject.MappingProjectId;
            return this;
        }

        public SystemItemMapBuilder WithSourceSystemItem(SystemItem systemItem)
        {
            _systemItemMap.SourceSystemItem = systemItem;
            _systemItemMap.SourceSystemItemId = systemItem.SystemItemId;
            systemItem.SourceSystemItemMaps.Add(_systemItemMap);
            return this;
        }

        public SystemItemMapBuilder WithTargetSystemItem(SystemItem systemItem)
        {
            _systemItemMap.TargetSystemItems.Add(systemItem);
            systemItem.TargetSystemItemMaps.Add(_systemItemMap);
            return this;
        }

        public SystemItemMapBuilder WithBusinessLogic(string businessLogic)
        {
            _systemItemMap.BusinessLogic = businessLogic;
            return this;
        }

        public SystemItemMapBuilder WithDeferredMapping(bool deferredMapping, string omissionReason)
        {
            _systemItemMap.DeferredMapping = deferredMapping;
            _systemItemMap.OmissionReason = omissionReason;
            return this;
        }

        public SystemItemMapBuilder WithMappingStatusType(int mappingStatusTypeId)
        {
            _systemItemMap.MappingStatusTypeId = mappingStatusTypeId;
            return this;
        }

        public SystemItemMapBuilder WithCompleteStatusType(int completeStatusTypeId)
        {
            _systemItemMap.CompleteStatusTypeId = completeStatusTypeId;
            return this;
        }

        public SystemItemMapBuilder WithStatusReasonType(int statusReasonTypeId)
        {
            _systemItemMap.MappingStatusReasonTypeId = statusReasonTypeId;
            return this;
        }

        public SystemItemMapBuilder WithExcludeInExternalReports(bool excludeInExternalReports)
        {
            _systemItemMap.ExcludeInExternalReports = excludeInExternalReports;
            return this;
        }

        public SystemItemMapBuilder WithWorkflowStatus(WorkflowStatusType workflowStatusType)
        {
            _systemItemMap.WorkflowStatusType = workflowStatusType;
            return this;
        }

        public SystemItemMapBuilder WithMappingMethod(MappingMethodType mappingMethodType)
        {
            _systemItemMap.MappingMethodType = mappingMethodType;
            return this;
        }

        public static implicit operator SystemItemMap(SystemItemMapBuilder builder)
        {
            return builder._systemItemMap;
        }
    }
}