// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using Kephas.Diagnostics.Contracts;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Extension methods for <see cref="IWebHostBuilder"/>
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Configures the web host builder using the ambient services.
        /// </summary>
        /// <param name="webHostBuilder">The web host builder.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>The provided web host builder.</returns>
        public static IWebHostBuilder UseAmbientServices(this IWebHostBuilder webHostBuilder, IAmbientServices ambientServices)
        {
            Requires.NotNull(webHostBuilder, nameof(webHostBuilder));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            webHostBuilder
                .UseSetting(WebHostDefaults.ApplicationKey, ambientServices.AppRuntime.GetAppId());

            return webHostBuilder;
        }
    }
}