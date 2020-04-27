// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the ambient services to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>The provided service collection.</returns>
        public static IServiceCollection AddAmbientServices(
            this IServiceCollection serviceCollection,
            IAmbientServices ambientServices)
        {
            Requires.NotNull(serviceCollection, nameof(serviceCollection));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            serviceCollection.Add(new AmbientServicesServiceDescriptor(ambientServices));
            return serviceCollection;
        }

        /// <summary>
        /// Gets the ambient services from this service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="throwOnNotFound">If true, throws an exception if the ambient services were not already added using the <see cref="AddAmbientServices"/> method.</param>
        /// <returns>The ambient services.</returns>
        public static IAmbientServices? GetAmbientServices(this IServiceCollection serviceCollection, bool throwOnNotFound = true)
        {
            Requires.NotNull(serviceCollection, nameof(serviceCollection));

            var descriptor = serviceCollection.OfType<AmbientServicesServiceDescriptor>().FirstOrDefault();
            if (descriptor == null)
            {
                return throwOnNotFound
                    ? throw new ServiceException($"The ambient services were not found. Before getting, add them using the {nameof(AddAmbientServices)}() method.")
                    : (IAmbientServices?)null;
            }

            return descriptor.AmbientServices;
        }

        private class AmbientServicesServiceDescriptor : ServiceDescriptor
        {
            public AmbientServicesServiceDescriptor(IAmbientServices ambientServices)
                : base(typeof(AmbientServicesProvider), new AmbientServicesProvider())
            {
                // do not register directly the IAmbientServices not to influence the registration.
                // instead, use a dummy provider class which is private and will not be used.
                this.AmbientServices = ambientServices;
            }

            public IAmbientServices AmbientServices { get; }

            private class AmbientServicesProvider
            {
            }
        }
    }
}