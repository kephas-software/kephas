// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using Kephas.Services;
    using Kephas.Services.Builder;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Marker interface for configuring services of a <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="SingletonAppServiceContractAttribute"/> marker is provided only to collect
    /// the metadata about the configurators at runtime. Implementors should provide a default constructor,
    /// otherwise the services will not be created.
    /// </remarks>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IServicesConfigurator
    {
        /// <summary>
        /// Configure the services.
        /// </summary>
        /// <param name="context">The host builder context.</param>
        /// <param name="services">The service collection.</param>
        /// <param name="servicesBuilder">The services builder.</param>
        void ConfigureServices(HostBuilderContext context, IServiceCollection services, IAppServiceCollectionBuilder servicesBuilder);
    }
}