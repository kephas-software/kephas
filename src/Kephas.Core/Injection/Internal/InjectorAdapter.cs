// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the composition context adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Kephas.Injection.Internal
{
    /// <summary>
    /// An adapter of <see cref="IServiceProvider"/> for <see cref="IInjector"/>.
    /// </summary>
    internal class InjectorAdapter : IInjector
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectorAdapter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public InjectorAdapter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType"/>.
        /// </returns>
        public object Resolve(Type contractType, string? serviceName = null)
        {
            return this.serviceProvider.GetService(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="contractType"/>.
        /// </returns>
        public IEnumerable<object> ResolveMany(Type contractType)
        {
            var collectionType = typeof(IEnumerable<>).MakeGenericType(contractType);
            return (IEnumerable<object>)this.serviceProvider.GetService(collectionType);
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public T Resolve<T>(string? serviceName = null)
            where T : class
        {
            return (T)this.serviceProvider.GetService(typeof(T));
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
            var collectionType = typeof(IEnumerable<>).MakeGenericType(typeof(T));
            return (IEnumerable<T>)this.serviceProvider.GetService(collectionType);
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType"/>, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public object TryResolve(Type contractType, string? serviceName = null)
        {
            return this.serviceProvider.GetService(contractType);
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public T? TryResolve<T>(string? serviceName = null)
            where T : class
        {
            return (T?)this.serviceProvider.GetService(typeof(T));
        }

        /// <summary>
        /// Creates a new scoped composition context.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        IInjector IInjector.CreateScopedInjector()
        {
            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
}