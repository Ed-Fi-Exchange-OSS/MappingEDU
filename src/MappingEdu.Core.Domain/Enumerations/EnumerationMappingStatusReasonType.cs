// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for Enumeration Mapping Status Reason
    /// </summary>
    public class EnumerationMappingStatusReasonType : DatabaseEnumeration<EnumerationMappingStatusReasonType, int?>
    {
        public static readonly EnumerationMappingStatusReasonType NonTypeElement = new EnumerationMappingStatusReasonType(1, "Aligns with Core Non-Type Element");
        public static readonly EnumerationMappingStatusReasonType Boolean = new EnumerationMappingStatusReasonType(2, "Boolean");
        public static readonly EnumerationMappingStatusReasonType DerivedValue = new EnumerationMappingStatusReasonType(3, "Derived Value");
        public static readonly EnumerationMappingStatusReasonType NotInSystem = new EnumerationMappingStatusReasonType(4, "n/a in System");
        public static readonly EnumerationMappingStatusReasonType Unknown = new EnumerationMappingStatusReasonType(null, string.Empty, false, true);

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

        private EnumerationMappingStatusReasonType(int? id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}