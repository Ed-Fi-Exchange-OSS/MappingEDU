// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Web.Http;
using Swashbuckle.Application;

namespace MappingEdu.Web.UI.Infrastructure.Config
{
    /// <summary>
    ///     Configures swashbuckle (]) settings.
    /// </summary>
    public class DocumentationConfig
    {
        /// <summary>
        ///     Registers the configuration.
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            config
                .EnableSwagger(c =>
                {
                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\bin";
                    var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                    var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                    c.SingleApiVersion("v1", "MappingEDU API")
                        .Description("The MappingEDU application")
                        .TermsOfService("Term TBD")
                        .Contact(cc => cc
                            .Name("Ed-Fi Alliance")
                            .Url(Configuration.Support.SupportUrl)
                            .Email(Configuration.Support.SupportEmailAddress))
                        .License(lc => lc
                            .Name("License")
                            .Url(Configuration.Support.LicenseUrl));

                    c.IncludeXmlComments(commentsFile);
                    c.UseFullTypeNameInSchemaIds();

                    //c
                    //    .OAuth2("oauth2")
                    //    .Description("Resource owner password credentials grant")
                    //    .Flow("password")
                    //    .AuthorizationUrl("http://localhost:32576/api/rest/v1/accesstoken");
                })
                .EnableSwaggerUi("api/docs/{*assetPath}");
        }
    }
}
