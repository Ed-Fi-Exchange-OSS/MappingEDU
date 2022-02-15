// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for Mapping Status
    /// </summary>
    public class MappingStatusType : DatabaseEnumeration<MappingStatusType, int?>
    {
        public static readonly MappingStatusType ApprovedForInclusion = new MappingStatusType(1, "Approved for Inclusion");
        public static readonly MappingStatusType ApprovedForOmission = new MappingStatusType(2, "Approved for Omission");
        public static readonly MappingStatusType ExcludingSnapshot = new MappingStatusType(3, "Excluding Snapshot");
        public static readonly MappingStatusType NeedFurtherReview = new MappingStatusType(4, "Need Further Review");
        public static readonly MappingStatusType OnHold = new MappingStatusType(5, "On Hold");
        public static readonly MappingStatusType OutOfScope = new MappingStatusType(6, "Out of Scope");
        public static readonly MappingStatusType ProposedForInclusion = new MappingStatusType(7, "Proposed for Inclusion");
        public static readonly MappingStatusType ProposedForOmission = new MappingStatusType(8, "Proposed for Omission");
        public static readonly MappingStatusType ApprovedDWHExtension = new MappingStatusType(9, "Approved DWH Extension");
        public static readonly MappingStatusType ProposedDWHExtension = new MappingStatusType(10, "Proposed DWH Extension");
        public static readonly MappingStatusType Unknown = new MappingStatusType(null, string.Empty, false, true);

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

        private MappingStatusType(int? id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}