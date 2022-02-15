// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MappingEdu.Service.Model.Datatables;

namespace MappingEdu.Service.Model.ElementDetails
{
    public class ElementDetailsSearchModel
    {
        public string DataTypeSource { get; set; }

        public string Definition { get; set; }

        public Guid DomainId { get; set; }

        public string DomainItemPath { get; set; }

        public int? FieldLength { get; set; }

        public bool IsExtended { get; set; }

        public int? ItemDataTypeId { get; set; }

        public string ItemName { get; set; }

        public string ItemUrl { get; set; }

        public int ItemTypeId { get; set; }

        public Guid SystemItemId { get; set; }

        public string TechnicalName { get; set; }
        
    }

    public class ElementDetailsSuggestModel : ElementDetailsSearchModel
    {
        public string CompareDefinition { get; set; }

        public string CompareDomainItemPath { get; set; }

        public string CompareItemName { get; set; }

    }

    public class SystemItemMapFlatModel
    {
        public Guid SystemItemMapId { get; set; }

        public Guid SourceSystemItemId { get; set; }

        public string BusinessLogic { get; set; }

        public int MappingMethodTypeId { get; set; }

        public string OmissionReason { get; set; }

        public int WorkflowStatusTypeId { get; set; }

        public bool? Flagged { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateBy { get; set; }

        public Guid? TargetSystemItem_MappedSystemId { get; set; }

        public Guid? TargetSystemItem_SystemItemId { get; set; }

        public int? TargetSystemItem_ItemTypeId { get; set; }
    }

    public class SystemItemSearchDatatablesModel : DatatablesModel
    {
        public Guid[] ElementGroups { get; set; }
        public Guid[] Entities { get; set; }
        public int[] ItemTypes { get; set; }
        public int[] ItemDataTypes { get; set; }
        public bool? IsExtended { get; set; }
    }
}