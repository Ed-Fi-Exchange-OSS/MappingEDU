// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Core.Domain.Enumerations;

namespace MappingEdu.Service.Model.MappingProject
{
    public class MappingProjectReportsViewModel
    {
        public decimal PercentComplete { get; set; }
    }

    public class MappingProjectCreateReportModel
    {
        public MappingProjectReportBasePage ElementList { get; set; }
        public MappingProjectReportBasePage EnumerationList { get; set; }
        public ElementMappingListPage ElementMappingList { get; set; }
        public EnumerationMappingListPage EnumrationMappingList { get; set; }
    }

    public class MappingProjectReportBasePage
    {
        public ICollection<ReportColumns> Columns { get; set; }
        public bool Show { get; set; }
    }

    public class ReportColumns
    {
        public string Column { get; set; }
        public string DisplayText { get; set; }
        public bool IsCustomDetail { get; set; }
    }

    public class ElementMappingListPage : MappingProjectReportBasePage
    {
        public ICollection<int> MappingMethods { get; set; }
        public ICollection<int> WorkflowStatuses { get; set; }
    }

    public class EnumerationMappingListPage : MappingProjectReportBasePage
    {
        public ICollection<int> EnumerationMappingStatusTypes { get; set; }
        public ICollection<int> EnumerationMappingStatusReasonTypes { get; set; }
    }

    public class MappingProjectReportEnumerationItemMap
    {
        public Guid SourceSystemItemId { get; set; }
        public Guid SourceSystemEnumerationItemId { get; set; }
        public string SourceCodeValue { get; set; }
        public string SourceShortDescription { get; set; }
        public string SourceDescription { get; set; }
        public int? EnumerationMappingStatusTypeId { get; set; }
        public int? EnumerationMappingStatusReasonTypeId { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Guid? TargetSystemEnumerationItemId { get; set; }
        public string TargetCodeValue { get; set; }
        public string TargetShortDescription { get; set; }
        public string TargetDescription { get; set; }
        public Guid? TargetSystemItemId { get; set; }
    }

    public class MappingProjectReportEnumerationItem
    {
        public string ElementGroup { get; set; } 
        public string ItemName { get; set; }
        public string CodeValue { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
    }
}