// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionCompositionContextBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi composition context base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.DependencyInjection.Hosting
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A composition context base for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public abstract class DependencyInjectionCompositionContextBase : ICompositionContext, IServiceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionCompositionContextBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected DependencyInjectionCompositionContextBase(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />.
        /// </returns>
        public object GetExport(Type contractType, string serviceName = null)
        {
            return this.ServiceProvider.GetRequiredService(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName"></param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="contractType" />.
        /// </returns>
        public IEnumerable<object> GetExports(Type contractType, string serviceName = null)
        {
            return this.ServiceProvider.GetServices(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName"></param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public T GetExport<T>(string serviceName = null)
        {
            return this.ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName"></param>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        public IEnumerable<T> GetExports<T>(string serviceName = null)
        {
            return this.ServiceProvider.GetServices<T>();
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName"></param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public object TryGetExport(Type contractType, string serviceName = null)
        {
            return this.ServiceProvider.GetService(contractType);
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName"></param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public T TryGetExport<T>(string serviceName = null)
        {
            return this.ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Creates a new scoped composition context.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        public ICompositionContext CreateScopedContext()
        {
            return new DependencyInjectionScopedCompositionContext(this.ServiceProvider.CreateScope());
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