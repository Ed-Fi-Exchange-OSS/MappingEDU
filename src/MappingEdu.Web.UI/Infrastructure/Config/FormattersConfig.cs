// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Http;

namespace MappingEdu.Web.UI.Infrastructure.Config
{
    /// <summary>
    ///     Configures the json formatters
    /// </summary>
    public class FormattersConfig
    {
        /// <summary>
        ///     Registers the configuration
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            //config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            //config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            //config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;

            //var isoConverter = new IsoDateTimeConverter {DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK"};
            //config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(isoConverter);
        }
    }
}