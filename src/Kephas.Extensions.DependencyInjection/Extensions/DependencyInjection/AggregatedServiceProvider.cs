// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatedServiceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aggregated service provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// An aggregated service provider.
    /// </summary>
    public class AggregatedServiceProvider : IServiceProvider, IDisposable
    {
        private readonly IList<IServiceProvider> innerServiceProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedServiceProvider"/> class.
        /// </summary>
        /// <param name="innerServiceProviders">A variable-length parameters list containing inner
        ///                                     service providers.</param>
        public AggregatedServiceProvider(params IServiceProvider[] innerServiceProviders)
        {
            if (innerServiceProviders == null || innerServiceProviders.Length == 0) throw new System.ArgumentException("Value must not be null or empty.", nameof(innerServiceProviders));

            this.innerServiceProviders = innerServiceProviders.ToList();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType">serviceType</paramref>.   -or-  null if
        /// there is no service object of type <paramref name="serviceType">serviceType</paramref>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            foreach (var serviceProvider in this.innerServiceProviders)
            {
                var service = serviceProvider.GetService(serviceType);
                if (service != null)
                {
                    return service;
                }
            }

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var serviceProvider in this.innerServiceProviders)
            {
                (serviceProvider as IDisposable)?.Dispose();
            }

            this.innerServiceProviders.Clear();
        }
    }
}