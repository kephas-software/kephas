// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndpointsMiddlewareConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting.MiddlewareConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Application.AspNetCore;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Middleware configurator for endpoints.
    /// </summary>
    [ProcessingPriority(Priority.BelowNormal)]
    public class EndpointsMiddlewareConfigurator : Loggable, IMiddlewareConfigurator
    {
        private readonly ICollection<Lazy<IEndpointConfigurator, AppServiceMetadata>> lazyConfigurators;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointsMiddlewareConfigurator"/> class.
        /// </summary>
        /// <param name="lazyConfigurators">The lazy endpoint configurators.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public EndpointsMiddlewareConfigurator(
            ICollection<Lazy<IEndpointConfigurator, AppServiceMetadata>> lazyConfigurators,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.lazyConfigurators = lazyConfigurators;
        }

        /// <summary>
        /// Configures the host using the given application context.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        public void Configure(IAspNetAppContext appContext)
        {
            var app = appContext.AppBuilder;
            var env = appContext.HostEnvironment;

            var configurators = this.lazyConfigurators
                .Order()
                .Select(lc => lc.Value)
                .ToList();
            app.UseEndpoints(endpoints =>
            {
                foreach (var configurator in configurators)
                {
                    configurator.Configure(endpoints, appContext);
                }
            });
        }
    }
}