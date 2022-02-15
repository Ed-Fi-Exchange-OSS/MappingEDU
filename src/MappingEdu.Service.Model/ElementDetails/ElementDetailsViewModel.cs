// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.ElementList;

namespace MappingEdu.Service.Model.ElementDetails
{
    public class ElementDetailsViewModel
    {
        public string DataTypeSource { get; set; }

        public string Definition { get; set; }

        public string DomainItemPath { get; set; }

        public string EnumerationSystemItemDefinition { get; set; }

        public Guid? EnumerationSystemItemId { get; set; }

        public string EnumerationSystemItemName { get; set; }

        public int? FieldLength { get; set; }

        public string FullItemPath { get; set; }

        public bool IsExtended { get; set; }

        public int? ItemDataTypeId { get; set; }

        public ItemDataType ItemDataType { get; set; }

        public string ItemName { get; set; }

        public int ItemTypeId { get; set; }

        public ItemType ItemType { get; set; }

        public string ItemUrl { get; set; }

        public Guid? ParentSystemItemId { get; set; }

        public ElementListViewModel.ElementPathViewModel.PathSegment[] PathSegments { get; set; }

        public Guid SystemItemId { get; set; }

        public string TechnicalName { get; set; }
    }

    public class ElementDetailsSearchViewModel : ElementDetailsViewModel
    {
        public Guid DomainId { get; set; }

        public string DomainName { get; set; }

        public string ShortItemPath { get; set; }

        public string SpacedItemPath { get; set; }
    }

    public class ElementDetailsSuggestViewModel : ElementDetailsSearchViewModel
    {
        public string PreviousBusinessLogic { get; set; }

        public MappingMethodType PreviousMappingMethod { get; set; }

        public string PreviousOmissionReason { get; set; }

        public decimal SuggestRank { get; set; }

        public string SuggestReason { get; set; }
    }
}