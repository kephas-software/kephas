// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Adapter for <see cref="System.IServiceProvider"/> based on <see cref="Injection.IServiceProvider"/>.
    /// </summary>
    internal class ServiceProviderAdapter : System.IServiceProvider
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/>
        /// class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        public ServiceProviderAdapter(IServiceProvider serviceProvider)
        {
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service
        /// object of type <paramref name="serviceType" />.
        /// </returns>
        public object? GetService([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type serviceType)
        {
            return this.serviceProvider.TryResolve(serviceType);
        }
    }
}