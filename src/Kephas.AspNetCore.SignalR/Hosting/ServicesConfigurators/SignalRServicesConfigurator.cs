// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalRServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.SignalR.Hosting.ServicesConfigurators
{
    using Kephas;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Services configurator for SignalR.
    /// </summary>
    [ProcessingPriority(ProcessingPriority)]
    public class SignalRServicesConfigurator : IServicesConfigurator
    {
        /// <summary>
        /// The processing priority of <see cref="SignalRServicesConfigurator"/>.
        /// </summary>
        public const Priority ProcessingPriority = Priority.High + 300;

        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="context">The host builder context.</param>
        /// <param name="services">The service collection.</param>
        /// <param name="servicesBuilder">The services builder.</param>
        public void ConfigureServices(
            HostBuilderContext context,
            IServiceCollection services,
            IAppServiceCollectionBuilder servicesBuilder)
        {
            services.AddSignalR();
        }
    }
}
