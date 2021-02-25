// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using Kephas.Services;
    using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="services">The services to configure.</param>
        /// <param name="ambientServices">The ambient services.</param>
        void ConfigureServices(IServiceCollection services, IAmbientServices ambientServices);
    }
}