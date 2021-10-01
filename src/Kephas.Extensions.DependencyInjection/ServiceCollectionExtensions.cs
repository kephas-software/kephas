// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
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
            ambientServices = ambientServices ?? throw new ArgumentNullException(nameof(ambientServices));

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

        /// <summary>
        /// Tries to get a service inside the startup pipeline.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>The service instance, if one could be found.</returns>
        public static T? TryGetStartupService<T>(this IServiceCollection serviceCollection)
        {
            Requires.NotNull(serviceCollection, nameof(serviceCollection));

            var serviceDescriptor = serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(T));
            if (serviceDescriptor == null)
            {
                return default;
            }

            if (serviceDescriptor.ImplementationInstance != null)
            {
                return (T)serviceDescriptor.ImplementationInstance;
            }

            if (serviceDescriptor.ImplementationFactory != null)
            {
                return (T)serviceDescriptor.ImplementationFactory(null);
            }

            return default;
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