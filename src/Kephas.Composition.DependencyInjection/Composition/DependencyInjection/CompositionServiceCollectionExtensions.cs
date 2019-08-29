// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection
{
    using System;

    using Kephas.Diagnostics.Contracts;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class CompositionServiceCollectionExtensions
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
            Requires.NotNull(ambientServices, nameof(ambientServices));

            ambientServices.Register(serviceCollection);
            serviceCollection.Replace(ServiceDescriptor.Transient<IServiceScopeFactory, CompositionServiceScopeFactory>());
            serviceCollection.TryAddSingleton<IServiceProvider>(provider => provider);

            return ambientServices;
        }
    }
}