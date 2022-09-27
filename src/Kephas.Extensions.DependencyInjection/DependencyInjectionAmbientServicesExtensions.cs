﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Microsoft.Extensions.DependencyInjection related ambient services extensions.
    /// </summary>
    public static class DependencyInjectionAmbientServicesExtensions
    {
        /// <summary>
        /// Builds the service provider using the service collection.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="builderOptions">The builder configuration.</param>
        /// <returns>The built service provider.</returns>
        public static IServiceProvider BuildWithDependencyInjection(this IAmbientServices ambientServices, Action<IServiceCollection> builderOptions)
        {
            var services = new ServiceCollection();
            builderOptions?.Invoke(services);

            return ambientServices.BuildWithDependencyInjection(services);
        }

        /// <summary>
        /// Builds the service provider using the service collection.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="services">The service collection.</param>
        /// <returns>The built service provider.</returns>
        public static IServiceProvider BuildWithDependencyInjection(this IAmbientServices ambientServices, IServiceCollection services)
        {
            services.UseAmbientServices(ambientServices);

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets the services configurators.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>An enumeration of <see cref="IServicesConfigurator"/>.</returns>
        public static IEnumerable<IServicesConfigurator> GetServicesConfigurators(this IAppRuntime appRuntime)
        {
            var appAssemblies = appRuntime.GetAppAssemblies();
            var configuratorTypes = ServiceHelper.GetAppServiceInfosProviders(appAssemblies)
                .SelectMany(p => p.GetAppServices())
                .Where(t => t.ContractDeclarationType == typeof(IServicesConfigurator))
                .Select(t => t.ServiceType)
                .ToList();
            var orderedConfiguratorTypes = configuratorTypes
                .Select(t => new Lazy<IServicesConfigurator, AppServiceMetadata>(
                    () => (IServicesConfigurator)Activator.CreateInstance(t),
                    new AppServiceMetadata(ServiceHelper.GetServiceMetadata(t, typeof(IServicesConfigurator)))))
                .Order();
            return orderedConfiguratorTypes.Select(f => f.Value);
        }
    }
}