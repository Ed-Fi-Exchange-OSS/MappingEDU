// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MappingEdu.Core.Domain.Enumerations
{
    /// <summary>
    ///     Enumeration for System Item Change Type
    /// </summary>
    public class ItemChangeType : DatabaseEnumeration<ItemChangeType, int>
    {
        public static readonly ItemChangeType Unknown = new ItemChangeType(0, string.Empty, false, true);
        public static readonly ItemChangeType AddedDomain = new ItemChangeType(1, "Added Domain");
        public static readonly ItemChangeType AddedEntity = new ItemChangeType(2, "Added Entity");
        public static readonly ItemChangeType AddedElement = new ItemChangeType(3, "Added Element");
        public static readonly ItemChangeType ChangedEntity = new ItemChangeType(4, "Changed Entity");
        public static readonly ItemChangeType ChangedElement = new ItemChangeType(5, "Changed Element");
        public static readonly ItemChangeType DeletedEntity = new ItemChangeType(6, "Deleted Entity");
        public static readonly ItemChangeType DeletedElement = new ItemChangeType(7, "Deleted Element");

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

        private ItemChangeType(int id, string name, bool isInDatabase = true, bool isDefault = false)
            : base(isDefault)
        {
            _id = id;
            _name = name;
            _isInDatabase = isInDatabase;
        }
    }
}