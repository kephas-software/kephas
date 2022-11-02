// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostingServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Microsoft.Extensions.DependencyInjection related ambient services extensions.
    /// </summary>
    public static class HostingServiceCollectionExtensions
    {
        /// <summary>
        /// Gets the services configurators.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <returns>The <paramref name="hostBuilder"/>.</returns>
        public static IHostBuilder UseServicesConfigurators(this IHostBuilder hostBuilder, IAppServiceCollectionBuilder servicesBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                try
                {
                    foreach (var configurator in GetServicesConfiguratorActions(servicesBuilder, context))
                    {
                        configurator(services);
                    }
                }
                catch (Exception ex)
                {
                    servicesBuilder.Logger.Fatal(ex, "Errors occurred during service configurator invocation.");
                    throw;
                }
            });

            return hostBuilder;
        }

        /// <summary>
        /// Gets the services configurators.
        /// </summary>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <returns>An enumeration of <see cref="IServicesConfigurator"/>.</returns>
        private static IEnumerable<IServicesConfigurator> GetServicesConfigurators(this IAppServiceCollectionBuilder servicesBuilder)
        {
            var configuratorTypes = servicesBuilder.AmbientServices
                .Where(t => t.ContractDeclarationType == typeof(IServicesConfigurator) || t.ContractType == typeof(IServicesConfigurator))
                .Select(t => t.InstanceType)
                .Where(t => t is not null)
                .ToList();
            var orderedConfiguratorTypes = configuratorTypes
                .Select(t => new Lazy<IServicesConfigurator, AppServiceMetadata>(
                    () => (IServicesConfigurator)Activator.CreateInstance(t),
                    new AppServiceMetadata(ServiceHelper.GetServiceMetadata(t, typeof(IServicesConfigurator)))))
                .Order();
            return orderedConfiguratorTypes.Select(f => f.Value);
        }

        private static IEnumerable<Action<IServiceCollection>> GetServicesConfiguratorActions(this IAppServiceCollectionBuilder servicesBuilder, HostBuilderContext context)
            => servicesBuilder.GetServicesConfigurators()
                .Select(e => GetServicesConfiguratorAction(e, context, servicesBuilder));


        private static Action<IServiceCollection> GetServicesConfiguratorAction(IServicesConfigurator c, HostBuilderContext context, IAppServiceCollectionBuilder servicesBuilder)
        {
            var logger = servicesBuilder.Logger;
            return s =>
            {
                logger.Debug($"Configuring services by {s.GetType()}...");
                c.ConfigureServices(context, s, servicesBuilder);
                logger.Debug($"Services configured by {s.GetType()}.");
            };
        }
    }
}
