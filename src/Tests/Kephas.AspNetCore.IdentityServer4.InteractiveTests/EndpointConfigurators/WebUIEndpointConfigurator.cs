// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebUIEndpointConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.EndpointConfigurators
{
    using Kephas.Application.AspNetCore;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Endpoint configurator for the web UI. Should be invoked after the default route.
    /// </summary>
    [ProcessingPriority(Priority.BelowNormal)]
    public class WebUIEndpointConfigurator : IEndpointConfigurator
    {
        /// <summary>
        /// Configures the endpoints using the given application context.
        /// </summary>
        /// <param name="endpoints">The endpoints builder.</param>
        /// <param name="appContext">Context for the application.</param>
        public void Configure(IEndpointRouteBuilder endpoints, IAspNetAppContext appContext)
        {
            endpoints.MapRazorPages();
        }
    }
}