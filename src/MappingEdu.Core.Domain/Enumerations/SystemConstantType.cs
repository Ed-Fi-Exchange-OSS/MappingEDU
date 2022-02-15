// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    public class SystemConstantType : DatabaseEnumeration<SystemConstantType, int>
    {
        public static readonly SystemConstantType Text = new SystemConstantType(1, "Text");
        public static readonly SystemConstantType ComplexText = new SystemConstantType(2, "ComplexText");
        public static readonly SystemConstantType Boolean = new SystemConstantType(3, "Boolean");
        public static readonly SystemConstantType TextArea = new SystemConstantType(4, "TextArea");

        public static readonly SystemConstantType Unknown = new SystemConstantType(0, "Unknown", false, true);

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

        private SystemConstantType(int id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}