// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using log4net;
using MappingEdu.Common.Exceptions;
using MappingEdu.Common.Resources;

namespace MappingEdu.Web.UI.Infrastructure.Attributes
{
    /// <summary>
    ///     Handles exception to http status code mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal class ExceptionHandling : ExceptionFilterAttribute
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Executes when an exception occurs.
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            // Security Exception
            if (context.Exception is SecurityException)
            {
                Logger.Debug(context.Exception);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(context.Exception.Message),
                    ReasonPhrase = "Security"
                });
            }

            Logger.Error(context.Exception);

            // Not Found Exception
            if (context.Exception is NotFoundException)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(context.Exception.Message),
                    ReasonPhrase = "NotFound"
                });

            // Business Exception
            if (context.Exception is BusinessException)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(context.Exception.Message),
                    ReasonPhrase = "BadRequest"
                });

            var message = context.Exception.InnerException?.InnerException?.InnerException?.Message;
            message = HttpContext.Current.IsDebuggingEnabled ? message ?? context.Exception.Message : ExceptionResources.Error_Critical_Message;

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(message),
                ReasonPhrase = ExceptionResources.Error_Critical_Title
            });
        }
    }
}
