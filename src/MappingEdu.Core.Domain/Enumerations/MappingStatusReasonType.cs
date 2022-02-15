// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for Mapping Status Reason
    /// </summary>
    public class MappingStatusReasonType : DatabaseEnumeration<MappingStatusReasonType, int?>
    {
        public static readonly MappingStatusReasonType AdultEducation = new MappingStatusReasonType(1, "Adult Education");
        public static readonly MappingStatusReasonType ApplicationEvaluationEligibility = new MappingStatusReasonType(2, "Application/Evaluation/Eligibility");
        public static readonly MappingStatusReasonType AssessmentFormDesign = new MappingStatusReasonType(3, "Assessment Form Design");
        public static readonly MappingStatusReasonType AssessmentRuntime = new MappingStatusReasonType(4, "Assessment Runtime");
        public static readonly MappingStatusReasonType ChangeTransactionInfo = new MappingStatusReasonType(5, "Change/Transaction Info");
        public static readonly MappingStatusReasonType DataWarehouseRequirement = new MappingStatusReasonType(6, "Data Warehouse Requirement");
        public static readonly MappingStatusReasonType DefinitionPurposeUnclear = new MappingStatusReasonType(7, "Definition/Purpose Unclear");
        public static readonly MappingStatusReasonType EdFacts = new MappingStatusReasonType(8, "EdFacts");
        public static readonly MappingStatusReasonType HealthInformation = new MappingStatusReasonType(9, "Health Information");
        public static readonly MappingStatusReasonType MappedInNonSnapshotVersion = new MappingStatusReasonType(10, "Mapped in Non-Snapshot Version");
        public static readonly MappingStatusReasonType NoCurrentK12PerformanceUseCase = new MappingStatusReasonType(11, "No current K12 performance use case");
        public static readonly MappingStatusReasonType NoKnownDataSource = new MappingStatusReasonType(12, "No known data source");
        public static readonly MappingStatusReasonType PendingUseCase = new MappingStatusReasonType(13, "Pending Use Case");
        public static readonly MappingStatusReasonType PerClientFeedback = new MappingStatusReasonType(14, "Per Client Feedback");
        public static readonly MappingStatusReasonType SlowlyChangingDimension = new MappingStatusReasonType(15, "Slowly Changing Dimension");
        public static readonly MappingStatusReasonType StudentPerformance = new MappingStatusReasonType(16, "Student Performance");
        public static readonly MappingStatusReasonType Tsdl = new MappingStatusReasonType(17, "TSDL");
        public static readonly MappingStatusReasonType Unknown = new MappingStatusReasonType(null, string.Empty, false, true);

        private readonly int? _id;
        private readonly bool _isInDatabase;
        private readonly string _name;

        public override int? Id
        {
            get { return _id; }
        }

        protected override bool IsInDatabase
        {
            get { return _isInDatabase; }
        }

        public override string Name
        {
            get { return _name; }
        }

        private MappingStatusReasonType(int? id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}