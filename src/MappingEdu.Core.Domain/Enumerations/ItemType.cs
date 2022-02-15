// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for Item Type
    /// </summary>
    public class ItemType : DatabaseEnumeration<ItemType, int>
    {
        public static readonly ItemType Domain = new ItemType(1, "Domain");
        public static readonly ItemType Entity = new ItemType(2, "Entity");
        public static readonly ItemType SubEntity = new ItemType(3, "SubEntity");
        public static readonly ItemType Element = new ItemType(4, "Element");
        public static readonly ItemType Enumeration = new ItemType(5, "Enumeration");
        public static readonly ItemType Unknown = new ItemType(0, "Unknown", false, true);

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

        private ItemType(int id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}