// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel;

namespace MappingEdu.Common.Util
{
    public static class ConversionUtil
    {
        /// <summary>
        ///     Converts an object to a boolean value.
        ///     Returns default(bool) if error.
        /// </summary>
        /// <param name="value">The vaue to convert</param>
        public static bool Bool(object value)
        {
            bool convertedInt;
            bool.TryParse(Convert.ToString(value), out convertedInt);
            return convertedInt;
        }

        /// <summary>
        ///     Converts a type.
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="value">The value to convert</param>
        public static T ChangeType<T>(object value)
        {
            return (T) ChangeType(typeof (T), value);
        }

        /// <summary>
        ///     Converts a type.
        /// </summary>
        /// <param name="t">The type to convert to</param>
        /// <param name="value">The value to convert</param>
        public static object ChangeType(Type t, object value)
        {
            var tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }

        /// <summary>
        ///     Converts an object to a double.
        ///     Returns default(double) if error.
        /// </summary>
        /// <param name="value">The vaue to convert</param>
        public static double Double(object value)
        {
            double convertedDouble;
            double.TryParse(Convert.ToString(value), out convertedDouble);
            return convertedDouble;
        }

        /// <summary>
        ///     Converts a string value into corresponing enumeration.
        ///     Returns default(T) if not found.
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The value to convert</param>
        public static T Enum<T>(object value) where T : struct // enum 
        {
            if (!typeof (T).IsEnum)
                throw new Exception("Type given must be an Enum");
            try
            {
                return (T) System.Enum.Parse(typeof (T), value.ToString(), true);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        ///     Converts an object to a guid value.
        ///     Returns default(bool) if error.
        /// </summary>
        /// <param name="value">The vaue to convert</param>
        /// <returns></returns>
        public static Guid Guid(object value)
        {
            Guid convertedGuid;
            System.Guid.TryParse(Convert.ToString(value), out convertedGuid);
            return convertedGuid;
        }

        /// <summary>
        ///     Converts an object to an integer.
        ///     Returns default(int) if error.
        /// </summary>
        /// <param name="value">The value to convert</param>
        public static int Int(object value)
        {
            int convertedInt;
            int.TryParse(Convert.ToString(value), out convertedInt);
            return convertedInt;
        }

        /// <summary>
        ///     Converts an object to an integer.
        ///     Returns default(int) if error.
        /// </summary>
        /// <param name="value">The vaue to convert</param>
        /// <param name="defaultValue">The default value</param>
        public static int Int(object value, int defaultValue)
        {
            var result = Int(value);
            return result == default(int) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts an object to a long.
        ///     Returns default(long) if error.
        /// </summary>
        /// <param name="value">The value to convert</param>
        public static long Long(object value)
        {
            long convertedInt;
            long.TryParse(Convert.ToString(value), out convertedInt);
            return convertedInt;
        }

        /// <summary>
        ///     Registers a type converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TC"></typeparam>
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof (T), new TypeConverterAttribute(typeof (TC)));
        }

        /// <summary>
        ///     Converts an object to an integer.
        ///     Returns default(int) if error.
        /// </summary>
        /// <param name="value">The vaue to convert</param>
        public static short Short(object value)
        {
            short convertedInt;
            short.TryParse(Convert.ToString(value), out convertedInt);
            return convertedInt;
        }
    }
}