// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostBuilderFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;

namespace Kephas.Extensions.Hosting
{
    using System;

    using Kephas.Extensions.Hosting.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// A host builder factory.
    /// </summary>
    public class HostBuilderFactory
    {
        private readonly string[]? args;
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostBuilderFactory"/> class.
        /// </summary>
        /// <param name="args">Optional. The app startup arguments.</param>
        /// <param name="ambientServices">Optional. The ambient services. If not provided, a default value will be created.</param>
        public HostBuilderFactory(string[]? args = null, IAmbientServices? ambientServices = null)
        {
            this.args = args;
            this.ambientServices = ambientServices ?? new AmbientServices();
        }

        /// <summary>
        /// Creates a host builder.
        /// </summary>
        /// <param name="ambientServicesConfig">Optional. The ambient services configuration delegate.</param>
        /// <returns>A new configured host builder.</returns>
        public virtual IHostBuilder CreateHostBuilder(Action<HostBuilderContext, IConfigurationBuilder, IAmbientServices>? ambientServicesConfig = null)
        {
            var hostBuilder = Host.CreateDefaultBuilder(this.args)
                .UseServiceProviderFactory(new CompositionServiceProviderFactory(this.ambientServices))
                .ConfigureServices(services => services.AddAmbientServices(this.ambientServices))
                .ConfigureAppConfiguration((ctx, cfg) => ambientServicesConfig?.Invoke(ctx, cfg, this.ambientServices));
            return hostBuilder;
        }
    }
}