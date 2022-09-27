// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionInjectorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.DependencyInjection.Hosting
{
    using System;
    using System.Collections.Generic;

    using Kephas.Injection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// An injector base for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public abstract class DependencyInjectionServiceProviderBase : IServiceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionServiceProviderBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected DependencyInjectionServiceProviderBase(System.IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        protected System.IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />.
        /// </returns>
        public object Resolve(Type contractType)
        {
            return this.ServiceProvider.GetRequiredService(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="contractType" />.
        /// </returns>
        public IEnumerable<object> ResolveMany(Type contractType)
        {
            return this.ServiceProvider.GetServices(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public T Resolve<T>()
            where T : class
        {
            return this.ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        public IEnumerable<T> ResolveMany<T>()
            where T : class
        {
            return this.ServiceProvider.GetServices<T>();
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public object? TryResolve(Type contractType)
        {
            return this.ServiceProvider.GetService(contractType);
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public T? TryResolve<T>()
            where T : class
        {
            return this.ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Creates a new scoped injector.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        public IInjectionScope CreateScope()
        {
            return new DependencyInjectionScopedServiceProvider(this.ServiceProvider.CreateScope());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public virtual void Dispose()
        {
            (this.ServiceProvider as IDisposable)?.Dispose();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType">serviceType</paramref>.   -or-  null if
        /// there is no service object of type <paramref name="serviceType">serviceType</paramref>.
        /// </returns>
        public object GetService(Type serviceType) => this.ServiceProvider.GetService(serviceType);
    }
}