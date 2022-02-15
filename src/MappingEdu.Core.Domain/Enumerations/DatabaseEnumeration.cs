// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#region

using System.Linq;

#endregion

namespace MappingEdu.Core.Domain.Enumerations
{
    public interface IDatabaseEnumeration
    {
        object DatabaseId { get; }

        string DatabaseText { get; }
    }

    public abstract class DatabaseEnumeration<TEnum, TId> : Enumeration<TEnum, TId>, IDatabaseEnumeration, IDisplayEnumeration<TId> where TEnum : DatabaseEnumeration<TEnum, TId>
    {
        protected abstract bool IsInDatabase { get; }

        public abstract string Name { get; }

        protected DatabaseEnumeration() : this(false)
        {
        }

        protected DatabaseEnumeration(bool isDefault) : base(isDefault)
        {
        }

        public string DatabaseText
        {
            get { return Name; }
        }

        public object DatabaseId
        {
            get { return Id; }
        }

        public string DisplayText
        {
            get { return Name; }
        }

        public static TEnum[] GetDatabaseValues()
        {
            return GetValues()
                .Where(x => x.IsInDatabase)
                .ToArray();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}