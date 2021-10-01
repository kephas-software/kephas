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

    using Kephas.Diagnostics.Contracts;
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
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>
        /// An IServiceCollection.
        /// </returns>
        public static IAmbientServices WithServiceCollection(this IAmbientServices ambientServices, IServiceCollection serviceCollection)
        {
            Requires.NotNull(serviceCollection, nameof(serviceCollection));
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

            // make sure to register the service collection *BEFORE* the attributed service provider.
            ambientServices.Register(serviceCollection);
            serviceCollection.Replace(ServiceDescriptor.Transient<IServiceScopeFactory, InjectionServiceScopeFactory>());
            serviceCollection.TryAddSingleton<IServiceProvider>(provider => provider);

            return ambientServices;
        }
    }
}