// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    public class CompleteStatusType : DatabaseEnumeration<CompleteStatusType, int?>
    {
        public static readonly CompleteStatusType Incomplete = new CompleteStatusType(1, "Incomplete");
        public static readonly CompleteStatusType Complete = new CompleteStatusType(2, "Complete");
        public static readonly CompleteStatusType Unknown = new CompleteStatusType(null, string.Empty, false, true);

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

        private CompleteStatusType(int? id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}