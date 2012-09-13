﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiManager.cs" company="Waking Venture, Inc.">
//   Copyright (c) 2012 Waking Venture, Inc.
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//   and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions 
//   of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//   DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NContext.Extensions.AspNetWebApi.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Web.Http;
    using System.Web.Http.SelfHost;

    using NContext.Configuration;
    using NContext.Extensions.AspNetWebApi.Routing;

    /// <summary>
    /// Defines an application-level manager for configuring WCF service routes.
    /// </summary>
    public class WebApiManager : IManageWebApi
    {
        private readonly WebApiConfiguration _WebApiConfiguration;

        private readonly Lazy<HttpSelfHostServer> _SelfHostServer;

        private readonly Lazy<IList<Route>> _ServiceRoutes;

        private CompositionContainer _CompositionContainer;

        private Boolean _IsConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiManager"/> class.
        /// Prevents a default instance of the <see cref="WebApiManager"/> class from being created.
        /// </summary>
        /// <param name="webApiConfiguration">The routing configuration.</param>
        /// <remarks></remarks>
        public WebApiManager(WebApiConfiguration webApiConfiguration)
        {
            if (webApiConfiguration == null)
            {
                throw new ArgumentNullException("webApiConfiguration");
            }

            _ServiceRoutes = new Lazy<IList<Route>>(() => new List<Route>());
            _WebApiConfiguration = webApiConfiguration;
            
            if (webApiConfiguration.IsSelfHosted)
            {
                _SelfHostServer = new Lazy<HttpSelfHostServer>(() => new HttpSelfHostServer(WebApiConfiguration.HttpSelfHostConfiguration));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is configured.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is configured; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsConfigured
        {
            get
            {
                return _IsConfigured;
            }
            protected set
            {
                _IsConfigured = value;
            }
        }

        /// <summary>
        /// Gets the service routes registered.
        /// </summary>
        /// <remarks></remarks>
        public ICollection<Route> HttpRoutes
        {
            get
            {
                return _ServiceRoutes.Value;
            }
        }

        /// <summary>
        /// Gets or sets the composition container.
        /// </summary>
        /// <value>The composition container.</value>
        /// <remarks></remarks>
        protected CompositionContainer CompositionContainer
        {
            get
            {
                return _CompositionContainer;
            }

            set
            {
                _CompositionContainer = value;
            }
        }

        protected HttpSelfHostServer SelfHostServer
        {
            get
            {
                return _SelfHostServer.Value;
            }
        }

        protected WebApiConfiguration WebApiConfiguration
        {
            get
            {
                return _WebApiConfiguration;
            }
        }

        /// <summary>
        /// Registers the service route.
        /// </summary>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeTemplate">The route template.</param>
        /// <param name="defaults">The defaults.</param>
        /// <param name="constraints">The constraints.</param>
        /// <remarks></remarks>
        public virtual void RegisterHttpRoute(String routeName, String routeTemplate, Object defaults = null, Object constraints = null)
        {
            HttpRoutes.Add(new Route(routeName, routeTemplate, defaults, constraints));
        }

        /// <summary>
        /// Registers the routes in the routing collection.
        /// </summary>
        protected virtual void CreateRoutes()
        {
            var serviceRouteCreatedActions = CompositionContainer.GetExports<IRunWhenAWebApiRouteIsMapped>();
            if (WebApiConfiguration.IsSelfHosted)
            {
                HttpRoutes.ForEach(
                    route =>
                        {
                            SelfHostServer.Configuration.Routes
                                .MapHttpRoute(route.RouteName, route.RouteTemplate, route.Defaults, route.Constraints);

                            serviceRouteCreatedActions.ForEach(createdAction => createdAction.Value.Run(route));
                        });

                SelfHostServer.OpenAsync().Wait();
            }
            else
            {
                HttpRoutes.ForEach(
                    route =>
                        {
                            GlobalConfiguration
                                .Configuration
                                .Routes
                                .MapHttpRoute(route.RouteName, route.RouteTemplate, route.Defaults, route.Constraints);

                            serviceRouteCreatedActions.ForEach(createdAction => createdAction.Value.Run(route));
                        });
            }
        }

        /// <summary>
        /// Configures the component instance.
        /// </summary>
        /// <param name="applicationConfiguration">The application configuration.</param>
        /// <remarks></remarks>
        public virtual void Configure(ApplicationConfigurationBase applicationConfiguration)
        {
            if (IsConfigured) return;

            CompositionContainer = applicationConfiguration.CompositionContainer;
            if (!WebApiConfiguration.IsSelfHosted)
            {
                if (WebApiConfiguration.AspNetHttpConfigurationDelegate != null)
                {
                    WebApiConfiguration.AspNetHttpConfigurationDelegate.Invoke(GlobalConfiguration.Configuration);
                }
            }

            var serviceConfigurables = _CompositionContainer.GetExports<IConfigureWebApi>();
            foreach (var serviceConfigurable in serviceConfigurables)
            {
                serviceConfigurable.Value.Configure(this);
            }

            CreateRoutes();

            IsConfigured = true;
        }
    }
}