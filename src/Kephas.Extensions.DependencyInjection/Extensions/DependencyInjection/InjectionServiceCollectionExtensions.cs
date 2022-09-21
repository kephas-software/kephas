// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class InjectionServiceCollectionExtensions
    {
        /// <summary>
        /// Includes the service collection in the composition.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>
        /// An IServiceCollection.
        /// </returns>
        public static IServiceCollection UseAmbientServices(this IServiceCollection services, IAmbientServices ambientServices)
        {
            services = services ?? throw new ArgumentNullException(nameof(services));
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            // make sure to register the service collection *BEFORE* the attributed service provider.
            ambientServices.Add(services);
            services.Replace(ServiceDescriptor.Transient<IServiceScopeFactory, InjectionServiceScopeFactory>());
            services.TryAddSingleton<IServiceProvider>(provider => provider);

            return services;
        }
    }
}