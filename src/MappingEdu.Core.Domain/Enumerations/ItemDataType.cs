// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for System Item Data Type
    /// </summary>
    public class ItemDataType : DatabaseEnumeration<ItemDataType, int?>
    {
        public static readonly ItemDataType Unknown = new ItemDataType(null, string.Empty, false, true);
        public static readonly ItemDataType Boolean = new ItemDataType(1, "Boolean");
        public static readonly ItemDataType Byte = new ItemDataType(2, "Byte");
        public static readonly ItemDataType Char = new ItemDataType(3, "Char");
        public static readonly ItemDataType Currency = new ItemDataType(4, "Currency");
        public static readonly ItemDataType Date = new ItemDataType(5, "Date");
        public static readonly ItemDataType Datetime = new ItemDataType(6, "DateTime");
        public static readonly ItemDataType Decimal = new ItemDataType(7, "Decimal");
        public static readonly ItemDataType Double = new ItemDataType(8, "Double");
        public static readonly ItemDataType Duration = new ItemDataType(9, "Duration");
        public static readonly ItemDataType Integer = new ItemDataType(10, "Integer");
        public static readonly ItemDataType Long = new ItemDataType(11, "Long");
        public static readonly ItemDataType Short = new ItemDataType(12, "Short");
        public static readonly ItemDataType String = new ItemDataType(13, "String");
        public static readonly ItemDataType Time = new ItemDataType(14, "Time");
        public static readonly ItemDataType UniqueId = new ItemDataType(15, "UniqueId");
        public static readonly ItemDataType Year = new ItemDataType(16, "Year");
        public static readonly ItemDataType Enumeration = new ItemDataType(17, "Enumeration");

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

        private ItemDataType(int? id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}