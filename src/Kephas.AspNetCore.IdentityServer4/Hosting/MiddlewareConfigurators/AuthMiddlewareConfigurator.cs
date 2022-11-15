// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthMiddlewareConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Hosting.MiddlewareConfigurators
{
    using Kephas.Application.AspNetCore;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Services;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Middleware configurator for authentication.
    /// </summary>
    /// <seealso cref="IMiddlewareConfigurator" />
    [ProcessingPriority(ProcessingPriority)]
    public class AuthMiddlewareConfigurator : IMiddlewareConfigurator
    {
        /// <summary>
        /// The processing priority of <see cref="AuthMiddlewareConfigurator"/>.
        /// </summary>
        public const Priority ProcessingPriority = Priority.High;

        /// <summary>
        /// Configures a specific middleware using the given application context.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        public void Configure(IAspNetAppContext appContext)
        {
            var app = appContext.AppBuilder;

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
        }
    }
}
