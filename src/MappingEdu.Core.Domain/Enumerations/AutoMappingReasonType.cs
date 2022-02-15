// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for System Item Data Type
    /// </summary>
    public class AutoMappingReasonType : DatabaseEnumeration<AutoMappingReasonType, int?>
    {
        public static readonly AutoMappingReasonType SamePath = new AutoMappingReasonType(1, "Identical path.");
        public static readonly AutoMappingReasonType SimilarMappingProject = new AutoMappingReasonType(2, "Similar Mapping Project.");
        public static readonly AutoMappingReasonType PreviousVersionDelta = new AutoMappingReasonType(3, "Previous version delta.");
        public static readonly AutoMappingReasonType PreviousSourceVersion = new AutoMappingReasonType(4, "Previous source version.");
        public static readonly AutoMappingReasonType PreviousTargetVersion = new AutoMappingReasonType(5, "Previous target version.");
        public static readonly AutoMappingReasonType PreviousPrevious = new AutoMappingReasonType(6, "Previous source previous target version.");
        public static readonly AutoMappingReasonType Transitive = new AutoMappingReasonType(7, "Transitive mapping found.");
        public static readonly AutoMappingReasonType Unknown = new AutoMappingReasonType(null, string.Empty, false, true);

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

        private AutoMappingReasonType(int? id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}