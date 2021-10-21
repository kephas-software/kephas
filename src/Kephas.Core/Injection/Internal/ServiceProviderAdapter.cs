// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// Adapter for <see cref="IServiceProvider"/> based on <see cref="IInjector"/>.
    /// </summary>
    internal class ServiceProviderAdapter : IServiceProvider
    {
        private readonly IInjector injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/>
        /// class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public ServiceProviderAdapter(IInjector injector)
        {
            injector = injector ?? throw new ArgumentNullException(nameof(injector));

            this.injector = injector;
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.-or- null if there is no service
        /// object of type <paramref name="serviceType" />.
        /// </returns>
        public object? GetService(Type serviceType)
        {
            return this.injector.TryResolve(serviceType);
        }
    }
}