// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEndpointConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting
{
    using Kephas.Application.AspNetCore;
    using Kephas.Services;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// Service contract for configuring the endpoints.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IEndpointConfigurator
    {
        /// <summary>
        /// Configures the endpoints using the given application context.
        /// </summary>
        /// <param name="endpoints">The endpoints builder.</param>
        /// <param name="appContext">Context for the application.</param>
        void Configure(IEndpointRouteBuilder endpoints, IAspNetAppContext appContext);
    }
}