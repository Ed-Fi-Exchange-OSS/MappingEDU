// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    public class MappingMethodType : DatabaseEnumeration<MappingMethodType, int>
    {
        public static readonly MappingMethodType EnterMappingBusinessLogic = new MappingMethodType(1, "Enter Mapping Business Logic");
        public static readonly MappingMethodType MarkForInclusion = new MappingMethodType(2, "Mark for Inclusion");
        public static readonly MappingMethodType MarkForExtension = new MappingMethodType(3, "Mark for Extension");
        public static readonly MappingMethodType MarkForOmission = new MappingMethodType(4, "Mark for Omission");
        public static readonly MappingMethodType Unknown = new MappingMethodType(0, "Unknown", false, true);

        private readonly int _id;
        private readonly bool _isInDatabase;
        private readonly string _name;

        public override int Id
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

        private MappingMethodType(int id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}