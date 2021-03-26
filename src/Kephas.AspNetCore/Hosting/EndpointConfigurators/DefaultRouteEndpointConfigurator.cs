// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRouteEndpointConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting.EndpointConfigurators
{
    using Kephas.Application.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// The endpoint configurator for the default route.
    /// </summary>
    public class DefaultRouteEndpointConfigurator : IEndpointConfigurator
    {
        /// <summary>
        /// Configures the endpoints using the given application context.
        /// </summary>
        /// <param name="endpoints">The endpoints builder.</param>
        /// <param name="appContext">Context for the application.</param>
        public void Configure(IEndpointRouteBuilder endpoints, IAspNetAppContext appContext)
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
        }
    }
}