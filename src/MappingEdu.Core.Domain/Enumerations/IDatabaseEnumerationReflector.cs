// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#region

using System.Linq;
using System.Reflection;

#endregion

namespace MappingEdu.Core.Domain.Enumerations
{
    public interface IDatabaseEnumerationReflector
    {
        IDatabaseEnumeration[] GetDatabaseValues(string enumerationName);
    }

    public class DatabaseEnumerationReflector : IDatabaseEnumerationReflector
    {
        public IDatabaseEnumeration[] GetDatabaseValues(string enumerationName)
        {
            var databaseEnumerationInterfaceType = typeof (IDatabaseEnumeration);
            var enumerationType = typeof (DomainModule).Assembly
                .GetTypes()
                .Single(x => x.Name.Equals(enumerationName) && databaseEnumerationInterfaceType.IsAssignableFrom(x));
            var genericEnumerationType = typeof (DatabaseEnumeration<,>).MakeGenericType(enumerationType.BaseType.GetGenericArguments());
            var getDatabaseValuesMethod = genericEnumerationType.GetMethod("GetDatabaseValues", BindingFlags.Public | BindingFlags.Static);
            var databaseValues = (dynamic[]) getDatabaseValuesMethod.Invoke(null, null);
            return databaseValues.Cast<IDatabaseEnumeration>().ToArray();
        }
    }
}