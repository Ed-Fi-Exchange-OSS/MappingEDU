// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using MappingEdu.Common;
using MappingEdu.Service;
using MappingEdu.Web.UI.Infrastructure.Helpers;
using MappingEdu.Web.UI.Infrastructure.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ControllerBase = MappingEdu.Web.UI.Controllers.ApiControllers.ControllerBase;

namespace MappingEdu.Web.UI.Infrastructure.Config
{
    /// <summary>
    ///     Configures the IOC container
    /// </summary>
    public class ContainerConfig
    {
        /// <summary>
        ///     Registers the autofac configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="app"></param>
        public static void Register(IAppBuilder app, HttpConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            // modules
            builder.RegisterModule(new ServicesModule());

            // mvc controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // controllers
            builder.RegisterApiControllers(typeof (ControllerBase).Assembly);

            // request
            builder.RegisterHttpRequestMessage(configuration);

            // authorization
            builder.RegisterType<AuthorizationServerProvider>().As<IOAuthAuthorizationServerProvider>().SingleInstance();
            builder.RegisterType<AuthorizationServerHelper>().As<IAuthorizationServerHelper>().SingleInstance();

            // set resolver
            var container = builder.Build();

            // Set the dependency resolver for Web API.
            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            configuration.DependencyResolver = webApiResolver;

            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(configuration);

            ConfigureOAuth(app, container);

            app.UseWebApi(configuration);
        }

        private static void ConfigureOAuth(IAppBuilder app, IContainer container)
        {
            var serverOptions = new OAuthAuthorizationServerOptions
            {
                AccessTokenExpireTimeSpan = Configuration.AccessToken.ExpireTimeSpan,
                AllowInsecureHttp = Configuration.AccessToken.AllowInsecureHttp,
                Provider = container.Resolve<IOAuthAuthorizationServerProvider>(),
                TokenEndpointPath = new PathString(Constants.Api.V1.AccessTokenRoute), // absolute for oauth
            };

            Startup.OAuthServerOptions = serverOptions;

            // Token Generation
            app.UseOAuthAuthorizationServer(serverOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

    }
}
