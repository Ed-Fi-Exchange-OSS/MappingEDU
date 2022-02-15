// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for Enumeration Mapping Status
    /// </summary>
    public class EnumerationMappingStatusType : DatabaseEnumeration<EnumerationMappingStatusType, int?>
    {
        public static readonly EnumerationMappingStatusType AcceptedMap = new EnumerationMappingStatusType(1, "Accepted: Core Type/DescriptorMap");
        public static readonly EnumerationMappingStatusType ApprovedExtension = new EnumerationMappingStatusType(2, "Approved Core Type/DescriptorMap Extension");
        public static readonly EnumerationMappingStatusType ApprovedEnumeration = new EnumerationMappingStatusType(3, "Approved Enumeration");
        public static readonly EnumerationMappingStatusType ApprovedExtensionDescriptorEnumeration = new EnumerationMappingStatusType(4, "Approved extension. Descriptor Enumeration");
        public static readonly EnumerationMappingStatusType Ignored = new EnumerationMappingStatusType(5, "Ignored");
        public static readonly EnumerationMappingStatusType Mapped = new EnumerationMappingStatusType(6, "Mapped: Core Type/DescriptorMap");
        public static readonly EnumerationMappingStatusType MapsToDescriptor = new EnumerationMappingStatusType(7, "Maps to Descriptor");
        public static readonly EnumerationMappingStatusType NotImplementingAsEnumeration = new EnumerationMappingStatusType(8, "Not Implementing As Enumeration");
        public static readonly EnumerationMappingStatusType Omitted = new EnumerationMappingStatusType(9, "Omitted");
        public static readonly EnumerationMappingStatusType ProposedExtension = new EnumerationMappingStatusType(10, "Proposed Core Type/DescriptorMap Extension");
        public static readonly EnumerationMappingStatusType ProposedEnumeration = new EnumerationMappingStatusType(11, "Proposed Enumeration");
        public static readonly EnumerationMappingStatusType ProposedExtensionDescriptorEnumeration = new EnumerationMappingStatusType(12, "Proposed extension. Descriptor Enumeration");
        public static readonly EnumerationMappingStatusType Unknown = new EnumerationMappingStatusType(null, string.Empty, false, true);

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

        private EnumerationMappingStatusType(int? id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}