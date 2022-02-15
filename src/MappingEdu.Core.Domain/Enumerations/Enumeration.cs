// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace MappingEdu.Core.Domain.Enumerations
{
    public interface IEnumeration
    {
    }

    public interface IEnumeration<TId> : IEnumeration
    {
        TId Id { get; }
    }

    public interface IDisplayEnumeration<TId> : IEnumeration<TId>
    {
        string DisplayText { get; }
    }

    public abstract class Enumeration<TEnum, TId> : IEnumeration<TId> where TEnum : Enumeration<TEnum, TId>
    {
        private static readonly object _valueLock = new object();
        private static readonly Dictionary<Type, object> _values = new Dictionary<Type, object>();
        private readonly bool _isDefault;

        protected Enumeration() : this(false)
        {
        }

        protected Enumeration(bool isDefault)
        {
            _isDefault = isDefault;
        }

        public abstract TId Id { get; }

        public static TEnumeration[] GetValues<TEnumeration>()
        {
            var type = typeof (TEnumeration);
            lock (_valueLock)
            {
                if (!_values.ContainsKey(type))
                {
                    _values[type] = type
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Where(x => x.FieldType == type)
                        .Select(x => x.GetValue(null))
                        .Cast<TEnumeration>()
                        .ToArray();
                }
            }

            return (TEnumeration[]) _values[type];
        }

        public static TEnum[] GetValues()
        {
            return GetValues<TEnum>();
        }

        public static TEnum GetById(TId id)
        {
            var values = GetValues().Where(x => Equals(id, x.Id)).ToArray();
            if (values.Length == 0)
                return null;
            if (values.Length > 1)
            {
                var message = string.Format("Found multiple values with the id {0}", id);
                throw new Exception(message);
            }
            return values[0];
        }

        public static TEnum GetDefault()
        {
            return GetValues().Single(x => x._isDefault);
        }

        public static TEnum GetByIdOrDefault(TId id)
        {
            return GetById(id) ?? GetDefault();
        }

        public static explicit operator TId(Enumeration<TEnum, TId> item)
        {
            return item.Id;
        }

        public static explicit operator Enumeration<TEnum, TId>(TId value)
        {
            return GetByIdOrDefault(value);
        }

        #region EqualityMembers

        protected bool Equals(Enumeration<TEnum, TId> other)
        {
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Enumeration<TEnum, TId>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }

        #endregion
    }
}