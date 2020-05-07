// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Extensions.Hosting.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A host builder factory.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Integrates a new instance of the ambient services into the host builder and configures it.
        /// </summary>
        /// <remarks>
        /// For ASP.NET Core applications, do not create the container here, but instead in the Startup.ConfigureAmbientServices method.
        /// </remarks>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="optionsConfig">Optional. The ambient services configuration delegate.</param>
        /// <returns>The provided host builder.</returns>
        public static IHostBuilder ConfigureAmbientServices(this IHostBuilder hostBuilder, Action<HostBuilderContext, IConfigurationBuilder, IAmbientServices>? optionsConfig = null)
        {
            return ConfigureAmbientServices(hostBuilder, new AmbientServices(), null, optionsConfig);
        }

        /// <summary>
        /// Integrates a new instance of the ambient services into the host builder and configures it.
        /// </summary>
        /// <remarks>
        /// For ASP.NET Core applications, do not create the container here, but instead in the Startup.ConfigureAmbientServices method.
        /// </remarks>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="args">The application arguments.</param>
        /// <param name="optionsConfig">Optional. The ambient services configuration delegate.</param>
        /// <returns>The provided host builder.</returns>
        public static IHostBuilder ConfigureAmbientServices(this IHostBuilder hostBuilder, IEnumerable<string>? args, Action<HostBuilderContext, IConfigurationBuilder, IAmbientServices>? optionsConfig = null)
        {
            return ConfigureAmbientServices(hostBuilder, new AmbientServices(), args, optionsConfig);
        }

        /// <summary>
        /// Integrates the ambient services into the host builder and configures it.
        /// </summary>
        /// <remarks>
        /// For ASP.NET Core applications, do not create the container here, but instead in the Startup.ConfigureAmbientServices method.
        /// </remarks>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="args">The application arguments.</param>
        /// <param name="optionsConfig">Optional. The ambient services configuration delegate.</param>
        /// <returns>The provided host builder.</returns>
        public static IHostBuilder ConfigureAmbientServices(this IHostBuilder hostBuilder, IAmbientServices ambientServices, IEnumerable<string>? args, Action<HostBuilderContext, IConfigurationBuilder, IAmbientServices>? optionsConfig = null)
        {
            Requires.NotNull(hostBuilder, nameof(hostBuilder));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            hostBuilder
                .UseServiceProviderFactory(new CompositionServiceProviderFactory(ambientServices))
                .ConfigureServices(services => services.AddAmbientServices(ambientServices))
                .ConfigureAppConfiguration(
                    (ctx, cfg) =>
                    {
                        ambientServices.RegisterAppArgs(args);
                        optionsConfig?.Invoke(ctx, cfg, ambientServices);
                    });

            hostBuilder.Properties[nameof(IAmbientServices)] = ambientServices;
            return hostBuilder;
        }
    }
}