// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using MappingEdu.Core.Domain.Enumerations;
using MappingEdu.Service.Model.DataStandard;
using MappingEdu.Service.Model.ElementDetails;

namespace MappingEdu.Service.Model.AutoMap
{
    public class AutoMap
    {
        public ElementDetailsSearchModel SourceSystemItem { get; set; }
        public List<ElementDetailsSearchModel> TargetSystemItems { get; set; }
        public AutoMappingReasonType Reason { get; set; }
        public MappingMethodType MappingMethod { get; set; }
        public string BusinessLogic { get; set; }
        public string OmissionReason { get; set; }
        public string MappingProjectName { get; set; }

        //For when an Item is transitively mapped
        public ElementDetailsSearchModel CommonSystemItem { get; set; }
        public DataStandardViewModel CommonDataStandard { get; set; }
        public DataStandardViewModel PreviousSourceDataStandard { get; set; }
        public DataStandardViewModel PreviousTargetDataStandard { get; set; }
    }
}